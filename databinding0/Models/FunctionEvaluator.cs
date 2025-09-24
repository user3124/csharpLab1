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
            // Заменяем запятую на точку
            function = function.Replace(",", ".");
            _expression = NormalizeExpression(function);
        }

        public double Evaluate(double x)
        {
            var expr = new Expression(_expression);
            expr.Parameters["x"] = x;

            expr.EvaluateFunction += (name, args) =>
            {
                double param(int i) => Convert.ToDouble(args.Parameters[i].Evaluate());

                switch (name.ToLower())
                {
                    case "log":
                    case "ln":
                        args.Result = Math.Log(param(0));
                        break;
                    case "log10": 
                        args.Result = Math.Log10(param(0));
                        break;
                    case "sqrt":
                        args.Result = Math.Sqrt(param(0));
                        break;
                    case "sin":
                        args.Result = Math.Sin(param(0));
                        break;
                    case "cos":
                        args.Result = Math.Cos(param(0));
                        break;
                    case "tan":
                        args.Result = Math.Tan(param(0));
                        break;
                    case "abs":
                        args.Result = Math.Abs(param(0));
                        break;
                    case "exp":
                        args.Result = Math.Exp(param(0));
                        break;
                    case "pow":
                        args.Result = Math.Pow(param(0), param(1));
                        break;
                    case "max":
                        args.Result = Math.Max(param(0), param(1));
                        break;
                    case "min":
                        args.Result = Math.Min(param(0), param(1));
                        break;
                    default:
                        throw new ArgumentException($"Неизвестная функция '{name}'");
                }
            };

            object result = expr.Evaluate();
            return Convert.ToDouble(result);
        }

        // Преобразуем операцию ^ в Pow(x,y)
        private static string NormalizeExpression(string input)
        {
            var pattern = @"(\w+)\s*\^\s*(\w+)";
            return Regex.Replace(input, pattern, "Pow($1,$2)");
        }
    }
}
