namespace IfcCreator.Interface.DTO
{
    public class RepresentationItem
    {
        public string constructionString {get; set; }

        public string material {get; set; }
        
        public Transformation transformation {get; set; }

        private RepresentationItem()
        {}

        public class Builder
        {
            private RepresentationItem representationItem = new RepresentationItem();

            public RepresentationItem build()
            {
                return this.representationItem;
            }

            public Builder withConstructionString(string constructionString) {
                this.representationItem.constructionString = constructionString;
                return this;
            }

            public Builder withMaterial(string materialName) {
                this.representationItem.material = materialName;
                return this;
            }

            public Builder withTransformation(Transformation transformation) {
                this.representationItem.transformation = transformation;
                return this;
            }
        
        }

    }
}