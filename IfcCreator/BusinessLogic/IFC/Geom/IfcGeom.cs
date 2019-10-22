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

    }
}