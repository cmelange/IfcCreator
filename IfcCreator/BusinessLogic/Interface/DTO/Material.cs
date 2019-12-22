using System;
using System.ComponentModel.DataAnnotations;

namespace IfcCreator.Interface.DTO
{
    public class Material
    {
        [Required]
        public string id {get; set; }
        public string name {get; set; }
        public ColorRGBa color {get; set; }
        public bool metal {get; set; }
        public double roughness {get; set; }

        private Material()
        {
            this.id = Guid.NewGuid().ToString();
            this.name = "default material";
            this.color = new ColorRGBa(1.0, 1.0, 1.0, 1.0);
            this.metal = false;
            this.roughness = 0.5;
        } 

        public class Builder
        {
            private Material material = new Material();
            
            public Material build()
            {
                return this.material;
            }

            public Builder withName(string name)
            {
                this.material.name = name;
                return this;
            }

            public Builder withColor(ColorRGBa color)
            {
                this.material.color = color;
                return this;
            }

            public Builder isMetal()
            {
                this.material.metal = true;
                return this;
            }

            public Builder isDielectric()
            {
                this.material.metal = false;
                return this;
            }

            public Builder withRoughness(double roughness)
            {
                this.material.roughness = roughness;
                return this;
            }
            
        }


    }
}