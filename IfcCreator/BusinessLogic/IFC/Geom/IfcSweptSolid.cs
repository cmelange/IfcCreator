using System;

using BuildingSmart.IFC.IfcGeometricModelResource;
using BuildingSmart.IFC.IfcMeasureResource;
using BuildingSmart.IFC.IfcGeometryResource;
using BuildingSmart.IFC.IfcProfileResource;

namespace IfcCreator.Ifc.Geom
{
#nullable enable
    public static class IfcSweptSolid
    {
        public static IfcExtrudedAreaSolid Extrude(this IfcProfileDef sweptArea,
                                                   double height)
        {
            return sweptArea.Extrude(height, null, null);
        }

        public static IfcExtrudedAreaSolid Extrude(this IfcProfileDef sweptArea,
                                                   double height,
                                                   IfcDirection? direction,
                                                   IfcAxis2Placement3D? position)
        {
            return new IfcExtrudedAreaSolid(sweptArea,
                                            position ?? IfcInit.CreateIfcAxis2Placement3D(),
                                            direction ?? new IfcDirection(0,0,1),
                                            new IfcPositiveLengthMeasure(height));
        }

        public static IfcRevolvedAreaSolid Revolve(this IfcProfileDef sweptArea,
                                                   double rotation)
        {
            return sweptArea.Revolve(rotation, null, null);
        }

        public static IfcRevolvedAreaSolid Revolve(this IfcProfileDef sweptArea,
                                                   double rotation,
                                                   IfcAxis1Placement? axis,
                                                   IfcAxis2Placement3D? position)
        {
            return new IfcRevolvedAreaSolid(sweptArea,
                                            position ?? IfcInit.CreateIfcAxis2Placement3D(),
                                            axis ?? new IfcAxis1Placement(new IfcCartesianPoint(0,0,0),
                                                                          new IfcDirection(0,1,0)),
                                            new IfcPlaneAngleMeasure(rotation));
        }

        public static IfcSweptAreaSolid Translate(this IfcSweptAreaSolid representation,
                                                  double[] translation)
        {
            representation.Position.Translate(translation);
            return representation;
        }

        public static IfcSweptAreaSolid Rotate(this IfcSweptAreaSolid representation,
                                               double[] rotation)
        {
            representation.Position.Rotate(rotation);
            return representation;
        }
    }
}