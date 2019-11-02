namespace IfcCreator.Interface.DTO
{
    public class Application
    {
        public string name {get; set; }
        public string version {get; set; }
        public string identifier {get; set; }
        public Organization organization {get; set; }

        private Application()
        {
            this.name = "";
            this.version = "";
            this.identifier = "";
            this.organization = new Organization.Builder().Build();
        }

        public class Builder
        {
            private Application application = new Application();

            public Application Build()
            {
                return this.application;
            }

            public Builder withName(string name)
            {
                this.application.name = name;
                return this;
            }

            public Builder withVersion(string version)
            {
                this.application.version = version;
                return this;
            }

            public Builder withIdentifier(string identifier)
            {
                this.application.identifier = identifier;
                return this;
            }

            public Builder withOrganization(Organization organization)
            {
                this.application.organization = organization;
                return this;
            }
        }
    }
}