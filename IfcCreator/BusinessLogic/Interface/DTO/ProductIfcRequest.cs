using System.ComponentModel.DataAnnotations;
namespace IfcCreator.Interface.DTO
{
    public enum IfcSchema {
        IFC2X3,
        IFC4,
        IFC4X1
    }

    public class ProductIfcRequest
    {
        [Required]
        public Project project {get; private set; }
        [Required]
        public OwnerHistory owner {get; private set; }
        [Required]
        public Product product {get; private set; }
        [Required]
        public IfcSchema schema {get; private set; }

        private ProductIfcRequest()
        {
            this.schema = IfcSchema.IFC2X3;
        }

        public class Builder
        {
            private ProductIfcRequest request = new ProductIfcRequest();

            public ProductIfcRequest Build()
            {
                return this.request;
            }

            public Builder withProject(Project project)
            {
                this.request.project = project;
                return this;
            }

            public Builder withOwner(OwnerHistory owner)
            {
                this.request.owner = owner;
                return this;
            }

            public Builder withProduct(Product product)
            {
                this.request.product = product;
                return this;
            }

            public Builder withSchema(IfcSchema schema)
            {
                this.request.schema = schema;
                return this;
            }

        }
    }
}