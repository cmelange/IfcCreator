using System.ComponentModel.DataAnnotations;

namespace IfcCreator.Interface.DTO
{
    public class Transformation
    {
        [MinLength(2)]
        [MaxLength(3)]
        public double[] translation {get; set; }
        [MinLength(2)]
        [MaxLength(3)]
        public double[] rotation {get; set; }
        [Range(double.Epsilon, double.MaxValue)]
        public double scale;

        private Transformation()
        {
            this.translation = new double[] {0,0,0};
            this.rotation = new double[] {0,0,0};
            this.scale = 1.0;
        }

        public class Builder
        {
            private Transformation transformation = new Transformation();

            public Transformation Build()
            {
                return this.transformation;
            }

            public Builder withTranslation(double[] translation)
            {
                this.transformation.translation = translation;
                return this;
            }

            public Builder withRotation(double[] rotation)
            {
                this.transformation.rotation = rotation;
                return this;
            }

            public Builder withScale(double scale)
            {
                this.transformation.scale = scale;
                return this;
            }
        }

    }
}