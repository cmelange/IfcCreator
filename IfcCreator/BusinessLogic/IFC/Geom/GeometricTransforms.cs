using System;

using BuildingSmart.IFC.IfcGeometricModelResource;
using BuildingSmart.IFC.IfcGeometryResource;

namespace IfcCreator.Ifc.Geom
{
#nullable enable
    public static class GeometricTransforms
    {
        private static double ToRadians(double degree)
        {
            return degree*Math.PI/180;
        }

        private static double[][] RotationMatrix2D(double rotation)
        {
            double theta = ToRadians(rotation);
            return new double[][]{ new double[] {Math.Cos(theta), -Math.Sin(theta)},
                                   new double[] {Math.Sin(theta), Math.Cos(theta)}};
        }

        private static IfcDirection ApplyMatrix2(this IfcDirection direction,
                                                 double[][] matrix)
        {
            double[] newDirection = {0,0};
            for (int i=0; i<2; ++i)
            {
                for (int j=0; j<2; ++j)
                {
                    newDirection[i] += matrix[i][j]*direction.DirectionRatios[j].Value;
                }
            }
            return new IfcDirection(newDirection[0], newDirection[1]);
        }

        public static IfcAxis1Placement Rotate(this IfcAxis1Placement axis,
                                          double rotation)
        {
            double[][] rotationMatrix = RotationMatrix2D(rotation);
            axis.Axis = axis.Axis.ApplyMatrix2(rotationMatrix);
            return axis;
        }

        private static double[][] RotationMatrix3D(double[] rotation)
        {
            double[] theta = new double[] {ToRadians(rotation[0]), ToRadians(rotation[0]), ToRadians(rotation[0])};

            double cosa = Math.Cos(theta[0]);
            double sina = Math.Sin(theta[0]);
            double cosb = Math.Cos(theta[1]);
            double sinb = Math.Sin(theta[1]);
            double cosc = Math.Cos(theta[2]);
            double sinc = Math.Sin(theta[2]);

            return new double[][] { new double[] {cosb*cosc, -cosb*sinc, sinb},
                                    new double[] {sina*sinb*cosc + cosa*sinc, -sina*sinb*sinc + cosa*cosc, -sina*cosb},
                                    new double[] {-cosa*sinb*cosc + sina*sinc, cosa*sinb*sinc + sina*cosc, cosa*cosb}};
        }

        private static IfcDirection ApplyMatrix3(this IfcDirection direction,
                                                 double[][] matrix)
        {
            double[] newDirection = {0,0,0};
            for (int i=0; i<3; ++i)
            {
                for (int j=0; j<3; ++j)
                {
                    newDirection[i] += matrix[i][j]*direction.DirectionRatios[j].Value;
                }
            }
            return new IfcDirection(newDirection[0], newDirection[1], newDirection[2]);
        }

        private static IfcCartesianPoint ApplyMatrix3(this IfcCartesianPoint location,
                                                 double[][] matrix)
        {
            double[] newPoint = {0,0,0};
            for (int i=0; i<3; ++i)
            {
                for (int j=0; j<3; ++j)
                {
                    newPoint[i] += matrix[i][j]*location.Coordinates[j].Value;
                }
            }
            return new IfcCartesianPoint(newPoint[0], newPoint[1], newPoint[2]);
        }

        public static IfcAxis2Placement3D Rotate(this IfcAxis2Placement3D placement3D,
                                                 double[] rotation)
        {
            double[][] rotationMatrix = RotationMatrix3D(rotation);
            IfcDirection refDirection = placement3D.RefDirection ?? new IfcDirection(1,0,0);
            IfcDirection axis = placement3D.Axis ?? new IfcDirection(0,0,1);
            placement3D.RefDirection = refDirection.ApplyMatrix3(rotationMatrix);
            placement3D.Axis = axis.ApplyMatrix3(rotationMatrix);
            placement3D.Location = placement3D.Location.ApplyMatrix3(rotationMatrix);
            return placement3D;
        }

        public static IfcAxis2Placement3D Translate(this IfcAxis2Placement3D placement3D,
                                                    double[] translation)
        {
            IfcCartesianPoint newPosition = 
                new IfcCartesianPoint(placement3D.Location.Coordinates[0].Value + translation[0],
                                      placement3D.Location.Coordinates[1].Value + translation[1],
                                      placement3D.Location.Coordinates[2].Value + translation[2]);
            placement3D.Location = newPosition;
            return placement3D;
        }

        public static IfcRepresentationItem Translate(this IfcRepresentationItem representation,
                                                      double[] translation)
        {
            if (representation is IfcBooleanResult)
            {
                ((IfcBooleanResult) representation).Translate(translation);
            }
            if (representation is IfcSweptAreaSolid)
            {
                ((IfcSweptAreaSolid) representation).Translate(translation);
            }
            return representation;
        }

        public static IfcRepresentationItem Rotate(this IfcRepresentationItem representation,
                                                   double[] rotation)
        {
            if (representation is IfcBooleanResult)
            {
                ((IfcBooleanResult) representation).Rotate(rotation);
            }
            if (representation is IfcSweptAreaSolid)
            {
                ((IfcSweptAreaSolid) representation).Rotate(rotation);
            }
            return representation;
        }
    }
}