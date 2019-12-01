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
            string constructionString = @"SHAPE({POLYLINE2D([[0,0],[0,1],[1,1],[1,0]]);
                                                 POLYLINE2D([[0.25,0.25],[0.25,0.75],[0.75,0.75],[0.75,0.25]])})
                                          .EXTRUDE(1)
                                          .UNION(SHAPE({POLYLINE2D([[0.5,0.5],[0.5,1.5],[1.5,1.5],[1.5,0.5]])})
                                                .EXTRUDE(1))";
            IfcRepresentationItem representation = 
                RepresentationParser.ParseConstructionString(constructionString);
            Assert.IsAssignableFrom<IfcBooleanResult>(representation);
        }
    }
}