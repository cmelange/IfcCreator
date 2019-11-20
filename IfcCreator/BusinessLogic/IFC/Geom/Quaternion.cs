using System;

namespace IfcCreator.Ifc.Geom
{
    public class Quaternion
    {
        public double w {get; set; }
        public double x {get; set; }
        public double y {get; set; }
        public double z {get; set; }

        public Quaternion()
        {
            this.w = 1;
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public Quaternion(double w, double x, double y, double z)
        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Quaternion(double[] quaternion) {
            this.w = quaternion[0];
            this.x = quaternion[1];
            this.y = quaternion[2];
            this.z = quaternion[3];
        }

        public Quaternion(Quaternion quaternion) {
            this.w = quaternion.w;
            this.x = quaternion.x;
            this.y = quaternion.y;
            this.z = quaternion.z;
        }

        private static double ToRadians(double degree)
        {
            return degree*Math.PI/180;
        }

        /**
        * As convention we use Euler intrinsic rotation in the order XYZ (Tait-Bryan xy'z'')
        * @param rotation euler angles around X,Y and Z angles respectively
        */
        public void SetFromEuler(double[] rotation) {
            double[] theta = new double[] {ToRadians(rotation[0]), ToRadians(rotation[1]), ToRadians(rotation[2])};
            double c1 = Math.Cos(theta[0] * 0.5);
            double s1 = Math.Sin(theta[0] * 0.5);
            double c2 = Math.Cos(theta[1] * 0.5);
            double s2 = Math.Sin(theta[1] * 0.5);
            double c3 = Math.Cos(theta[2] * 0.5);
            double s3 = Math.Sin(theta[2] * 0.5);

            var qx = new Quaternion(c1, s1, 0, 0);
            var qy = new Quaternion(c2, 0, s2, 0);
            var qz = new Quaternion(c3, 0, 0, s3);
            
            Quaternion result = (qx.Multiply(qy.Multiply(qz)));
            
            this.w = result.w;
            this.x = result.x;
            this.y = result.y;
            this.z = result.z;

            return;
        }

        public Quaternion Multiply(Quaternion q) {
            var result = new Quaternion();
            result.w = this.w*q.w - this.x*q.x - this.y*q.y - this.z*q.z;
            result.x = this.w*q.x + this.x*q.w + this.y*q.z - this.z*q.y;
            result.y = this.w*q.y + this.y*q.w + this.z*q.x - this.x*q.z;
            result.z = this.w*q.z + this.z*q.w + this.x*q.y - this.y*q.x;
            return result;
        }

        public Quaternion Conjugate() {
            return new Quaternion(this.w, -this.x, -this.y, -this.z);
        }

        public double[] ToArray() {
            return new double[] {this.w, this.x, this.y, this.z};
        }

    }
}