using System;
using System.IO;
using AtCoderTemplateForNetCore;
using AtCoderTemplateForNetCore.Problems;

namespace StressTester
{
    class Program
    {
        static void Main(string[] args)
        {
            IProblem question = new ProblemA();
            IProblem naiveSolver = new NaiveSolver();

            long count = 0;

            while (true)
            {
                using var input = new MemoryStream();
                using var expectedOutput = new MemoryStream();
                using var actualOutput = new MemoryStream();
                var expectedIO = new IOManager(input, expectedOutput);
                var actualIO = new IOManager(input, actualOutput);
                var inputReader = new StreamReader(input);
                var inputWriter = new StreamWriter(input);
                var expectedOutputReader = new StreamReader(expectedOutput);
                var actualOutputReader = new StreamReader(actualOutput);

                // 入力作成
                GenerateInput(inputWriter);
                inputWriter.Flush();

                // チェック
                input.Position = 0;
                naiveSolver.Solve(expectedIO);
                expectedIO.Flush();
                input.Position = 0;
                question.Solve(actualIO);
                actualIO.Flush();

                input.Position = 0;
                expectedOutput.Position = 0;
                actualOutput.Position = 0;

                Console.WriteLine($"[Test {count++}]");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("[Input]");
                Console.ResetColor();
                Console.Write(inputReader.ReadToEnd());

                var expected = expectedOutputReader.ReadToEnd();
                var actual = actualOutputReader.ReadToEnd();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("[Expected]");
                Console.ResetColor();
                Console.Write(expected);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("[Actual]");
                Console.ResetColor();
                Console.Write(actual);

                if (expected == actual)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Accepted");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Wrong Answer");
                    Console.ResetColor();
                    Console.WriteLine();
                    break;
                }
            }
        }

        private static void GenerateInput(StreamWriter writer)
        {
            var random = new Random();

            // 入力を作成
        }
    }

    class NaiveSolver : ProblemBase
    {
        public NaiveSolver() : base(false) { }

        protected override void SolveEach(IOManager io)
        {
            // 愚直解を記述
        }
    }
}
