using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NCalc;

namespace InkMath
{
    class MathProblem
    {
        public enum Operators {
            Multiplication = '*',
            Division       = '/',
            Addition       = '+',
            Subtraction    = '-'
        };

        public List<int> terms = new List<int>();
        public string equation;

        public MathProblem(Operators operation)
        {
            equation = "";

            Random rnd = new Random();

            int numTerms = 2;
            for (int i = 0; i < numTerms; i++)
                equation += String.Format("{0} {1} ",
                    rnd.Next(0, 10),
                    ((char)operation).ToString());

            equation = equation.Substring(0, equation.Length - 3);
        }

        public bool Solve(string solution)
        {
            if (!ValidateNumber(solution))
                return false;

            return (Solve(int.Parse(solution as string)));
        }

        public bool Solve(int solution)
        {
            Expression e = new Expression(equation);
            return solution == (int)e.Evaluate();
        }

        public bool Solve(float solution)
        {
            Expression e = new Expression(equation);
            return solution == (float)e.Evaluate();
        }

        public bool ValidateNumber(string input)
        {
            return (Regex.IsMatch(input, @"^[-+]?[0-9]*\.?[0-9]+$"));
        }
    }
}
