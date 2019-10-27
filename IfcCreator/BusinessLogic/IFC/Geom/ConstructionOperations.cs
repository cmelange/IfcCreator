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
        POLYGON_SHAPE,
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
            switch (operation)
            {
                case OperationName.POLYGON_SHAPE:
                    PolygonShape(operandStack);
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

        private static void PolygonShape(Stack operandStack)
        {
            try
            {
                string operand = (string) operandStack.Pop();
                //remove all spaces
                operand.Replace(" ", "");
                if ((operand.Length < 19) || !operand.StartsWith("[[[") || !operand.EndsWith("]]]"))
                {   //operand should represent a three dimensional array
                    throw new ArgumentException(string.Format("Invalid operand for POLYGON_SHAPE: {0}", operand));
                }
            
                // Parse operand
                int openParenthesis = 2;
                List<List<double[]>> paths = new List<List<double[]>>();
                List<double[]> currentPath = new List<double[]>();
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
                            if (openParenthesis == 2)
                            {   // closing current vector
                                currentVector.Add(Double.Parse(stringBuffer.ToString()));
                                stringBuffer.Clear();
                                currentPath.Add(currentVector.ToArray());
                                currentVector = new List<double>();
                            }
                            if (openParenthesis == 1)
                            {   // closing current path
                                paths.Add(currentPath);
                                currentPath = new List<double[]>();
                            }
                            break;
                        case ',':
                            if (openParenthesis == 3)
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

                // Construct IFC closed profile
                var curveList = new List<IfcPolyline>();
                for (int i=0; i<paths.Count; ++i)
                {
                    if (!paths[i][0].SequenceEqual(paths[i][paths[i].Count-1]))
                    {   // close the curve when needed
                        paths[i].Add(paths[i][0]);
                    }
                    if (paths[i].Count < 4)
                    {
                        throw new ArgumentException("At least 4 points are needed to define a shape");
                    }
                    curveList.Add(IfcGeom.CreatePolyLine(paths[i]));
                }

                IfcCurve outerCurve = curveList[0];
                //remove the outer curve from the list to have a list of the innercurves
                curveList.RemoveRange(0,1);
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

            } catch (Exception e) 
            {
                throw new ArgumentException("Invalid operand for POLYGON_SHAPE", e);
            }
        }

        private static void Extrude(Stack operandStack)
        {
            try
            {
                double extrudeHeight = Double.Parse((string) operandStack.Pop());
                IfcProfileDef extrudeProfile = (IfcProfileDef) operandStack.Pop();
                operandStack.Push(extrudeProfile.Extrude(extrudeHeight));
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid operands for EXTRUDE", e);
            }
        }

        private static void Revolve(Stack operandStack)
        {
            try
            {
                double revolveAngle = Double.Parse((string) operandStack.Pop());
                IfcProfileDef revolveProfile = (IfcProfileDef) operandStack.Pop();
                operandStack.Push(revolveProfile.Revolve(revolveAngle));
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid operands for REVOLVE", e);
            }
        }

        private static void Union(Stack operandStack)
        {
            try
            {
                var secondOperand = (IfcBooleanOperand) operandStack.Pop();
                var firstOperand = (IfcBooleanOperand) operandStack.Pop();
                operandStack.Push(firstOperand.Union(secondOperand));
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid operands for UNION", e);
            }
        }

        private static void Difference(Stack operandStack)
        {
            try
            {
                var secondOperand = (IfcBooleanOperand) operandStack.Pop();
                var firstOperand = (IfcBooleanOperand) operandStack.Pop();
                operandStack.Push(firstOperand.Difference(secondOperand));
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid operands for DIFFERENCE", e);
            }
        }

        private static void Intersection(Stack operandStack)
        {
            try
            {
                var secondOperand = (IfcBooleanOperand) operandStack.Pop();
                var firstOperand = (IfcBooleanOperand) operandStack.Pop();
                operandStack.Push(firstOperand.Intersection(secondOperand));
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid operands for INTERSECTION", e);
            }
        }

        private static double[] ParseVector(string vector)
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
            if (result.Count != 3)
            {
                throw new ArgumentException("Vector should have exactely 3 coordinates");
            }
            return result.ToArray();
        }
        
        private static void Translate(Stack operandStack)
        {
            try
            {

                var operand = (string) operandStack.Pop();
                double[] translationVector = ParseVector(operand);

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
            catch (Exception e)
            {
                throw new ArgumentException("Invalid operand for TRANSLATION", e);
            }
        }

        private static void Rotate(Stack operandStack)
        {
            try
            {
                var operand = (string) operandStack.Pop();
                double[] rotation = ParseVector(operand);

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
            catch (Exception e)
            {
                throw new ArgumentException("Invalid operand for ROTATION", e);
            }
        }

        private static void Plane(Stack operandStack)
        {
            try
            {
                var operand = (string) operandStack.Pop();
                //remove all spaces
                operand.Replace(" ", "");
                int splitPos = operand.IndexOf("],") + 1;
                //plane equation is n[0]*x + n[1]*y + n[2]*z = d
                double[] normal = ParseVector(operand.Substring(0,splitPos));
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
            catch (Exception e)
            {
                throw new ArgumentException("Invalid operand for PLANE", e);
            }
        }

        private static void CutByPlane(Stack operandStack)
        {
            try
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
            catch (Exception e)
            {
                throw new ArgumentException("Invalid operand for CUT_BY_PLANE", e);
            }
        }

    }
}