using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using BuildingSmart.IFC.IfcGeometryResource;

namespace IfcCreator.Ifc.Geom
{

    static class RepresentationParser
    {
        public static IfcRepresentationItem ParseConstructionString(string expression)
        {
            Stack<OperationName> operationStack = new Stack<OperationName>();
            Stack operandStack = new Stack();
            StringBuilder stringBuffer = new StringBuilder();
            for (int i=0; i < expression.Length; ++i)
            {
                switch (expression[i])
                {
                    case '(':
                        // parenthesis are opened after operation
                        operationStack.Push((OperationName) Enum.Parse(typeof(OperationName), stringBuffer.ToString()));
                        stringBuffer.Clear();
                        break;
                    case ')':
                        // parenthesis are closed after operand
                        operandStack.Push(stringBuffer.ToString());
                        stringBuffer.Clear();
                        ConstructionOperations.ExecuteOperation(operationStack.Pop(), operandStack);
                        break;
                    case ' ':
                        // Remove spaces
                        break;
                    default:
                        stringBuffer.Append(expression[i]);
                        break;
                }
            }

            if (operandStack.Count != 1)
            {   // operand stack should contain the result
                throw new ArgumentException(string.Format("Could not parse geometric representation expression: {0}", expression));
            }
            return (IfcRepresentationItem) operandStack.Pop();
        }
    }
}