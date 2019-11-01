namespace IfcCreator.Interface.DTO
{
    public class RepresentationItem
    {
        public string constructionString {get; private set; }

        public RepresentationItem(string constructionString)
        {
            this.constructionString = constructionString;
        }
    }
}