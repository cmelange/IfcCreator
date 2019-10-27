using System.Collections.Generic;
using System.IO;
using Xunit;

using BuildingSmart.IFC.IfcKernel;
using BuildingSmart.IFC.IfcProductExtension;
using BuildingSmart.IFC.IfcRepresentationResource;
using BuildingSmart.IFC.IfcMeasureResource;
using BuildingSmart.IFC.IfcGeometryResource;
using BuildingSmart.IFC.IfcGeometricModelResource;
using BuildingSmart.IFC.IfcProfileResource;

namespace IfcCreator.Ifc.Geom
{
    public class IfcConstructiveSolidTest
    {
        [Fact]
        public void BooleanOperatorTest()
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
            IfcCsgPrimitive3D union_first = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(0,0,0), null, null),
                                                         new IfcPositiveLengthMeasure(1),
                                                         new IfcPositiveLengthMeasure(1),
                                                         new IfcPositiveLengthMeasure(1));
            IfcCsgPrimitive3D union_second = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(0.5,0.5,0.5), null, null),
                                                          new IfcPositiveLengthMeasure(1),
                                                          new IfcPositiveLengthMeasure(1),
                                                          new IfcPositiveLengthMeasure(1));
            IfcRepresentationItem unionRepresentation = union_first.Union(union_second);

            IfcCsgPrimitive3D diff_first = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(2,0,0), null, null),
                                                        new IfcPositiveLengthMeasure(1),
                                                        new IfcPositiveLengthMeasure(1),
                                                        new IfcPositiveLengthMeasure(1));
            IfcCsgPrimitive3D diff_second = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(2.5,0.5,0.5), null, null),
                                                         new IfcPositiveLengthMeasure(1),
                                                         new IfcPositiveLengthMeasure(1),
                                                         new IfcPositiveLengthMeasure(1));
            IfcRepresentationItem diffRepresentation = diff_first.Difference(diff_second);

            IfcCsgPrimitive3D inter_first = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(4,0,0), null, null),
                                                         new IfcPositiveLengthMeasure(1),
                                                         new IfcPositiveLengthMeasure(1),
                                                         new IfcPositiveLengthMeasure(1));
            IfcCsgPrimitive3D inter_second = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(4.5,0.5,0.5), null, null),
                                                          new IfcPositiveLengthMeasure(1),
                                                          new IfcPositiveLengthMeasure(1),
                                                          new IfcPositiveLengthMeasure(1));
            IfcRepresentationItem interRepresentation = inter_first.Intersection(inter_second);
            IfcPolyline outerCurve = IfcGeom.CreatePolyLine(new List<double[]>() { new double[] {6, 0},
                                                                                     new double[] {6, 1},
                                                                                     new double[] {7, 1},
                                                                                     new double[] {7, 0},
                                                                                     new double[] {6, 0}});
            IfcProfileDef profileDef = new IfcArbitraryClosedProfileDef(IfcProfileTypeEnum.AREA,
                                                                        null,
                                                                        outerCurve);
            IfcSweptAreaSolid cut_reference = profileDef.Extrude(1);
            IfcPlane plane = IfcGeom.CreatePlane(new double[] {6.5, 0.5, 0},
                                                 new double[] {1,1,0});
            IfcRepresentationItem cutRepresentation = cut_reference.ClipByPlane(plane);

            //Create product with representation and place in storey
            var contextEnum = project.RepresentationContexts.GetEnumerator();
            contextEnum.MoveNext(); 
            IfcShapeRepresentation shapeRepresentation = 
                new IfcShapeRepresentation(contextEnum.Current,
                                           new IfcLabel("union shape"),
                                           new IfcLabel("BooleanResult"),
                                           new IfcRepresentationItem[] {unionRepresentation, diffRepresentation, interRepresentation, cutRepresentation});
            IfcProxy product = IfcInit.CreateProxy(null, null, null, storey.ObjectPlacement, null);
            product.Representation = new IfcProductDefinitionShape(null, null, new IfcRepresentation[] {shapeRepresentation});
            storey.Contains(product, null);
            
            //Write to IFC file
            using (FileStream fs = File.Create("./constructive_geom_test.ifc"))
            {
               project.SerializeToStep(fs, IFC_SCHEMA.IFC2X3, null);
            }
        }
    }
}