using System;
using System.Collections.Generic;

using BuildingSmart.IFC.IfcGeometryResource;

namespace IfcCreator.Ifc.Geom
{
#nullable enable
    public static class IfcGeom
    {
        public static IfcPolyline CreatePolyLine(IfcCartesianPoint[] outerCurve)
        {
            List<IfcCartesianPoint> outerPointList = new List<IfcCartesianPoint>();
            foreach(IfcCartesianPoint point in outerCurve)
            {
                outerPointList.Add(point);
            }
            return new IfcPolyline(outerPointList.ToArray());
        }

    }
}