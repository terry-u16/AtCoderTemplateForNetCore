using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;

namespace AtCoderTemplateForNetCore.Test
{
    public class AtCoderTester
    {
        [Theory]
        [InlineData("", "")]
        public void QuestionATest(string input, string output)
        {
            IAtCoderQuestion question = new QuestionA();
            var answer = question.Solve(input);
            Assert.Equal(output, answer);
        }

        //[Theory]
        //[InlineData("", "")] 
        public void QuestionBTest(string input, string output)
        {
            IAtCoderQuestion question = new QuestionB();
            var answer = question.Solve(input);
            Assert.Equal(output, answer);
        }

        //[Theory]
        //[InlineData("", "")] 
        public void QuestionCTest(string input, string output)
        {
            IAtCoderQuestion question = new QuestionC();
            var answer = question.Solve(input);
            Assert.Equal(output, answer);
        }

        //[Theory]
        //[InlineData("", "")] 
        public void QuestionDTest(string input, string output)
        {
            IAtCoderQuestion question = new QuestionD();
            var answer = question.Solve(input);
            Assert.Equal(output, answer);
        }

        //[Theory]
        //[InlineData("", "")] 
        public void QuestionETest(string input, string output)
        {
            IAtCoderQuestion question = new QuestionE();
            var answer = question.Solve(input);
            Assert.Equal(output, answer);
        }
        //[Theory]
        //[InlineData("", "")] 
        public void QuestionFTest(string input, string output)
        {
            IAtCoderQuestion question = new QuestionF();
            var answer = question.Solve(input);
            Assert.Equal(output, answer);
        }
    }
}
