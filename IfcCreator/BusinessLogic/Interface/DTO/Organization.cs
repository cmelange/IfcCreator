namespace IfcCreator.Interface.DTO
{
    public class Organization
    {
        public string name {get; set; }
        public string description {get; set; }
        public string identifier {get; set; }

        private Organization() {
            this.name = "";
            this.description = "";
            this.identifier = "";
        }

        public class Builder
        {
            private Organization organization = new Organization();

            public Organization build()
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