using System;
using Xunit;
using AtCoderTemplateForNetCore.Problems;
using System.Collections.Generic;
using System.Linq;

namespace AtCoderTemplateForNetCore.Test
{
    public class AtCoderTester
    {
        [Theory]
        [InlineData(@"", @"")]
        public void ProblemATest(string input, string output)
        {
            var outputs = SplitByNewLine(output);
            IProblem question = new ProblemA();

            var answers = SplitByNewLine(question.Solve(input).Trim());

            Assert.Equal(outputs, answers);
        }

        //[Theory]
        //[InlineData(@"", @"")]
        public void ProblemBTest(string input, string output)
        {
            var outputs = SplitByNewLine(output);
            IProblem question = new ProblemB();

            var answers = SplitByNewLine(question.Solve(input).Trim());

            Assert.Equal(outputs, answers);
        }

        //[Theory]
        //[InlineData(@"", @"")]
        public void ProblemCTest(string input, string output)
        {
            var outputs = SplitByNewLine(output);
            IProblem question = new ProblemC();

            var answers = SplitByNewLine(question.Solve(input).Trim());

            Assert.Equal(outputs, answers);
        }

        //[Theory]
        //[InlineData(@"", @"")]
        public void ProblemDTest(string input, string output)
        {
            var outputs = SplitByNewLine(output);
            IProblem question = new ProblemD();

            var answers = SplitByNewLine(question.Solve(input).Trim());

            Assert.Equal(outputs, answers);
        }

        //[Theory]
        //[InlineData(@"", @"")]
        public void ProblemETest(string input, string output)
        {
            var outputs = SplitByNewLine(output);
            IProblem question = new ProblemE();

            var answers = SplitByNewLine(question.Solve(input).Trim());

            Assert.Equal(outputs, answers);
        }

        //[Theory]
        //[InlineData(@"", @"")]
        public void ProblemFTest(string input, string output)
        {
            var outputs = SplitByNewLine(output);
            IProblem question = new ProblemF();

            var answers = SplitByNewLine(question.Solve(input).Trim());

            Assert.Equal(outputs, answers);
        }

        IEnumerable<string> SplitByNewLine(string input) => input?.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None) ?? new string[0];
    }
}
