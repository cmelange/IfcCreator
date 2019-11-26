namespace IfcCreator.Interface.DTO
{
    public class Person
    {
        public string givenName {get; set; }
        public string familyName {get; set; }
        public string identifier {get; set; }

        private Person() {
            this.givenName = "";
            this.familyName = "";
            this.identifier = "";
        }

        public class Builder
        {
            private Person person = new Person();

            public Person build()
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