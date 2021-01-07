using System;
using Xunit;
using AtCoderTemplateForNetCore.Problems;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AtCoderTemplateForNetCore.Test
{
    public class AtCoderTester
    {
        //[Theory]
        //[InlineData(@"", @"")]
        public void ProblemATest(string input, string output)
        {
            IProblem solver = CreateInstanceOf(MethodBase.GetCurrentMethod());
            IJudge judge = new BasicJudge(solver);
            judge.Judge(input, output);
        }

        //[Theory]
        //[InlineData(@"", @"")]
        public void ProblemBTest(string input, string output)
        {
            IProblem solver = CreateInstanceOf(MethodBase.GetCurrentMethod());
            IJudge judge = new BasicJudge(solver);
            judge.Judge(input, output);
        }

        //[Theory]
        //[InlineData(@"", @"")]
        public void ProblemCTest(string input, string output)
        {
            IProblem solver = CreateInstanceOf(MethodBase.GetCurrentMethod());
            IJudge judge = new BasicJudge(solver);
            judge.Judge(input, output);
        }

        //[Theory]
        //[InlineData(@"", @"")]
        public void ProblemDTest(string input, string output)
        {
            IProblem solver = CreateInstanceOf(MethodBase.GetCurrentMethod());
            IJudge judge = new BasicJudge(solver);
            judge.Judge(input, output);
        }

        //[Theory]
        //[InlineData(@"", @"")]
        public void ProblemETest(string input, string output)
        {
            IProblem solver = CreateInstanceOf(MethodBase.GetCurrentMethod());
            IJudge judge = new BasicJudge(solver);
            judge.Judge(input, output);
        }

        //[Theory]
        //[InlineData(@"", @"")]
        public void ProblemFTest(string input, string output)
        {
            IProblem solver = CreateInstanceOf(MethodBase.GetCurrentMethod());
            IJudge judge = new BasicJudge(solver);
            judge.Judge(input, output);
        }

        private static IProblem CreateInstanceOf(MethodBase method)
        {
            var type = typeof(IProblem);
            var nameSpace = type.Namespace;
            var className = method.Name.Replace("Test", "");

            var judgeType = Type.GetType(nameSpace + "." + className + ", " + type.Assembly.FullName);
            return Activator.CreateInstance(judgeType) as IProblem;
        }
    }
}
