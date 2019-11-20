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

        /**
         *  calculates the Euler intrinsic rotation matrix in the order XY'Z" (Tait-Bryan angles)
         */
        private static double[][] RotationMatrix3D(double[] rotation)
        {
            double[] theta = new double[] {ToRadians(rotation[0]), ToRadians(rotation[1]), ToRadians(rotation[2])};

            double c1 = Math.Cos(theta[0]);
            double s1 = Math.Sin(theta[0]);
            double c2 = Math.Cos(theta[1]);
            double s2 = Math.Sin(theta[1]);
            double c3 = Math.Cos(theta[2]);
            double s3 = Math.Sin(theta[2]);


            return new double[][] { new double[] {c2*c3, -c2*s3, s2},
                                    new double[] {c1*s3 + c3*s1*s2, c1*c3 - s1*s2*s3, -c2*s1},
                                    new double[] {s1*s3 - c1*c3*s2, c3*s1 + c1*s2*s3, c1*c2}};
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

        private static IfcDirection ApplyQuaternion(this IfcDirection direction,
                                                    Quaternion q)
        {
            var directionQ = new Quaternion(0,
                                            direction.DirectionRatios[0].Value,
                                            direction.DirectionRatios[1].Value,
                                            direction.DirectionRatios[2].Value);
            Quaternion result = q.Multiply(directionQ.Multiply(q.Conjugate()));
            return new IfcDirection(result.x, result.y, result.z);
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

        private static IfcCartesianPoint ApplyQuaternion(this IfcCartesianPoint location,
                                                         Quaternion q)
        {
            var pointQ = new Quaternion(0,
                                        location.Coordinates[0].Value,
                                        location.Coordinates[1].Value,
                                        location.Coordinates[2].Value);
            Quaternion result = q.Multiply(pointQ.Multiply(q.Conjugate()));
            return new IfcCartesianPoint(result.x, result.y, result.z);
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

        public static IfcAxis2Placement3D ApplyQuaternion(this IfcAxis2Placement3D placement3D,
                                                         Quaternion q)
        {
            IfcDirection refDirection = placement3D.RefDirection ?? new IfcDirection(1,0,0);
            IfcDirection axis = placement3D.Axis ?? new IfcDirection(0,0,1);
            placement3D.RefDirection = refDirection.ApplyQuaternion(q);
            placement3D.Axis = axis.ApplyQuaternion(q);
            placement3D.Location = placement3D.Location.ApplyQuaternion(q);
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

        public static IfcRepresentationItem ApplyQuaternion(this IfcRepresentationItem representation,
                                                            Quaternion q)
        {
            if (representation is IfcBooleanResult)
            {
                ((IfcBooleanResult) representation).ApplyQuaternion(q);
            }
            if (representation is IfcSweptAreaSolid)
            {
                ((IfcSweptAreaSolid) representation).ApplyQuaternion(q);
            }
            return representation;
        }

    }
}