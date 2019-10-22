using System;
using System.Collections;
using System.Collections.Generic;

using BuildingSmart.IFC.IfcProfileResource;
using BuildingSmart.IFC.IfcGeometryResource;
using BuildingSmart.IFC.IfcGeometricModelResource;

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
            Assert.Single(operandStack);
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
            Assert.Single(operandStack);
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
            Assert.Single(((IfcArbitraryProfileDefWithVoids) response).InnerCurves);
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

        [Fact]
        public void ExtrudeTest()
        {
            var operandStack = new Stack();
            IfcPolyline outerCurve = IfcGeom.CreatePolyLine(new List<double[]>() { new double[] {-0.5, -0.5},
                                                                                   new double[] {-0.5, 0.5},
                                                                                   new double[] {0.5, 0.5},
                                                                                   new double[] {0.5, -0.5},
                                                                                   new double[] {-0.5, -0.5}});
            IfcProfileDef profileDef = new IfcArbitraryClosedProfileDef(IfcProfileTypeEnum.AREA,
                                                                        null,
                                                                        outerCurve);
            operandStack.Push(profileDef);
            operandStack.Push("5.75");
            ConstructionOperations.ExecuteOperation(OperationName.EXTRUDE, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsType<IfcExtrudedAreaSolid>(response);
            Assert.Equal(5.75, ((IfcExtrudedAreaSolid) response).Depth.Value.Value);
        }

        [Fact]
        public void RevolveTest()
        {
            var operandStack = new Stack();
            IfcPolyline outerCurve = IfcGeom.CreatePolyLine(new List<double[]>() { new double[] {-0.5, -0.5},
                                                                                   new double[] {-0.5, 0.5},
                                                                                   new double[] {0.5, 0.5},
                                                                                   new double[] {0.5, -0.5},
                                                                                   new double[] {-0.5, -0.5}});
            IfcProfileDef profileDef = new IfcArbitraryClosedProfileDef(IfcProfileTypeEnum.AREA,
                                                                        null,
                                                                        outerCurve);
            operandStack.Push(profileDef);
            operandStack.Push("70.77");
            ConstructionOperations.ExecuteOperation(OperationName.REVOLVE, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsType<IfcRevolvedAreaSolid>(response);
            Assert.Equal(70.77, ((IfcRevolvedAreaSolid) response).Angle.Value);
        }
    }
}