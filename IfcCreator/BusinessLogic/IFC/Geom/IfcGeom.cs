using System;
using System.Collections.Generic;

using BuildingSmart.IFC.IfcGeometryResource;

namespace IfcCreator.Ifc.Geom
{
#nullable enable
    public static class IfcGeom
    {
        public static IfcPolyline CreatePolyLine(IEnumerable<double[]> curve)
        {
            List<IfcCartesianPoint> pointList = new List<IfcCartesianPoint>();
            foreach(double[] point in curve)
            {
                if (point.Length == 2)
                {
                    pointList.Add(new IfcCartesianPoint(point[0], point[1]));
                }
                if (point.Length == 3)
                {
                    pointList.Add(new IfcCartesianPoint(point[0], point[1], point[2]));
                }
                if ((point.Length < 2) || (point.Length > 3))
                {
                    throw new ArgumentException(string.Format("Cartesion point definition has invalid dimension {0}", point.Length));
                }
            }
            return new IfcPolyline(pointList.ToArray());
        }

        public static IfcPlane CreatePlane(double[] point,
                                           double[] normal)
        {
            //find a ref direction perpendicular to the plane normal
            //dot product xn*xr + yn*yr + zn*zr = 0
            double xr = normal[0] + 1;
            double yr = normal[1] - 1;
            double zr = -1 * (normal[0]*xr + normal[1]*yr) / normal[2];
            return new IfcPlane(new IfcAxis2Placement3D(new IfcCartesianPoint(point[0],
                                                                              point[1],
                                                                              point[2]),
                                                        new IfcDirection(normal[0],
                                                                         normal[1],
                                                                         normal[2]),
                                                        new IfcDirection(xr, yr, zr)));
        }

    }
}