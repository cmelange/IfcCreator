namespace IfcCreator.Interface.DTO
{
    public class Organization
    {
        public string name {get; private set; }
        public string description {get; private set; }
        public string identifier {get; private set; }

        private Organization() {
            this.name = "";
            this.description = "";
            this.identifier = "";
        }

        public class Builder
        {
            private Organization organization = new Organization();

            public Organization Build()
            {
                return this.organization;
            }

            public Builder withName(string name)
            {
                this.organization.name = name;
                return this;
            }

            public Builder withDescription(string description)
            {
                this.organization.description = description;
                return this;
            }

            public Builder withIdentifier(string identifier)
            {
                this.organization.identifier = identifier;
                return this;
            }
        }

    }
}