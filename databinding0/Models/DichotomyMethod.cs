using System;

namespace databinding0.Models
{
    public class DichotomyMethod
    {
        private readonly FunctionEvaluator _evaluator;

        public DichotomyMethod(string function)
        {
            _evaluator = new FunctionEvaluator(function);
        }

        public double Solve(double a, double b, double eps, out int iterations)
        {
            iterations = 0;

            double fa = _evaluator.Evaluate(a);
            double fb = _evaluator.Evaluate(b);

            if (fa * fb > 0)
                throw new Exception("На интервале нет гарантированного корня (f(a) и f(b) одного знака)");

            while (Math.Abs(b - a) > eps)
            {
                iterations++;

                double x = (a + b) / 2.0;
                double fx = _evaluator.Evaluate(x);

                if (fa * fx <= 0)
                {
                    b = x;
                    fb = fx;
                }
                else
                {
                    a = x;
                    fa = fx;
                }
            }

            return (a + b) / 2.0;
        }
    }
}
