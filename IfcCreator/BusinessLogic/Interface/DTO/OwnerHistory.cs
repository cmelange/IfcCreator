namespace IfcCreator.Interface.DTO
{
    public class OwnerHistory
    {
        public Person person {get; set; }
        public Organization organization {get; set; }
        public Application application {get; set; }

        private OwnerHistory()
        {
            this.person = new Person.Builder().Build();
            this.organization = new Organization.Builder().Build();
            this.application = new Application.Builder().Build();
        }

        public class Builder
        {
            private OwnerHistory ownerHistory;

            public Builder()
            {
                this.ownerHistory = new OwnerHistory();
            }

            public OwnerHistory Build()
            {
                return this.ownerHistory;
            }

            public Builder withPerson(Person person)
            {
                this.ownerHistory.person = person;
                return this;
            }

            public Builder withOrganization(Organization organization)
            {
                this.ownerHistory.organization = organization;
                return this;
            }

            public Builder withApllication(Application application)
            {
                this.ownerHistory.application = application;
                return this;
            }
        }
    }
}