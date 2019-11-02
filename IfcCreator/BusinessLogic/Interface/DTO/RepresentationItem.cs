namespace IfcCreator.Interface.DTO
{
    public class RepresentationItem
    {
        public string constructionString {get; set; }

        public RepresentationItem()
        {}

        public RepresentationItem(string constructionString)
        {
            this.constructionString = constructionString;
        }
    }
}