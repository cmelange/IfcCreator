using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BuildingSmart.IFC.IfcGeometricModelResource;
using BuildingSmart.IFC.IfcGeometryResource;
using BuildingSmart.IFC.IfcProfileResource;

namespace IfcCreator.Ifc.Geom
{
    public enum OperationName
    {
        POLYLINE2D,
        SHAPE,
        EXTRUDE,
        REVOLVE,
        TRANSLATION,
        ROTATION,
        UNION,
        DIFFERENCE,
        INTERSECTION,
        PLANE,
        CUT_BY_PLANE
    }

    public static class ConstructionOperations
    {
        public static void ExecuteOperation(OperationName operation,
                                            Stack operandStack)
        {
            try
            {
                switch (operation)
                {
                    case OperationName.POLYLINE2D:
                        Polyline2D(operandStack);
                        break;
                    case OperationName.SHAPE:
                        Shape(operandStack);
                        break;
                    case OperationName.EXTRUDE:
                        Extrude(operandStack);
                        break;
                    case OperationName.REVOLVE:
                        Revolve(operandStack);
                        break;
                    case OperationName.TRANSLATION:
                        Translate(operandStack);
                        break;
                    case OperationName.ROTATION:
                        Rotate(operandStack);
                        break;
                    case OperationName.UNION:
                        Union(operandStack);
                        break;
                    case OperationName.DIFFERENCE:
                        Difference(operandStack);
                        break;
                    case OperationName.INTERSECTION:
                        Intersection(operandStack);
                        break;
                    case OperationName.PLANE:
                        Plane(operandStack);
                        break;
                    case OperationName.CUT_BY_PLANE:
                        CutByPlane(operandStack);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid Argument", operation.ToString(), ex);
            }
        }

        private static void Polyline2D(Stack operandStack)
        {
            string operand = (string) operandStack.Pop();
            //remove all spaces
            operand.Replace(" ", "");
            if ((operand.Length < 12) || !operand.StartsWith("[[") || !operand.EndsWith("]]"))
            {   //operand should represent a two dimensional array with at least two points
                throw new ArgumentException(string.Format("Invalid operand for POLYLINE2D: {0}", operand), "POLYLINE2D");
            }
        
            // Parse operand
            int openParenthesis = 2;
            List<double[]> curve = new List<double[]>();
            List<double> currentVector = new List<double>();
            StringBuilder stringBuffer = new StringBuilder(); 
            for (int i=2; i < operand.Length; ++i)
            {
                switch (operand[i])
                {
                    case '[':
                        openParenthesis++;
                        break;
                    case ']':
                        openParenthesis--;
                        if (openParenthesis == 1)
                        {   // closing current vector
                            currentVector.Add(Double.Parse(stringBuffer.ToString()));
                            stringBuffer.Clear();
                            curve.Add(currentVector.ToArray());
                            currentVector = new List<double>();
                        }
                        break;
                    case ',':
                        if (openParenthesis == 2)
                        {   // starting new coordinate in vector
                            currentVector.Add(Double.Parse(stringBuffer.ToString()));
                            stringBuffer.Clear();
                        }
                        break;
                    default:
                        stringBuffer.Append(operand[i]);
                        break;
                }
            }

            // Construct IFC PolyLine
            operandStack.Push(IfcGeom.CreatePolyLine(curve));

        }

        private static void Shape(Stack operandStack)
        {
            var curveList = new List<IfcCurve>();
            var poppedValue = operandStack.Pop();

            while(poppedValue.GetType() != typeof(char) )
            {
                curveList.Add((IfcCurve) poppedValue);
                poppedValue = operandStack.Pop();
            }

            // Construct IFC closed profile
            IfcCurve outerCurve = curveList[curveList.Count-1];
            //remove the outer curve from the list to have a list of the innercurves
            curveList.RemoveRange(curveList.Count-1,1);
            if (curveList.Count == 0)
            {   // only outer curve without voids
                operandStack.Push(new IfcArbitraryClosedProfileDef(IfcProfileTypeEnum.AREA,
                                                                   null,
                                                                   outerCurve));
            }
            else
            {   // outer curve with voids
                operandStack.Push(new IfcArbitraryProfileDefWithVoids(IfcProfileTypeEnum.AREA,
                                                                      null,
                                                                      outerCurve,
                                                                      curveList.ToArray()));
            }
        }

        private static void Extrude(Stack operandStack)
        {
            double extrudeHeight = Double.Parse((string) operandStack.Pop());
            IfcProfileDef extrudeProfile = (IfcProfileDef) operandStack.Pop();
            operandStack.Push(extrudeProfile.Extrude(extrudeHeight));
        }

        private static void Revolve(Stack operandStack)
        {
            double revolveAngle = Double.Parse((string) operandStack.Pop());
            IfcProfileDef revolveProfile = (IfcProfileDef) operandStack.Pop();
            operandStack.Push(revolveProfile.Revolve(revolveAngle));
        }

        private static void Union(Stack operandStack)
        {
            var secondOperand = (IfcBooleanOperand) operandStack.Pop();
            var firstOperand = (IfcBooleanOperand) operandStack.Pop();
            operandStack.Push(firstOperand.Union(secondOperand));
        }

        private static void Difference(Stack operandStack)
        {
            var secondOperand = (IfcBooleanOperand) operandStack.Pop();
            var firstOperand = (IfcBooleanOperand) operandStack.Pop();
            operandStack.Push(firstOperand.Difference(secondOperand));
        }

        private static void Intersection(Stack operandStack)
        {
            var secondOperand = (IfcBooleanOperand) operandStack.Pop();
            var firstOperand = (IfcBooleanOperand) operandStack.Pop();
            operandStack.Push(firstOperand.Intersection(secondOperand));
        }

        private static double[] ParseArray(string vector)
        {
            //remove all spaces
            vector.Replace(" ", "");
            if ((vector.Length < 7) || !vector.StartsWith("[") || !vector.EndsWith("]"))
            {   //operand should represent a one dimensional array
                throw new ArgumentException("Could not parse vector");
            }
            StringBuilder stringBuffer = new StringBuilder();
            List<double> result = new List<double>(); 
            for (int i=1; i < vector.Length; ++i)
            {
                switch (vector[i])
                {
                    case ']':
                        // closing translation vector
                        result.Add(Double.Parse(stringBuffer.ToString()));
                        break;
                    case ',':
                        // starting new coordinate in vector
                        result.Add(Double.Parse(stringBuffer.ToString()));
                        stringBuffer.Clear();
                        break;
                    default:
                        stringBuffer.Append(vector[i]);
                        break;
                }
            }
            return result.ToArray();
        }
        
        private static void Translate(Stack operandStack)
        {
            var operand = (string) operandStack.Pop();
            double[] translationVector = ParseArray(operand);
            
            if (translationVector.Length != 3)
            {
                throw new ArgumentException("Vector should have exactely 3 coordinates");
            }

            var item = operandStack.Pop();
            if (item is IfcSweptAreaSolid)
            {
                operandStack.Push(((IfcSweptAreaSolid) item).Translate(translationVector.ToArray()));
                return;
            }
            if (item is IfcBooleanResult)
            {
                operandStack.Push(((IfcBooleanResult) item).Translate(translationVector.ToArray()));
                return;
            }
            throw new ArgumentException("Invalid type for TRANSLATION");
        }

        private static void Rotate(Stack operandStack)
        {
            var operand = (string) operandStack.Pop();
            double[] rotation = ParseArray(operand);

            var item = operandStack.Pop();
            if (item is IfcSweptAreaSolid)
            {
                operandStack.Push(((IfcSweptAreaSolid) item).Rotate(rotation.ToArray()));
                return;
            }
            if (item is IfcBooleanResult)
            {
                operandStack.Push(((IfcBooleanResult) item).Rotate(rotation.ToArray()));
                return;
            }

            throw new ArgumentException("Invalid type for ROTATION");
        }

        private static void Plane(Stack operandStack)
        {
            var operand = (string) operandStack.Pop();
            //remove all spaces
            operand.Replace(" ", "");
            int splitPos = operand.IndexOf("],") + 1;
            //plane equation is n[0]*x + n[1]*y + n[2]*z = d
            double[] normal = ParseArray(operand.Substring(0,splitPos));
            double d = Double.Parse(operand.Substring(splitPos + 1, operand.Length - splitPos - 1));
            //find point on plane
            double[] point = new double[] {0,0,0};
            for (int i =0; i<3; ++i)
            {
                if (Math.Abs(normal[i]) > 0)
                {
                    point[i] = d/normal[i];
                    break;
                }
            }                
            IfcPlane plane = IfcGeom.CreatePlane(point, normal);
            operandStack.Push(plane);
         }

        private static void CutByPlane(Stack operandStack)
        {
            IfcPlane planeOperand = (IfcPlane) operandStack.Pop();
            var objectOperand = operandStack.Pop();
            if (objectOperand is IfcSweptAreaSolid)
            {
                operandStack.Push(((IfcSweptAreaSolid) objectOperand).ClipByPlane(planeOperand));
                return;
            }
            if (objectOperand is IfcBooleanClippingResult)
            {
                operandStack.Push(((IfcBooleanClippingResult) objectOperand).ClipByPlane(planeOperand));
                return;
            }

            throw new ArgumentException("Invalid type for CUT_BY_PLANE");
        }

    }
}