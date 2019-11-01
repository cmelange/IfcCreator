using System.Collections.Generic;
using System.IO;
using Xunit;

using BuildingSmart.IFC.IfcProductExtension;
using BuildingSmart.IFC.IfcUtilityResource;
using BuildingSmart.IFC.IfcGeometricModelResource;
using BuildingSmart.IFC.IfcKernel;
using BuildingSmart.IFC.IfcRepresentationResource;
using BuildingSmart.Serialization;
using BuildingSmart.Serialization.Step;

using IfcCreator.Interface;
using IfcCreator.Interface.DTO;

namespace IfcCreator
{
    public class ProductIfcCreatorTest
    {
        [Fact]
        public void CreateProductIfcTest()
        {
            // === create request
            string projectName = "test project";
            string projectDescription = "project for test purposes";
            Project project = 
                new Project.Builder().withName(projectName)
                                     .withDescription(projectDescription)
                                     .Build();
            string personGivenName = "Cedric";
            string personFamilyName = "Mélange";
            string personIdentifier = "cmela";
            Person person = 
                new Person.Builder().withGivenName(personGivenName)
                                    .withFamilyName(personFamilyName)
                                    .withIdentifier(personIdentifier)
                                    .Build();
            string organizationName = "EC Life";
            string organizationDescription = "EC Life - for an easy, economical and ecological life";
            string organizationIdentifier = "ECL";
            Organization organization =
                new Organization.Builder().withName(organizationName)
                                          .withDescription(organizationDescription)
                                          .withIdentifier(organizationIdentifier)
                                          .Build();
            string applicationName = "ECL IfcCreator";
            string applicationVersion = "1.0";
            string applicationIdentifier = "IfcCreator";
            Application application =
                new Application.Builder().withName(applicationName)
                                         .withVersion(applicationVersion)
                                         .withIdentifier(applicationIdentifier)
                                         .withOrganization(new Organization.Builder().Build())
                                         .Build();
            OwnerHistory owner = 
                new OwnerHistory.Builder().withPerson(person)
                                          .withOrganization(organization)
                                          .withApllication(application)
                                          .Build();

            string productName = "test product";
            string productDescription = "product for test purposes";
            string constructionString = "POLYGON_SHAPE([[[0,0],[0,1],[1,1],[1,0]]]).EXTRUDE(1)";
            RepresentationItem representationItem =new RepresentationItem(constructionString);
            Representation representation = 
                new Representation.Builder().AddRepresentationItem(representationItem)
                                            .Build();
            Product product =
                new Product.Builder().withName(productName)
                                     .withDescription(productDescription)
                                     .withType(ProductType.PROXY)
                                     .addRepresenation(representation)
                                     .Build();
            IfcSchema schema = IfcSchema.IFC2X3;
            ProductIfcRequest request = 
                new ProductIfcRequest.Builder().withProject(project)
                                               .withOwner(owner)
                                               .withProduct(product)
                                               .withSchema(schema)
                                               .Build();

            // === convert to IFC stream
            MemoryStream memStream = new MemoryStream();
            IProductIfcCreator productIfcCreator = new ProductIfcCreator();
            productIfcCreator.CreateProductIfc(request, memStream);

            // === write to IFC file
            using (FileStream fileStream = File.Create("./product__ifc_creator_test.ifc"))
            {
               memStream.WriteTo(fileStream);
            }

            // === parse IFC stream
            Serializer serializer = new StepSerializer(typeof(IfcProject),
                                                       null,
                                                       schema.ToString(),
                                                       null,
                                                       "ECL IfcCreator");
            IfcProject parsedProject = (IfcProject) serializer.ReadObject(memStream);

            // === test values
            Assert.Equal(projectName, parsedProject.Name);
            Assert.Equal(projectDescription, parsedProject.Description);
            IfcOwnerHistory parsedowner = parsedProject.OwnerHistory;
            Assert.Equal(personGivenName, parsedowner.OwningUser.ThePerson.GivenName);
            Assert.Equal(personFamilyName, parsedowner.OwningUser.ThePerson.FamilyName);
            Assert.Equal(personIdentifier, parsedowner.OwningUser.ThePerson.Identification.Value.Value);
            Assert.Equal(organizationName, parsedowner.OwningUser.TheOrganization.Name);
            Assert.Equal(organizationDescription, parsedowner.OwningUser.TheOrganization.Description);
            Assert.Equal(applicationName, parsedowner.OwningApplication.ApplicationFullName);
            Assert.Equal(applicationVersion, parsedowner.OwningApplication.Version);
            Assert.Equal(applicationIdentifier, parsedowner.OwningApplication.ApplicationIdentifier.Value);
            IfcSite parsedSite = (IfcSite) GetFirst(GetFirst(parsedProject.IsDecomposedBy).RelatedObjects);
            Assert.Equal(personGivenName, parsedSite.OwnerHistory.OwningUser.ThePerson.GivenName);
            IfcBuilding parsedBuilding = (IfcBuilding) GetFirst(GetFirst(parsedSite.IsDecomposedBy).RelatedObjects);
            Assert.Equal(personGivenName, parsedBuilding.OwnerHistory.OwningUser.ThePerson.GivenName);
            IfcBuildingStorey parsedStorey = (IfcBuildingStorey) GetFirst(GetFirst(parsedBuilding.IsDecomposedBy).RelatedObjects);
            Assert.Equal(personGivenName, parsedStorey.OwnerHistory.OwningUser.ThePerson.GivenName);
            IfcProduct parsedProduct = (IfcProduct) GetFirst(GetFirst(parsedStorey.ContainsElements).RelatedElements);
            Assert.Equal(personGivenName, parsedProduct.OwnerHistory.OwningUser.ThePerson.GivenName);
            Assert.Equal(productName, parsedProduct.Name);
            Assert.Equal(productDescription, parsedProduct.Description);
            Assert.Equal(1, parsedProduct.Representation.Representations.Count);
            IfcRepresentation parsedShapeRepresentation = GetFirst(parsedProduct.Representation.Representations);
            Assert.Equal("Body", parsedShapeRepresentation.RepresentationIdentifier);
            Assert.Equal("SolidModel", parsedShapeRepresentation.RepresentationType);
            Assert.Equal(1, parsedShapeRepresentation.Items.Count);
            Assert.IsType<IfcExtrudedAreaSolid>(GetFirst(parsedShapeRepresentation.Items));
        }

        private T GetFirst<T>(IEnumerable<T> enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current;
        }

    }
}