using System;
using System.Collections;

using BuildingSmart.IFC.IfcProfileResource;
using BuildingSmart.IFC.IfcGeometryResource;

using Xunit;

namespace IfcCreator.Ifc.Geom
{
    public class ConstructionOperationsTest
    {
        [Fact]
        public void PolygonShapeTest()
        {
            var operandStack = new Stack();
            operandStack.Push("[[[0,0],[1.0,1.0],[1,0]]]");
            var operation = OperationName.POLYGON_SHAPE;
            ConstructionOperations.ExecuteOperation(operation, operandStack);
            Assert.Equal(1, operandStack.Count);
            var response = operandStack.Pop();
            Assert.IsType<IfcArbitraryClosedProfileDef>(response);
            Assert.Collection(((IfcPolyline)((IfcArbitraryClosedProfileDef) response).OuterCurve).Points,
                              p0 => { Assert.Equal(0, p0.Coordinates[0].Value);
                                      Assert.Equal(0, p0.Coordinates[1].Value); },
                              p1 => { Assert.Equal(1, p1.Coordinates[0].Value);
                                      Assert.Equal(1, p1.Coordinates[1].Value); },
                              p2 => { Assert.Equal(1, p2.Coordinates[0].Value);
                                      Assert.Equal(0, p2.Coordinates[1].Value); },
                              p3 => { Assert.Equal(0, p3.Coordinates[0].Value);
                                      Assert.Equal(0, p3.Coordinates[1].Value); });

            operandStack.Push("[[[0,0],[1.0,1.0],[1,0]], [[0.5,0.5],[0.75,0.75],[0.75,0]]]");
            ConstructionOperations.ExecuteOperation(operation, operandStack);
            Assert.Equal(1, operandStack.Count);
            response = operandStack.Pop();
            Assert.IsType<IfcArbitraryProfileDefWithVoids>(response);
            Assert.Collection(((IfcPolyline)((IfcArbitraryProfileDefWithVoids) response).OuterCurve).Points,
                              p0 => { Assert.Equal(0, p0.Coordinates[0].Value);
                                      Assert.Equal(0, p0.Coordinates[1].Value); },
                              p1 => { Assert.Equal(1, p1.Coordinates[0].Value);
                                      Assert.Equal(1, p1.Coordinates[1].Value); },
                              p2 => { Assert.Equal(1, p2.Coordinates[0].Value);
                                      Assert.Equal(0, p2.Coordinates[1].Value); },
                              p3 => { Assert.Equal(0, p3.Coordinates[0].Value);
                                      Assert.Equal(0, p3.Coordinates[1].Value); });
            Assert.Equal(1, ((IfcArbitraryProfileDefWithVoids) response).InnerCurves.Count);
            var innerCurveEnumerator = ((IfcArbitraryProfileDefWithVoids) response).InnerCurves.GetEnumerator();
            innerCurveEnumerator.MoveNext();
            Assert.Collection(((IfcPolyline) innerCurveEnumerator.Current).Points,
                              p0 => { Assert.Equal(0.5, p0.Coordinates[0].Value);
                                      Assert.Equal(0.5, p0.Coordinates[1].Value); },
                              p1 => { Assert.Equal(0.75, p1.Coordinates[0].Value);
                                      Assert.Equal(0.75, p1.Coordinates[1].Value); },
                              p2 => { Assert.Equal(0.75, p2.Coordinates[0].Value);
                                      Assert.Equal(0, p2.Coordinates[1].Value); },
                              p3 => { Assert.Equal(0.5, p3.Coordinates[0].Value);
                                      Assert.Equal(0.5, p3.Coordinates[1].Value); });
        }

        [Theory]
        [InlineData("[[0,1]]")]
        [InlineData("[[[0,1]]]")]
        [InlineData("[[[0,0],[1.0],[1,0]]]")]
        public void PolygonShapeExceptionsTest(string operand)
        {
            var operandStack = new Stack();
            operandStack.Push(operand);
            var operation = OperationName.POLYGON_SHAPE;
            Assert.Throws<ArgumentException>(
                () => ConstructionOperations.ExecuteOperation(operation, operandStack));
        }
    }
}