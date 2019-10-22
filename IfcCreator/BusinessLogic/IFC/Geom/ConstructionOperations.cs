using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        INTERSECTION
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
                    break;
                case OperationName.REVOLVE:
                    break;
                case OperationName.TRANSLATION:
                    break;
                case OperationName.ROTATION:
                    break;
                case OperationName.UNION:
                    break;
                case OperationName.DIFFERENCE:
                    break;
                case OperationName.INTERSECTION:
                    break;
                default:
                    break;
            }
        }

        private static void PolygonShape(Stack operandStack)
        {
            string operand = (string) operandStack.Pop();
            //remove all spaces
            operand.Replace(" ", "");
            if ((operand.Length < 19) || !operand.StartsWith("[[[") || !operand.EndsWith("]]]"))
            {   //operand should represent a three dimensional array
                throw new ArgumentException(string.Format("Invalid operand for POLYGON_SHAPE: {0}", operand));
            }

            try
            {
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
                throw new ArgumentException(string.Format("Invalid operand for POLYGON_SHAPE: {0}", operand), e);
            }
        }
    }
}