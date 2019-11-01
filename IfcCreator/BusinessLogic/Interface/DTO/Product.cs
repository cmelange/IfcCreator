using System.Collections.Generic;

namespace IfcCreator.Interface.DTO
{
    public enum ProductType
    {
        PROXY
    }
    public class Product
    {
        public ProductType type {get; protected set; }
        public string name {get; protected set; }
        public string description {get; protected set; }
        public List<Representation> represenations {get; protected set; }

        private Product()
        {
            this.type = ProductType.PROXY;
            this.name = "";
            this.description = "";
            this.represenations = new List<Representation>();
        }

        public class Builder: AbstractProductBuilder<Product>
        {
            public Builder()
            {
                this.product = new Product();
            }
        }

        public abstract class AbstractProductBuilder<T> where T: Product
        {
            protected T product;

            public T Build()
            {
                return this.product;
            }
            public AbstractProductBuilder<T> withType(ProductType type)
            {
                this.product.type = type;
                return this;
            }

            public AbstractProductBuilder<T> withName(string name)
            {
                this.product.name = name;
                return this;
            }

            public AbstractProductBuilder<T> withDescription(string description)
            {
                this.product.description = description;
                return this;
            }

            public AbstractProductBuilder<T> addRepresenation(Representation representation)
            {
                this.product.represenations.Add(representation);
                return this;
            }

            public AbstractProductBuilder<T> addRepresenations(IEnumerable<Representation> representations)
            {
                foreach(Representation representation in representations)
                {
                    this.product.represenations.Add(representation);
                }
                return this;
            }

        }
    }
}