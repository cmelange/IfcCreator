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
        public void Polyline2DTest()
        {
            var operandStack = new Stack();
            operandStack.Push("[[0,0],[1.0,1.0],[1,0]]");
            var operation = OperationName.POLYLINE2D;
            ConstructionOperations.ExecuteOperation(operation, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsType<IfcPolyline>(response);
            Assert.Collection(((IfcPolyline) response).Points,                         
                              p0 => { Assert.Equal(0, p0.Coordinates[0].Value);
                                      Assert.Equal(0, p0.Coordinates[1].Value); },
                              p1 => { Assert.Equal(1, p1.Coordinates[0].Value);
                                      Assert.Equal(1, p1.Coordinates[1].Value); },
                              p2 => { Assert.Equal(1, p2.Coordinates[0].Value);
                                      Assert.Equal(0, p2.Coordinates[1].Value); });
        }

        [Theory]
        [InlineData("[0,1]")]
        [InlineData("[[0,1]]")]
        [InlineData("[[0,0],[1.0],[1,0]]")]
        public void Polyline2DExceptionsTest(string operand)
        {
            var operandStack = new Stack();
            operandStack.Push(operand);
            var operation = OperationName.POLYLINE2D;
            Assert.Throws<ArgumentException>(
                () => ConstructionOperations.ExecuteOperation(operation, operandStack));
        }

        [Fact]
        public void Circleline2DTest()
        {
            var operandStack = new Stack();
            operandStack.Push("[0.5, -0.5]");   // center
            operandStack.Push("1.5");           // radius
            operandStack.Push("180");           // start parameter
            operandStack.Push("90");            // end parameter
            var operation = OperationName.CIRCLELINE2D;
            ConstructionOperations.ExecuteOperation(operation, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsType<IfcTrimmedCurve>(response);
            IfcTrimmedCurve curve = (IfcTrimmedCurve) response;
            Assert.IsType<IfcCircle>(curve.BasisCurve);
            IfcCircle circle = (IfcCircle) curve.BasisCurve;
            Assert.Equal(0.5, ((IfcAxis2Placement2D) circle.Position).Location.Coordinates[0].Value);
            Assert.Equal(-0.5, ((IfcAxis2Placement2D) circle.Position).Location.Coordinates[1].Value);
            Assert.Equal(1, ((IfcAxis2Placement2D) circle.Position).RefDirection.DirectionRatios[0].Value);
            Assert.Equal(0, ((IfcAxis2Placement2D) circle.Position).RefDirection.DirectionRatios[1].Value);
            Assert.Equal(1.5, circle.Radius.Value.Value);
            var trim1Enumerator = curve.Trim1.GetEnumerator();
            trim1Enumerator.MoveNext();
            Assert.Equal(180, ((IfcParameterValue) trim1Enumerator.Current).Value);
            var trim2Enumerator = curve.Trim2.GetEnumerator();
            trim2Enumerator.MoveNext();
            Assert.Equal(90, ((IfcParameterValue) trim2Enumerator.Current).Value);
            Assert.True(curve.SenseAgreement.Value);
            Assert.Equal(IfcTrimmingPreference.PARAMETER, curve.MasterRepresentation);
        }

        [Fact]
        public void CompositeCurve2DTest()
        {
            var operandStack = new Stack();
            operandStack.Push('{');
            operandStack.Push(IfcGeom.CreatePolyLine(new List<double[]>() { new double[] {-0.5, -0.5},
                                                                            new double[] {0, 0}}));
            operandStack.Push(IfcGeom.CreatePolyLine(new List<double[]>() { new double[] {0, 0},
                                                                            new double[] {0.5, 0.5}}));
            var operation = OperationName.COMPOSITE_CURVE2D;
            ConstructionOperations.ExecuteOperation(operation, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsType<IfcCompositeCurve>(response);
            IfcCompositeCurve curve = (IfcCompositeCurve) response;
            Assert.Equal(2, curve.Segments.Count);
            IfcCompositeCurveSegment curve0 = curve.Segments[0];
            Assert.Equal(IfcTransitionCode.CONTINUOUS, curve0.Transition);
            Assert.True(curve0.SameSense.Value);
            Assert.Equal(-0.5, ((IfcPolyline) curve0.ParentCurve).Points[0].Coordinates[0].Value);
            Assert.Equal(-0.5, ((IfcPolyline) curve0.ParentCurve).Points[0].Coordinates[1].Value);
            Assert.Equal(0, ((IfcPolyline) curve0.ParentCurve).Points[1].Coordinates[0].Value);
            Assert.Equal(0, ((IfcPolyline) curve0.ParentCurve).Points[1].Coordinates[1].Value);
            IfcCompositeCurveSegment curve1 = curve.Segments[1];
            Assert.Equal(IfcTransitionCode.CONTINUOUS, curve1.Transition);
            Assert.True(curve1.SameSense.Value);
            Assert.Equal(0, ((IfcPolyline) curve1.ParentCurve).Points[0].Coordinates[0].Value);
            Assert.Equal(0, ((IfcPolyline) curve1.ParentCurve).Points[0].Coordinates[1].Value);
            Assert.Equal(0.5, ((IfcPolyline) curve1.ParentCurve).Points[1].Coordinates[0].Value);
            Assert.Equal(0.5, ((IfcPolyline) curve1.ParentCurve).Points[1].Coordinates[1].Value);
        }

        [Fact]
        public void Shape()
        {
            var operandStack = new Stack();
            IfcPolyline outerCurve = IfcGeom.CreatePolyLine(new List<double[]>() { new double[] {-0.5, -0.5},
                                                                                   new double[] {-0.5, 0.5},
                                                                                   new double[] {0.5, 0.5},
                                                                                   new double[] {0.5, -0.5},
                                                                                   new double[] {-0.5, -0.5}});
            operandStack.Push('{');
            operandStack.Push(outerCurve);
            var operation = OperationName.SHAPE;
            ConstructionOperations.ExecuteOperation(operation, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsType<IfcArbitraryClosedProfileDef>(response);
            Assert.Collection(((IfcPolyline)((IfcArbitraryClosedProfileDef) response).OuterCurve).Points,
                              p0 => { Assert.Equal(-0.5, p0.Coordinates[0].Value);
                                      Assert.Equal(-0.5, p0.Coordinates[1].Value); },
                              p1 => { Assert.Equal(-0.5, p1.Coordinates[0].Value);
                                      Assert.Equal(0.5, p1.Coordinates[1].Value); },
                              p2 => { Assert.Equal(0.5, p2.Coordinates[0].Value);
                                      Assert.Equal(0.5, p2.Coordinates[1].Value); },
                              p3 => { Assert.Equal(0.5, p3.Coordinates[0].Value);
                                      Assert.Equal(-0.5, p3.Coordinates[1].Value); },
                              p4 => { Assert.Equal(-0.5, p4.Coordinates[0].Value);
                                      Assert.Equal(-0.5, p4.Coordinates[1].Value); });

            IfcPolyline innerCurve = IfcGeom.CreatePolyLine(new List<double[]>() { new double[] {-0.25, -0.25},
                                                                                   new double[] {-0.25, 0.25},
                                                                                   new double[] {0.25, 0.25},
                                                                                   new double[] {0.25, -0.25},
                                                                                   new double[] {-0.25, -0.25}});
            operandStack.Push('{');
            operandStack.Push(outerCurve);
            operandStack.Push(innerCurve);
            ConstructionOperations.ExecuteOperation(operation, operandStack);
            Assert.Single(operandStack);
            response = operandStack.Pop();
            Assert.IsType<IfcArbitraryProfileDefWithVoids>(response);
            Assert.Collection(((IfcPolyline)((IfcArbitraryProfileDefWithVoids) response).OuterCurve).Points,
                              p0 => { Assert.Equal(-0.5, p0.Coordinates[0].Value);
                                      Assert.Equal(-0.5, p0.Coordinates[1].Value); },
                              p1 => { Assert.Equal(-0.5, p1.Coordinates[0].Value);
                                      Assert.Equal(0.5, p1.Coordinates[1].Value); },
                              p2 => { Assert.Equal(0.5, p2.Coordinates[0].Value);
                                      Assert.Equal(0.5, p2.Coordinates[1].Value); },
                              p3 => { Assert.Equal(0.5, p3.Coordinates[0].Value);
                                      Assert.Equal(-0.5, p3.Coordinates[1].Value); },
                              p4 => { Assert.Equal(-0.5, p4.Coordinates[0].Value);
                                      Assert.Equal(-0.5, p4.Coordinates[1].Value); });
            Assert.Single(((IfcArbitraryProfileDefWithVoids) response).InnerCurves);
            var innerCurveEnumerator = ((IfcArbitraryProfileDefWithVoids) response).InnerCurves.GetEnumerator();
            innerCurveEnumerator.MoveNext();
            Assert.Collection(((IfcPolyline) innerCurveEnumerator.Current).Points,
                              p0 => { Assert.Equal(-0.25, p0.Coordinates[0].Value);
                                      Assert.Equal(-0.25, p0.Coordinates[1].Value); },
                              p1 => { Assert.Equal(-0.25, p1.Coordinates[0].Value);
                                      Assert.Equal(0.25, p1.Coordinates[1].Value); },
                              p2 => { Assert.Equal(0.25, p2.Coordinates[0].Value);
                                      Assert.Equal(0.25, p2.Coordinates[1].Value); },
                              p3 => { Assert.Equal(0.25, p3.Coordinates[0].Value);
                                      Assert.Equal(-0.25, p3.Coordinates[1].Value); },
                              p4 => { Assert.Equal(-0.25, p4.Coordinates[0].Value);
                                      Assert.Equal(-0.25, p4.Coordinates[1].Value); });
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
            operandStack.Push("[-90,0,90]");
            ConstructionOperations.ExecuteOperation(OperationName.ROTATION, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsAssignableFrom<IfcSweptAreaSolid>(response);
            Assert.Equal(0, ((IfcSweptAreaSolid) response).Position.RefDirection.DirectionRatios[0].Value, 10);
            Assert.Equal(0, ((IfcSweptAreaSolid) response).Position.RefDirection.DirectionRatios[1].Value, 10);
            Assert.Equal(-1, ((IfcSweptAreaSolid) response).Position.RefDirection.DirectionRatios[2].Value, 10);

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
            operandStack.Push("[-90,0,90]");
            ConstructionOperations.ExecuteOperation(OperationName.ROTATION, operandStack);
            Assert.Single(operandStack);
            response = operandStack.Pop();
            Assert.IsAssignableFrom<IfcBooleanResult>(response);
            Assert.Equal(0, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).FirstOperand).Position.RefDirection.DirectionRatios[0].Value, 10);
            Assert.Equal(0, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).FirstOperand).Position.RefDirection.DirectionRatios[1].Value, 10);
            Assert.Equal(-1, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).FirstOperand).Position.RefDirection.DirectionRatios[2].Value, 10);
            Assert.Equal(0, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).SecondOperand).Position.RefDirection.DirectionRatios[0].Value, 10);
            Assert.Equal(0, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).SecondOperand).Position.RefDirection.DirectionRatios[1].Value, 10);
            Assert.Equal(-1, ((IfcCsgPrimitive3D) ((IfcBooleanResult) response).SecondOperand).Position.RefDirection.DirectionRatios[2].Value, 10);
        }        

        [Fact]
        public void PlaneTest()
        {
            var operandStack = new Stack();
            operandStack.Push("[0,0,2],-2");
            ConstructionOperations.ExecuteOperation(OperationName.PLANE, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsAssignableFrom<IfcPlane>(response);
            Assert.Equal(0, ((IfcPlane) response).Position.Axis.DirectionRatios[0].Value);
            Assert.Equal(0, ((IfcPlane) response).Position.Axis.DirectionRatios[1].Value);
            Assert.Equal(2, ((IfcPlane) response).Position.Axis.DirectionRatios[2].Value);
            Assert.Equal(0, ((IfcPlane) response).Position.Location.Coordinates[0].Value);
            Assert.Equal(0, ((IfcPlane) response).Position.Location.Coordinates[1].Value);
            Assert.Equal(-1, ((IfcPlane) response).Position.Location.Coordinates[2].Value);
        }

        [Fact]
        public void CutByPlaneTest()
        {
            // === Cut IfcSweptAreaSolid
            var operandStack = new Stack();
            IfcPolyline outerCurve = IfcGeom.CreatePolyLine(new List<double[]>() { new double[] {6, 0},
                                                                                   new double[] {6, 1},
                                                                                   new double[] {7, 1},
                                                                                   new double[] {7, 0},
                                                                                   new double[] {6, 0}});
            IfcProfileDef profileDef = new IfcArbitraryClosedProfileDef(IfcProfileTypeEnum.AREA,
                                                                        null,
                                                                        outerCurve);
            operandStack.Push(profileDef.Extrude(1));
            operandStack.Push(IfcGeom.CreatePlane(new double[] {6.5, 0.5, 0},
                                                  new double[] {1,1,0}));
            ConstructionOperations.ExecuteOperation(OperationName.CUT_BY_PLANE, operandStack);
            Assert.Single(operandStack);
            var response = operandStack.Pop();
            Assert.IsAssignableFrom<IfcBooleanClippingResult>(response);
            Assert.IsType<IfcExtrudedAreaSolid>(((IfcBooleanClippingResult) response).FirstOperand);
            Assert.IsType<IfcPlane>(((IfcHalfSpaceSolid) ((IfcBooleanClippingResult) response).SecondOperand).BaseSurface);

            // === Cut IfcSweptAreaSolid
            operandStack.Push(response);
            operandStack.Push(IfcGeom.CreatePlane(new double[] {6.5, 0.5, 0},
                                                  new double[] {1,1,0}));
            ConstructionOperations.ExecuteOperation(OperationName.CUT_BY_PLANE, operandStack);
            Assert.Single(operandStack);
            response = operandStack.Pop();
            Assert.IsAssignableFrom<IfcBooleanClippingResult>(response);
            Assert.IsType<IfcBooleanClippingResult>(((IfcBooleanClippingResult) response).FirstOperand);
            Assert.IsType<IfcPlane>(((IfcHalfSpaceSolid) ((IfcBooleanClippingResult) response).SecondOperand).BaseSurface);
        }

    }
}