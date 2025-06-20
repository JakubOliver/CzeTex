using Xunit;
using CzeTex;

using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace CzeTex.Tests
{
    public class UnitTest2
    {
        public static string startOfScript = "run --project ../CzeTex/CzeTex.csproj ";
        public static string textExamples = "../CzeTex.Tests/examples/";

        private Process RunScript(string input, string setup = "")
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"{startOfScript} {input} {setup}",
                    WorkingDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../../../CzeTex")),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            string error = proc.StandardError.ReadToEnd();

            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                System.Console.WriteLine(error);
            }

            return proc;
        }

        [Theory]
        [InlineData("examples/formulasForVolume.txt")]
        [InlineData("examples/principMaximality.txt")]
        [InlineData("examples/recipe.txt")]
        [InlineData("examples/review.txt")]
        [InlineData("../docs/examples.txt")]
        [InlineData("../docs/user.txt")]
        [InlineData("../docs/programmer.txt")]
        [InlineData("../CzeTex.Tests/examples/lorem.txt")]
        [InlineData("../CzeTex.Tests/examples/lorem2.txt")]
        [InlineData("../CzeTex.Tests/examples/list.txt")]
        [InlineData("../CzeTex.Tests/examples/math.txt")]
        [InlineData("../CzeTex.Tests/examples/oldReview.txt")]
        [InlineData("../CzeTex.Tests/examples/combinations.txt")]
        [InlineData("../CzeTex.Tests/examples/link.txt")]
        [InlineData("../CzeTex.Tests/examples/math2.txt")]
        public void OnlyInput(string input)
        {
            Process proc = this.RunScript(input);
            Assert.Equal(0, proc.ExitCode);
        }

        [Fact]
        public void InputAndSetup()
        {
            Process proc = this.RunScript("examples/proNastaveni.txt", "examples/nastaveni.json");
            Assert.Equal(0, proc.ExitCode);
        }

        [Fact]
        public void InvalidCzeTexFunction()
        {
            Process proc = this.RunScript($"{textExamples}wrongCzeTexFunction.txt");
            Assert.Equal(134, proc.ExitCode); //TrieException
        }

        [Fact]
        public void InvalidJson()
        {
            Process proc = this.RunScript($"{textExamples}wrongCzeTexFunction.txt {textExamples}spatnySetup.json");
            Assert.Equal(134, proc.ExitCode); //JSONLoaderException
        }

        [Fact]
        public void InvalidParameter()
        {
            Process proc = this.RunScript($"{textExamples}invalidParameter.txt");
            Assert.Equal(134, proc.ExitCode); //InvalidParameterException
        }

        [Fact]
        public void NotGetFunction()
        {
            Process proc = this.RunScript($"{textExamples}getFunction.txt");
            Assert.Equal(134, proc.ExitCode); //InvalidParameterException
        }

        [Fact]
        public void NoSection()
        {
            Process proc = this.RunScript($"{textExamples}noSection.txt");
            Assert.Equal(134, proc.ExitCode); //No active section
        }

        [Fact]
        public void InvalidListStructure()
        {
            Process proc = this.RunScript($"{textExamples}listItemNoList.txt");
            Assert.Equal(134, proc.ExitCode);
        }

        [Fact]
        public void InvalidJsonSign()
        {
            Process proc = this.RunScript($"{textExamples}dobre.txt {textExamples}noSign.json");
            Assert.Equal(134, proc.ExitCode);
        }

        [Fact]
        public void InvalidGTitleParameter()
        {
            Process proc = this.RunScript($"{textExamples}invalidGTitle.txt");
            Assert.Equal(134, proc.ExitCode);
        }

        [Fact]
        public void RemovingFromEmpty()
        {
            Process proc = this.RunScript($"{textExamples}removingFromEmpty.txt");
            Assert.Equal(134, proc.ExitCode);
        }

        [Fact]
        public void ParagraphIsNotSet()
        {
            Process proc = this.RunScript($"{textExamples}ParagraphIsNotSet.txt");
            Assert.Equal(134, proc.ExitCode);
        }

        [Fact]
        public void IsFunction()
        {
            Assert.True(StringFunctions.IsFunction("/title"));
        }

        [Fact]
        public void GetFunction()
        {
            Assert.Equal("title", StringFunctions.GetFunctionName("/title"));
        }
    }
}
