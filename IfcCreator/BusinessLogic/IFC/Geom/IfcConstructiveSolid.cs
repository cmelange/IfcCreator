using System;

using BuildingSmart.IFC.IfcGeometricModelResource;
using BuildingSmart.IFC.IfcMeasureResource;
using BuildingSmart.IFC.IfcGeometryResource;

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
                                                           double[] point,
                                                           double[] normal)
        {
            //find a ref direction perpendicular to the plane normal
            //dot product xn*xr + yn*yr + zn*zr = 0
            double xr = normal[0] + 1;
            double yr = normal[1] - 1;
            double zr = -1 * (normal[0]*xr + normal[1]*yr) / normal[2];
            IfcAxis2Placement3D planeDef = new IfcAxis2Placement3D(new IfcCartesianPoint(point[0],
                                                                                         point[1],
                                                                                         point[2]),
                                                                   new IfcDirection(normal[0],
                                                                                    normal[1],
                                                                                    normal[2]),
                                                                   new IfcDirection(xr, yr, zr));
            return new IfcBooleanClippingResult(IfcBooleanOperator.DIFFERENCE,
                                                first,
                                                new IfcHalfSpaceSolid(new IfcPlane(planeDef),
                                                                      new IfcBoolean(false)));
        }
    }
}