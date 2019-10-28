using BuildingSmart.IFC.IfcGeometryResource;
using BuildingSmart.IFC.IfcGeometricModelResource;

using Xunit;

namespace IfcCreator.Ifc.Geom
{
    public class RepresentationParserTest
    {
        [Fact]
        public void ParseConstructionStringTest()
        {
            string constructionString = @"POLYGON_SHAPE([[[0,0],[0,1],[1,1],[1,0]]]).EXTRUDE(1)
                                          .UNION(POLYGON_SHAPE([[[0.5,0.5],[0.5,1.5],[1.5,1.5],[1.5,0.5]]]).EXTRUDE(1))";
            IfcRepresentationItem representation = 
                RepresentationParser.ParseConstructionString(constructionString);
            Assert.IsAssignableFrom<IfcBooleanResult>(representation);
        }
    }
}