using System.Collections.Generic;
using System.IO;
using Xunit;

using BuildingSmart.IFC.IfcProductExtension;
using BuildingSmart.IFC.IfcUtilityResource;
using BuildingSmart.IFC.IfcGeometricModelResource;
using BuildingSmart.IFC.IfcKernel;
using BuildingSmart.IFC.IfcRepresentationResource;
using BuildingSmart.IFC.IfcPresentationAppearanceResource;
using BuildingSmart.Serialization;
using BuildingSmart.Serialization.Step;

using IfcCreator.Interface;
using IfcCreator.Interface.DTO;
using IfcCreator.Ifc.Geom;

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
                                     .build();
            string personGivenName = "Cedric";
            string personFamilyName = "Mélange";
            string personIdentifier = "cmela";
            Person person = 
                new Person.Builder().withGivenName(personGivenName)
                                    .withFamilyName(personFamilyName)
                                    .withIdentifier(personIdentifier)
                                    .build();
            string organizationName = "EC Life";
            string organizationDescription = "EC Life - for an easy, economical and ecological life";
            string organizationIdentifier = "ECL";
            Organization organization =
                new Organization.Builder().withName(organizationName)
                                          .withDescription(organizationDescription)
                                          .withIdentifier(organizationIdentifier)
                                          .build();
            string applicationName = "ECL IfcCreator";
            string applicationVersion = "1.0";
            string applicationIdentifier = "IfcCreator";
            Application application =
                new Application.Builder().withName(applicationName)
                                         .withVersion(applicationVersion)
                                         .withIdentifier(applicationIdentifier)
                                         .withOrganization(new Organization.Builder().build())
                                         .build();
            OwnerHistory owner = 
                new OwnerHistory.Builder().withPerson(person)
                                          .withOrganization(organization)
                                          .withApllication(application)
                                          .build();

            string productName = "test product";
            string productDescription = "product for test purposes";
            string constructionString = "SHAPE({POLYLINE2D([[0,0],[0,1],[1,1],[1,0]])}).EXTRUDE(1)";
            var material0 = new Material.Builder().withName("material0")
                                                  .withColor(new ColorRGBa(1,0,0,1))
                                                  .withRoughness(0)
                                                  .isMetal()
                                                  .build();
            var material1 = new Material.Builder().withName("material1")
                                                  .withColor(new ColorRGBa(0,0,1,1))
                                                  .withRoughness(1)
                                                  .isDielectric()
                                                  .build();
            RepresentationItem representationItem0 =
                new RepresentationItem.Builder()
                                      .withConstructionString(constructionString)
                                      .withMaterial(material0.id)
                                      .build();
            var rotationQ = new Quaternion();
            rotationQ.SetFromEuler(new double[] {90, 90, 90} );
            RepresentationItem representationItem1 =
                new RepresentationItem.Builder()
                                      .withConstructionString(constructionString)
                                      .withTransformation(new Transformation.Builder()
                                                                            .withTranslation(new double[] {2,0,0})
                                                                            .withRotation(rotationQ.ToArray())
                                                                            .build())
                                      .withMaterial(material1.id)
                                      .build();
            Representation representation = 
                new Representation.Builder().AddRepresentationItem(representationItem0)
                                            .AddRepresentationItem(representationItem1)
                                            .addMaterial(material0)
                                            .addMaterial(material1)
                                            .build();
            Product product =
                new Product.Builder().withName(productName)
                                     .withDescription(productDescription)
                                     .withType(ProductType.PROXY)
                                     .addRepresenation(representation)
                                     .build();
            IfcSchema schema = IfcSchema.IFC2X3;
            ProductIfcRequest request = 
                new ProductIfcRequest.Builder().withProject(project)
                                               .withOwner(owner)
                                               .withProduct(product)
                                               .withSchema(schema)
                                               .build();

            // === convert to IFC stream
            MemoryStream memStream = new MemoryStream();
            IProductIfcCreator productIfcCreator = new ProductIfcCreator();
            productIfcCreator.CreateProductIfc(request, memStream);

            // === write to IFC file
            using (FileStream fileStream = File.Create("./product_ifc_creator_test.ifc"))
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
            IfcSite parsedSite = (IfcSite) GetItem(GetItem(parsedProject.IsDecomposedBy,0).RelatedObjects,0);
            Assert.Equal(personGivenName, parsedSite.OwnerHistory.OwningUser.ThePerson.GivenName);
            IfcBuilding parsedBuilding = (IfcBuilding) GetItem(GetItem(parsedSite.IsDecomposedBy,0).RelatedObjects,0);
            Assert.Equal(personGivenName, parsedBuilding.OwnerHistory.OwningUser.ThePerson.GivenName);
            IfcBuildingStorey parsedStorey = (IfcBuildingStorey) GetItem(GetItem(parsedBuilding.IsDecomposedBy,0).RelatedObjects,0);
            Assert.Equal(personGivenName, parsedStorey.OwnerHistory.OwningUser.ThePerson.GivenName);
            IfcProduct parsedProduct = (IfcProduct) GetItem(GetItem(parsedStorey.ContainsElements,0).RelatedElements,0);
            Assert.Equal(personGivenName, parsedProduct.OwnerHistory.OwningUser.ThePerson.GivenName);
            Assert.Equal(productName, parsedProduct.Name);
            Assert.Equal(productDescription, parsedProduct.Description);
            Assert.Equal(1, parsedProduct.Representation.Representations.Count);
            IfcRepresentation parsedShapeRepresentation = GetItem(parsedProduct.Representation.Representations,0);
            Assert.Equal("Body", parsedShapeRepresentation.RepresentationIdentifier);
            Assert.Equal("SolidModel", parsedShapeRepresentation.RepresentationType);
            Assert.Equal(2, parsedShapeRepresentation.Items.Count);
            Assert.IsType<IfcExtrudedAreaSolid>(GetItem(parsedShapeRepresentation.Items,0));
            Assert.IsType<IfcExtrudedAreaSolid>(GetItem(parsedShapeRepresentation.Items,1));
            var parsedItem0 = (IfcExtrudedAreaSolid) GetItem(parsedShapeRepresentation.Items,0);
            Assert.Equal(1, parsedItem0.StyledByItem.Count);
            var StyledItem0 = GetItem(parsedItem0.StyledByItem, 0);
            Assert.Equal("material0", ((IfcSurfaceStyle) GetItem(StyledItem0.Styles, 0)).Name);
            var parsedItem1 = (IfcExtrudedAreaSolid) GetItem(parsedShapeRepresentation.Items,1);
            Assert.Equal(2, parsedItem1.Position.Location.Coordinates[0].Value);
            Assert.Equal(0, parsedItem1.Position.Location.Coordinates[1].Value);
            Assert.Equal(0, parsedItem1.Position.Location.Coordinates[2].Value);
            Assert.Equal(1, parsedItem1.Position.Axis.DirectionRatios[0].Value, 5);
            Assert.Equal(0, parsedItem1.Position.Axis.DirectionRatios[1].Value, 5);
            Assert.Equal(0, parsedItem1.Position.Axis.DirectionRatios[2].Value, 5);
            Assert.Equal(0, parsedItem1.Position.RefDirection.DirectionRatios[0].Value, 5);
            Assert.Equal(0, parsedItem1.Position.RefDirection.DirectionRatios[1].Value, 5);
            Assert.Equal(1, parsedItem1.Position.RefDirection.DirectionRatios[2].Value, 5);
            Assert.Equal(1, parsedItem0.StyledByItem.Count);
            var StyledItem1 = GetItem(parsedItem1.StyledByItem, 0);
            Assert.Equal("material1", ((IfcSurfaceStyle) GetItem(StyledItem1.Styles, 0)).Name);
        }

        private T GetItem<T>(IEnumerable<T> enumerable, int index)
        {
            var enumerator = enumerable.GetEnumerator();
            for (int i=0; i <= index; ++i)
            {
                enumerator.MoveNext();
            }
            return enumerator.Current;
        }

    }
}