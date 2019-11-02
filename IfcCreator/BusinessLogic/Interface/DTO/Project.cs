namespace IfcCreator.Interface.DTO
{
    public class Project
    {
        public string name {get; set; }
        public string description {get; set; }

        private Project() {
            this.name = "";
            this.description = "";
        }

        public class Builder
        {
            private Project project = new Project();

            public Project Build()
            {
                return this.project;
            }

            public Builder withName(string name)
            {
                this.project.name = name;
                return this;
            }

            public Builder withDescription(string description)
            {
                this.project.description = description;
                return this;
            }
        }

    }
}