using System.ComponentModel.DataAnnotations;

namespace IfcCreator.Interface.DTO
{
    public class ColorRGBa
    {
        [Range(0,1)]
        public double red {get; set; } = 1.0;
        [Range(0,1)]
        public double green {get; set; } = 1.0;
        [Range(0,1)]
        public double blue {get; set; } = 1.0;
        [Range(0,1)]
        public double alpha {get; set; } = 1.0;

        public ColorRGBa(double red,
                         double green,
                         double blue,
                         double alpha)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
        }
    }
}