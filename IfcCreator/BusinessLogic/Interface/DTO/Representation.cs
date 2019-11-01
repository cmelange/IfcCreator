using System.Collections.Generic;

namespace IfcCreator.Interface.DTO
{
    public class Representation
    {
        public List<RepresentationItem> representationItems {get; private set; }

        private Representation()
        {
            this.representationItems = new List<RepresentationItem>();
        }

        public class Builder
        {
            private Representation representation = new Representation();

            public Representation Build()
            {
                return this.representation;
            }

            public Builder AddRepresentationItem(RepresentationItem item)
            {
                this.representation.representationItems.Add(item);
                return this;
            }

            public Builder AddRepresentationItems(IEnumerable<RepresentationItem> items)
            {
                foreach (RepresentationItem item in items)
                {
                    this.representation.representationItems.Add(item);
                }
                return this;
            }
        }
    }
}