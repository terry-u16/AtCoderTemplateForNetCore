using System;
using System.Collections.Generic;
using AtCoderTemplateForNetCore.Problems;
using Xunit;

namespace AtCoderTemplateForNetCore.Test
{
    class BasicJudge : IJudge
    {
        IProblem _solver { get; }

        public BasicJudge(IProblem solver)
        {
            _solver = solver;
        }

        public void Judge(string input, string output)
        {
            var outputs = SplitByNewLine(output);
            var answers = SplitByNewLine(_solver.Solve(input).Trim());
            Assert.Equal(outputs, answers);
        }

        IEnumerable<string> SplitByNewLine(string input) => input?.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None) ?? new string[0];
    }
}
