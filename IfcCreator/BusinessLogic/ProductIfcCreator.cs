using System;
using System.Collections.Generic;
using System.IO;

using BuildingSmart.IFC.IfcKernel;
using BuildingSmart.IFC.IfcProductExtension;
using BuildingSmart.IFC.IfcActorResource;
using BuildingSmart.IFC.IfcUtilityResource;
using BuildingSmart.IFC.IfcRepresentationResource;
using BuildingSmart.IFC.IfcMeasureResource;
using BuildingSmart.IFC.IfcGeometryResource;

using IfcCreator.Ifc;
using IfcCreator.Ifc.Geom;
using IfcCreator.Interface;
using IfcCreator.Interface.DTO;
using IfcCreator.ExceptionHandling;

namespace IfcCreator
{
    public class ProductIfcCreator : IProductIfcCreator
    {
        public void CreateProductIfc(ProductIfcRequest request,
                                       Stream outputStream)
        {
            IfcPerson person = IfcInit.CreatePerson(request.owner.person.givenName,
                                                    request.owner.person.familyName,
                                                    request.owner.person.identifier);
            IfcOrganization organization = 
                    IfcInit.CreateOrganization(request.owner.organization.name,
                                               request.owner.organization.description,
                                               request.owner.organization.identifier);
            IfcOrganization applicationOrganization =
                IfcInit.CreateOrganization(request.owner.application.organization.name,
                                           request.owner.application.organization.description,
                                           request.owner.application.organization.identifier);
            IfcApplication application =
                    IfcInit.CreateApplication(applicationOrganization,
                                              request.owner.application.version,
                                              request.owner.application.name,
                                              request.owner.application.identifier);
            IfcOwnerHistory ownerHistory = 
                    IfcInit.CreateOwnerHistory(person, organization, application);

            IfcProject project = IfcInit.CreateProject(request.project.name,
                                                       request.project.description,
                                                       ownerHistory);
            IfcSite site = IfcInit.CreateSite(null, null, ownerHistory);
            project.Aggregate(site, ownerHistory);
            IfcBuilding building = 
                    IfcInit.CreateBuilding(null, null, ownerHistory, site.ObjectPlacement);
            site.Aggregate(building, ownerHistory);
            IfcBuildingStorey storey = 
                    IfcInit.CreateBuildingStorey(null, null, ownerHistory, building.ObjectPlacement);
            building.Aggregate(storey, ownerHistory);
            IfcProduct product;
            switch (request.product.type)
            {
                default:
                    product = IfcInit.CreateProxy(request.product.name,
                                                  request.product.description,
                                                  ownerHistory,
                                                  storey.ObjectPlacement,
                                                  null);
                    break;
            }
            List<IfcRepresentationContext> contextList = 
                new List<IfcRepresentationContext> (project.RepresentationContexts);
            product.Representation = 
                CreateProductRepresentation(contextList, request.product.representations);
            storey.Contains(product, ownerHistory);
            project.SerializeToStep(outputStream, request.schema.ToString(), null);
            return;
        }

        private IfcProductRepresentation CreateProductRepresentation(List<IfcRepresentationContext> contextList,
                                                                     IEnumerable<Representation> reperesentationList)
        {
            var shapeRepresenations = new List<IfcShapeRepresentation>();
            foreach(Representation representation in reperesentationList)
            {
                IfcShapeRepresentation shapeRepresentation = 
                    CreateShapeRepresentation(contextList[0], representation);
                shapeRepresenations.Add(shapeRepresentation);
            }
            return new IfcProductDefinitionShape(new IfcLabel(""),
                                                 new IfcText(""),
                                                 shapeRepresenations.ToArray());

        }

        private IfcShapeRepresentation CreateShapeRepresentation(IfcRepresentationContext context,
                                                                 Representation representation)
        {
            var representationItemList = new List<IfcRepresentationItem>();
            foreach(RepresentationItem item in representation.representationItems)
            {
                IfcRepresentationItem representationItem;
                try
                {
                    representationItem = RepresentationParser.ParseConstructionString(item.constructionString);
                }
                catch (ArgumentException ex)
                {
                    var errors = new Dictionary<string, string[]>();
                    errors.Add(ex.ParamName, new string[]{ ex.Message });
                    throw new ValidationException(errors, ex.Message);
                }

                if (item.transformation != null)
                {
                    representationItem.Translate(item.transformation.translation)
                                      .Rotate(item.transformation.rotation);
                }
                
                representationItemList.Add(representationItem);
                
            }
            return new IfcShapeRepresentation(context,
                                              new IfcLabel("Body"),
                                              new IfcLabel("SolidModel"),
                                              representationItemList.ToArray());
        }
    }
}