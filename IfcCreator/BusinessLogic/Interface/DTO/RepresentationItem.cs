namespace IfcCreator.Interface.DTO
{
    public class RepresentationItem
    {
        public string constructionString {get; set; }

        public Transformation transformation {get; set; }

        private RepresentationItem()
        {}

        public class Builder
        {
            private RepresentationItem representationItem = new RepresentationItem();

            public RepresentationItem Build()
            {
                return this.representationItem;
            }

            public Builder withConstructionString(string constructionString) {
                this.representationItem.constructionString = constructionString;
                return this;
            }

            public Builder withTransformation(Transformation transformation) {
                this.representationItem.transformation = transformation;
                return this;
            }
        
        }

    }
}