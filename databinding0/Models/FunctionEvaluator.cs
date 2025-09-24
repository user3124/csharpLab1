using System;
using System.Text.RegularExpressions;
using NCalc;

namespace databinding0.Models
{
    public class FunctionEvaluator
    {
        private readonly string _expression;

        public FunctionEvaluator(string function)
        {
            _expression = NormalizeExpression(function);
        }

        public double Evaluate(double x)
        {
            var expr = new Expression(_expression);
            expr.Parameters["x"] = x;

            object result = expr.Evaluate();
            return Convert.ToDouble(result);
        }

        private static string NormalizeExpression(string input)
        {
            var pattern = @"(\w+)\s*\^\s*(\w+)";
            return Regex.Replace(input, pattern, "Pow($1,$2)");
        }
    }
}
