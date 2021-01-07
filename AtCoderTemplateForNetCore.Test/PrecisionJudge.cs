using System;
using System.Collections.Generic;
using System.Text;
using AtCoderTemplateForNetCore.Problems;
using Xunit;

namespace AtCoderTemplateForNetCore.Test
{
    class PrecisionJudge : IJudge
    {
        readonly IProblem _solver;
        readonly int _precision;

        public PrecisionJudge(IProblem solver, int precision = 6)
        {
            _solver = solver;
            _precision = precision;
        }

        public void Judge(string input, string output)
        {
            var answer = _solver.Solve(input);
            Assert.Equal(double.Parse(output.Trim()), double.Parse(answer.Trim()), _precision);
        }
    }
}
