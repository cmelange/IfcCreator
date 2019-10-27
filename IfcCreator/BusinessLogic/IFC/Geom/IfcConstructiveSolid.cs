using BuildingSmart.IFC.IfcGeometricModelResource;
using BuildingSmart.IFC.IfcMeasureResource;
using BuildingSmart.IFC.IfcGeometryResource;

using IfcCreator.Ifc.Geom;

namespace IfcCreator.Ifc.Geom
{
#nullable enable
    public static class IfcConstructiveSolid
    {
        public static IfcBooleanResult Union(this IfcBooleanOperand first, IfcBooleanOperand second)
        {
            return new IfcBooleanResult(IfcBooleanOperator.UNION, first, second);
        }

        public static IfcBooleanResult Difference(this IfcBooleanOperand first, IfcBooleanOperand second)
        {
            return new IfcBooleanResult(IfcBooleanOperator.DIFFERENCE, first, second);
        }

        public static IfcBooleanResult Intersection(this IfcBooleanOperand first, IfcBooleanOperand second)
        {
            return new IfcBooleanResult(IfcBooleanOperator.INTERSECTION, first, second);
        }

        public static IfcBooleanClippingResult ClipByPlane(this IfcSweptAreaSolid first,
                                                           IfcPlane plane)
        {
            return ClipByPlaneBase(first, plane);
        }

        public static IfcBooleanClippingResult ClipByPlane(this IfcBooleanClippingResult first,
                                                           IfcPlane plane)
        {
            return ClipByPlaneBase(first, plane);
        }

        private static IfcBooleanClippingResult ClipByPlaneBase(this IfcBooleanOperand first,
                                                                IfcPlane plane)
        {
            
            return new IfcBooleanClippingResult(IfcBooleanOperator.DIFFERENCE,
                                                first,
                                                new IfcHalfSpaceSolid(plane,
                                                                      new IfcBoolean(false)));
        }

        public static IfcBooleanResult Translate(this IfcBooleanResult representation,
                                                 double[] translation)
        {   //An IfcBooleanResult is translated by recursively translating every operand

            // === first operand
            if (representation.FirstOperand is IfcSweptAreaSolid)
            {
                ((IfcSweptAreaSolid) representation.FirstOperand).Translate(translation);
            }
            if (representation.FirstOperand is IfcBooleanResult)
            {
                ((IfcBooleanResult) representation.FirstOperand).Translate(translation);
            }
            if (representation.FirstOperand is IfcCsgPrimitive3D)
            {
                ((IfcCsgPrimitive3D) representation.FirstOperand).Position.Translate(translation);
            }

            // === second operand
            if (representation.SecondOperand is IfcSweptAreaSolid)
            {
                ((IfcSweptAreaSolid) representation.SecondOperand).Translate(translation);
            }
            if (representation.SecondOperand is IfcBooleanResult)
            {
                ((IfcBooleanResult) representation.SecondOperand).Translate(translation);
            }
            if (representation.SecondOperand is IfcCsgPrimitive3D)
            {
                ((IfcCsgPrimitive3D) representation.SecondOperand).Position.Translate(translation);
            }
            if (representation.SecondOperand is IfcHalfSpaceSolid)
            {
                ((IfcPlane) ((IfcHalfSpaceSolid) representation.SecondOperand).BaseSurface)
                    .Position.Translate(translation);
            }
            return representation;
        }

        public static IfcBooleanResult Rotate(this IfcBooleanResult representation,
                                              double[] rotation)
        {   //An IfcBooleanResult is rotated by recursively rotating every operand

            // === first operand
            if (representation.FirstOperand is IfcSweptAreaSolid)
            {
                ((IfcSweptAreaSolid) representation.FirstOperand).Rotate(rotation);
            }
            if (representation.FirstOperand is IfcBooleanResult)
            {
                ((IfcBooleanResult) representation.FirstOperand).Rotate(rotation);
            }
            if (representation.FirstOperand is IfcCsgPrimitive3D)
            {
                ((IfcCsgPrimitive3D) representation.FirstOperand).Position.Rotate(rotation);
            }

            // === second operand
            if (representation.SecondOperand is IfcSweptAreaSolid)
            {
                ((IfcSweptAreaSolid) representation.SecondOperand).Rotate(rotation);
            }
            if (representation.SecondOperand is IfcBooleanResult)
            {
                ((IfcBooleanResult) representation.SecondOperand).Rotate(rotation);
            }
            if (representation.SecondOperand is IfcCsgPrimitive3D)
            {
                ((IfcCsgPrimitive3D) representation.SecondOperand).Position.Rotate(rotation);
            }
            if (representation.SecondOperand is IfcHalfSpaceSolid)
            {
                ((IfcPlane) ((IfcHalfSpaceSolid) representation.SecondOperand).BaseSurface)
                    .Position.Rotate(rotation);
            }
            return representation;
        }
    }
}