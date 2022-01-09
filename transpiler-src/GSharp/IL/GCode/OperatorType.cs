using System.CodeDom;

namespace GSharp.IL.GCode
{
    public static class OperatorType
    {
        public static CodeBinaryOperatorType String2BinaryOperator(string s)
        {
            switch (s)
            {
                case "+":
                return CodeBinaryOperatorType.Add;
                case "-":
                return CodeBinaryOperatorType.Subtract;
                case "*":
                return CodeBinaryOperatorType.Multiply;
                case "/":
                return CodeBinaryOperatorType.Divide;
                case "=":
                return CodeBinaryOperatorType.Assign;
                case "==":
                return CodeBinaryOperatorType.ValueEquality;
                case "&":
                return CodeBinaryOperatorType.BitwiseOr;
                case "|":
                return CodeBinaryOperatorType.BitwiseAnd;
                case "&&":
                return CodeBinaryOperatorType.BooleanAnd;
                case "||":
                return CodeBinaryOperatorType.BooleanOr;
                case ">":
                return CodeBinaryOperatorType.GreaterThan;
                case ">=":
                return CodeBinaryOperatorType.GreaterThanOrEqual;
                case "<":
                return CodeBinaryOperatorType.LessThan;
                case "<=":
                return CodeBinaryOperatorType.LessThanOrEqual;
                case "%":
                return CodeBinaryOperatorType.Modulus;

            }
            return CodeBinaryOperatorType.Add;
        }
    }
}