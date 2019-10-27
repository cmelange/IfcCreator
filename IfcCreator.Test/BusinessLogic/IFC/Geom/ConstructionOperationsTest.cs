using System;
using System.Collections;
using System.Collections.Generic;

using BuildingSmart.IFC.IfcProfileResource;
using BuildingSmart.IFC.IfcGeometryResource;
using BuildingSmart.IFC.IfcGeometricModelResource;
using BuildingSmart.IFC.IfcMeasureResource;

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

        [Fact]
        public void UnionTest()
        {
            var operandStack = new Stack();
            IfcCsgPrimitive3D first = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(0,0,0), null, null),
                                                   new IfcPositiveLengthMeasure(1),
                                                   new IfcPositiveLengthMeasure(1),
                                                   new IfcPositiveLengthMeasure(1));
            IfcCsgPrimitive3D second = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(0.5,0.5,0.5), null, null),
                                                    new IfcPositiveLengthMeasure(1),
                                                    new IfcPositiveLengthMeasure(1),
                                                    new IfcPositiveLengthMeasure(1));
            operandStack.Push(first);
            operandStack.Push(second);
            ConstructionOperations.ExecuteOperation(OperationName.UNION, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsType<IfcBooleanResult>(response);
            Assert.Equal(IfcBooleanOperator.UNION, ((IfcBooleanResult) response).Operator);
            Assert.Equal(0, ((IfcBlock)((IfcBooleanResult) response).FirstOperand).Position.Location.Coordinates[0].Value);
            Assert.Equal(0.5, ((IfcBlock)((IfcBooleanResult) response).SecondOperand).Position.Location.Coordinates[0].Value);
        }

        [Fact]
        public void DifferenceTest()
        {
            var operandStack = new Stack();
            IfcCsgPrimitive3D first = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(0,0,0), null, null),
                                                   new IfcPositiveLengthMeasure(1),
                                                   new IfcPositiveLengthMeasure(1),
                                                   new IfcPositiveLengthMeasure(1));
            IfcCsgPrimitive3D second = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(0.5,0.5,0.5), null, null),
                                                    new IfcPositiveLengthMeasure(1),
                                                    new IfcPositiveLengthMeasure(1),
                                                    new IfcPositiveLengthMeasure(1));
            operandStack.Push(first);
            operandStack.Push(second);
            ConstructionOperations.ExecuteOperation(OperationName.DIFFERENCE, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsType<IfcBooleanResult>(response);
            Assert.Equal(IfcBooleanOperator.DIFFERENCE, ((IfcBooleanResult) response).Operator);
            Assert.Equal(0, ((IfcBlock)((IfcBooleanResult) response).FirstOperand).Position.Location.Coordinates[0].Value);
            Assert.Equal(0.5, ((IfcBlock)((IfcBooleanResult) response).SecondOperand).Position.Location.Coordinates[0].Value);
        }

        [Fact]
        public void IntersectionTest()
        {
            var operandStack = new Stack();
            IfcCsgPrimitive3D first = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(0,0,0), null, null),
                                                   new IfcPositiveLengthMeasure(1),
                                                   new IfcPositiveLengthMeasure(1),
                                                   new IfcPositiveLengthMeasure(1));
            IfcCsgPrimitive3D second = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(0.5,0.5,0.5), null, null),
                                                    new IfcPositiveLengthMeasure(1),
                                                    new IfcPositiveLengthMeasure(1),
                                                    new IfcPositiveLengthMeasure(1));
            operandStack.Push(first);
            operandStack.Push(second);
            ConstructionOperations.ExecuteOperation(OperationName.INTERSECTION, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsType<IfcBooleanResult>(response);
            Assert.Equal(IfcBooleanOperator.INTERSECTION, ((IfcBooleanResult) response).Operator);
            Assert.Equal(0, ((IfcBlock)((IfcBooleanResult) response).FirstOperand).Position.Location.Coordinates[0].Value);
            Assert.Equal(0.5, ((IfcBlock)((IfcBooleanResult) response).SecondOperand).Position.Location.Coordinates[0].Value);
        }

        [Fact]
        public void TranslationTest()
        {
            // ==== Translate IfcSweptAreaSolid
            var operandStack = new Stack();
            IfcPolyline outerCurve = IfcGeom.CreatePolyLine(new List<double[]>() { new double[] {-1, -1},
                                                                                   new double[] {-1, 1},
                                                                                   new double[] {1, 1},
                                                                                   new double[] {1, -1},
                                                                                   new double[] {-1, -1}});
            IfcProfileDef profileDef = new IfcArbitraryClosedProfileDef(IfcProfileTypeEnum.AREA,
                                                                        null,
                                                                        outerCurve);
            operandStack.Push(profileDef.Extrude(1));
            operandStack.Push("[1,2,3]");
            ConstructionOperations.ExecuteOperation(OperationName.TRANSLATION, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsAssignableFrom<IfcSweptAreaSolid>(response);
            Assert.Equal(1, ((IfcSweptAreaSolid) response).Position.Location.Coordinates[0].Value);
            Assert.Equal(2, ((IfcSweptAreaSolid) response).Position.Location.Coordinates[1].Value);
            Assert.Equal(3, ((IfcSweptAreaSolid) response).Position.Location.Coordinates[2].Value);

            // ==== Translate IfcBooleanResult
            operandStack.Clear();
            IfcCsgPrimitive3D union_first = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(0,0,0), null, null),
                                                         new IfcPositiveLengthMeasure(1),
                                                         new IfcPositiveLengthMeasure(1),
                                                         new IfcPositiveLengthMeasure(1));
            IfcCsgPrimitive3D union_second = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(0.5,0.5,0.5), null, null),
                                                          new IfcPositiveLengthMeasure(1),
                                                          new IfcPositiveLengthMeasure(1),
                                                          new IfcPositiveLengthMeasure(1));
            operandStack.Push(union_first.Union(union_second));
            operandStack.Push("[1,2,3]");
            ConstructionOperations.ExecuteOperation(OperationName.TRANSLATION, operandStack);
            Assert.Single(operandStack);
            response = operandStack.Pop();
            Assert.IsAssignableFrom<IfcBooleanResult>(response);
            Assert.Equal(1, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).FirstOperand).Position.Location.Coordinates[0].Value);
            Assert.Equal(2, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).FirstOperand).Position.Location.Coordinates[1].Value);
            Assert.Equal(3, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).FirstOperand).Position.Location.Coordinates[2].Value);
            Assert.Equal(1.5, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).SecondOperand).Position.Location.Coordinates[0].Value);
            Assert.Equal(2.5, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).SecondOperand).Position.Location.Coordinates[1].Value);
            Assert.Equal(3.5, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).SecondOperand).Position.Location.Coordinates[2].Value);
        }

        [Fact]
        public void RotationTest()
        {
            // ==== Rotate IfcSweptAreaSolid
            var operandStack = new Stack();
            IfcPolyline outerCurve = IfcGeom.CreatePolyLine(new List<double[]>() { new double[] {-1, -1},
                                                                                   new double[] {-1, 1},
                                                                                   new double[] {1, 1},
                                                                                   new double[] {1, -1},
                                                                                   new double[] {-1, -1}});
            IfcProfileDef profileDef = new IfcArbitraryClosedProfileDef(IfcProfileTypeEnum.AREA,
                                                                        null,
                                                                        outerCurve);
            operandStack.Push(profileDef.Extrude(1));
            operandStack.Push("[90,90,90]");
            ConstructionOperations.ExecuteOperation(OperationName.ROTATION, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsAssignableFrom<IfcSweptAreaSolid>(response);
            Assert.Equal(0, ((IfcSweptAreaSolid) response).Position.RefDirection.DirectionRatios[0].Value, 10);
            Assert.Equal(0, ((IfcSweptAreaSolid) response).Position.RefDirection.DirectionRatios[1].Value, 10);
            Assert.Equal(1, ((IfcSweptAreaSolid) response).Position.RefDirection.DirectionRatios[2].Value, 10);

            // ==== Rotate IfcBooleanResult
            operandStack.Clear();
            IfcCsgPrimitive3D union_first = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(0,0,0), null, null),
                                                         new IfcPositiveLengthMeasure(1),
                                                         new IfcPositiveLengthMeasure(1),
                                                         new IfcPositiveLengthMeasure(1));
            IfcCsgPrimitive3D union_second = new IfcBlock(new IfcAxis2Placement3D(new IfcCartesianPoint(0.5,0.5,0.5), null, null),
                                                          new IfcPositiveLengthMeasure(1),
                                                          new IfcPositiveLengthMeasure(1),
                                                          new IfcPositiveLengthMeasure(1));
            operandStack.Push(union_first.Union(union_second));
            operandStack.Push("[90,90,90]");
            ConstructionOperations.ExecuteOperation(OperationName.ROTATION, operandStack);
            Assert.Single(operandStack);
            response = operandStack.Pop();
            Assert.IsAssignableFrom<IfcBooleanResult>(response);
            Assert.Equal(0, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).FirstOperand).Position.RefDirection.DirectionRatios[0].Value, 10);
            Assert.Equal(0, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).FirstOperand).Position.RefDirection.DirectionRatios[1].Value, 10);
            Assert.Equal(1, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).FirstOperand).Position.RefDirection.DirectionRatios[2].Value, 10);
            Assert.Equal(0, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).SecondOperand).Position.RefDirection.DirectionRatios[0].Value, 10);
            Assert.Equal(0, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).SecondOperand).Position.RefDirection.DirectionRatios[1].Value, 10);
            Assert.Equal(1, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).SecondOperand).Position.RefDirection.DirectionRatios[2].Value, 10);
        }        

    }
}