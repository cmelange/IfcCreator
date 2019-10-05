using System.IO;
using Xunit;

using BuildingSmart.IFC.IfcKernel;
using BuildingSmart.IFC.IfcProductExtension;
using BuildingSmart.IFC.IfcRepresentationResource;
using BuildingSmart.IFC.IfcMeasureResource;
using BuildingSmart.IFC.IfcGeometryResource;
using BuildingSmart.IFC.IfcProfileResource;

namespace IfcCreator.Ifc.Geom
{
    public class IfcSweptSolidTest
    {
        [Fact]
        public void ExtrudeAndRevolveTest()
        {
            //Set up project hierarchy
            IfcProject project = IfcInit.CreateProject(null, null, null);
            IfcSite site = IfcInit.CreateSite(null, null, null);
            project.Aggregate(site, null);
            IfcBuilding building = IfcInit.CreateBuilding(null, null, null, null);
            site.Aggregate(building, null);
            IfcBuildingStorey storey = IfcInit.CreateBuildingStorey(null, null, null, null);
            building.Aggregate(storey, null);

            //Create shape representation
            // -- extruded profile shape
            IfcPolyline outerCurve = IfcGeom.CreatePolyLine(new IfcCartesianPoint[] {new IfcCartesianPoint(-0.5, -0.5),
                                                                                     new IfcCartesianPoint(-0.5, 0.5),
                                                                                     new IfcCartesianPoint(0.5, 0.5),
                                                                                     new IfcCartesianPoint(0.5, -0.5),
                                                                                     new IfcCartesianPoint(-0.5, -0.5)});
            IfcProfileDef profileDef = new IfcArbitraryClosedProfileDef(IfcProfileTypeEnum.AREA,
                                                                        null,
                                                                        outerCurve);
            IfcRepresentationItem extrudedRepresentation = profileDef.Extrude(1).Rotate(new double[] {45, 0, 45}).Translate(new double[] {-2, 0, 0});

            // -- revolved profile shape
            IfcRepresentationItem revolvedRepresentation = profileDef.Revolve(-45, 
                                                                              new IfcAxis1Placement(new IfcCartesianPoint(1,0,0),
                                                                                                    new IfcDirection(0,1,0)),
                                                                              null);

            //Create product with representation and place in storey
            var contextEnum = project.RepresentationContexts.GetEnumerator();
            contextEnum.MoveNext(); 
            IfcShapeRepresentation shapeRepresentation = 
                new IfcShapeRepresentation(contextEnum.Current,
                                           new IfcLabel("extruded square"),
                                           new IfcLabel("SweptSolid"),
                                           new IfcRepresentationItem[] {extrudedRepresentation, revolvedRepresentation});
            IfcProxy product = IfcInit.CreateProxy(null, null, null, storey.ObjectPlacement, null);
            product.Representation = new IfcProductDefinitionShape(null, null, new IfcRepresentation[] {shapeRepresentation});;
            storey.Contains(product, null);
            
            //Write to IFC file
            using (FileStream fs = File.Create("./swept_geom_test.ifc"))
            {
               project.SerializeToStep(fs, IFC_SCHEMA.IFC2X3, null);
            }
        }
    }
}