namespace IfcCreator.Interface.DTO
{
    public class Person
    {
        public string givenName {get; private set; }
        public string familyName {get; private set; }
        public string identifier {get; private set; }

        private Person() {
            this.givenName = "";
            this.familyName = "";
            this.identifier = "";
        }

        public class Builder
        {
            private Person person = new Person();

            public Person Build()
            {
                return this.person;
            }

            public Builder withGivenName(string givenName)
            {
                this.person.givenName = givenName;
                return this;
            }

            public Builder withFamilyName(string familyName)
            {
                this.person.familyName = familyName;
                return this;
            }

            public Builder withIdentifier(string identifier)
            {
                this.person.identifier = identifier;
                return this;
            }          
        }

    }
}