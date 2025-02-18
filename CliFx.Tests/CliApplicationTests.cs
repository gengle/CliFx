﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CliFx.Services;
using CliFx.Tests.TestCommands;
using FluentAssertions;
using NUnit.Framework;

namespace CliFx.Tests
{
    [TestFixture]
    public class CliApplicationTests
    {
        private const string TestVersionText = "v1.0";

        private static IEnumerable<TestCaseData> GetTestCases_RunAsync()
        {
            yield return new TestCaseData(
                new[] {typeof(HelloWorldDefaultCommand)},
                new string[0],
                "Hello world."
            );
            
            yield return new TestCaseData(
                new[] {typeof(ConcatCommand)},
                new[] {"concat", "-i", "foo", "-i", "bar", "-s", " "},
                "foo bar"
            );
            
            yield return new TestCaseData(
                new[] {typeof(ConcatCommand)},
                new[] {"concat", "-i", "one", "two", "three", "-s", ", "},
                "one, two, three"
            );

            yield return new TestCaseData(
                new[] {typeof(DivideCommand)},
                new[] {"div", "-D", "24", "-d", "8"},
                "3"
            );

            yield return new TestCaseData(
                new[] {typeof(HelloWorldDefaultCommand)},
                new[] {"--version"},
                TestVersionText
            );

            yield return new TestCaseData(
                new[] {typeof(ConcatCommand)},
                new[] {"--version"},
                TestVersionText
            );
            
            yield return new TestCaseData(
                new[] {typeof(HelloWorldDefaultCommand)},
                new[] {"-h"},
                null
            );

            yield return new TestCaseData(
                new[] {typeof(HelloWorldDefaultCommand)},
                new[] {"--help"},
                null
            );
            
            yield return new TestCaseData(
                new[] {typeof(ConcatCommand)},
                new string[0],
                null
            );
            
            yield return new TestCaseData(
                new[] {typeof(ConcatCommand)},
                new[] {"-h"},
                null
            );

            yield return new TestCaseData(
                new[] {typeof(ConcatCommand)},
                new[] {"--help"},
                null
            );
            
            yield return new TestCaseData(
                new[] {typeof(ConcatCommand)},
                new[] {"concat", "-h"},
                null
            );

            yield return new TestCaseData(
                new[] {typeof(ExceptionCommand)},
                new[] {"exc", "-h"},
                null
            );

            yield return new TestCaseData(
                new[] {typeof(CommandExceptionCommand)},
                new[] {"exc", "-h"},
                null
            );

            yield return new TestCaseData(
                new[] {typeof(ConcatCommand)},
                new[] {"[preview]"},
                null
            );

            yield return new TestCaseData(
                new[] {typeof(ExceptionCommand)},
                new[] {"exc", "[preview]"},
                null
            );

            yield return new TestCaseData(
                new[] {typeof(ConcatCommand)},
                new[] {"concat", "[preview]", "-o", "value"},
                null
            );
        }

        private static IEnumerable<TestCaseData> GetTestCases_RunAsync_Negative()
        {
            yield return new TestCaseData(
                new Type[0],
                new string[0],
                null, null
            );

            yield return new TestCaseData(
                new[] {typeof(ConcatCommand)},
                new[] {"non-existing"},
                null, null
            );

            yield return new TestCaseData(
                new[] {typeof(ExceptionCommand)},
                new[] {"exc"},
                null, null
            );

            yield return new TestCaseData(
                new[] {typeof(CommandExceptionCommand)},
                new[] {"exc"},
                null, null
            );

            yield return new TestCaseData(
                new[] {typeof(CommandExceptionCommand)},
                new[] {"exc"},
                null, null
            );
         
            yield return new TestCaseData(
                new[] {typeof(CommandExceptionCommand)},
                new[] {"exc", "-m", "foo bar"},
                "foo bar", null
            );
            
            yield return new TestCaseData(
                new[] {typeof(CommandExceptionCommand)},
                new[] {"exc", "-m", "foo bar", "-c", "666"},
                "foo bar", 666
            );
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases_RunAsync))]
        public async Task RunAsync_Test(IReadOnlyList<Type> commandTypes, IReadOnlyList<string> commandLineArguments,
            string expectedStdOut = null)
        {
            // Arrange
            using (var stdoutStream = new StringWriter())
            {
                var console = new VirtualConsole(stdoutStream);

                var application = new CliApplicationBuilder()
                    .AddCommands(commandTypes)
                    .UseVersionText(TestVersionText)
                    .UseConsole(console)
                    .Build();

                // Act
                var exitCode = await application.RunAsync(commandLineArguments);
                var stdOut = stdoutStream.ToString().Trim();

                // Assert
                exitCode.Should().Be(0);

                if (expectedStdOut != null)
                    stdOut.Should().Be(expectedStdOut);
                else
                    stdOut.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases_RunAsync_Negative))]
        public async Task RunAsync_Negative_Test(IReadOnlyList<Type> commandTypes, IReadOnlyList<string> commandLineArguments,
            string expectedStdErr = null, int? expectedExitCode = null)
        {
            // Arrange
            using (var stderrStream = new StringWriter())
            {
                var console = new VirtualConsole(TextWriter.Null, stderrStream);

                var application = new CliApplicationBuilder()
                    .AddCommands(commandTypes)
                    .UseVersionText(TestVersionText)
                    .UseConsole(console)
                    .Build();

                // Act
                var exitCode = await application.RunAsync(commandLineArguments);
                var stderr = stderrStream.ToString().Trim();

                // Assert
                if (expectedExitCode != null)
                    exitCode.Should().Be(expectedExitCode);
                else
                    exitCode.Should().NotBe(0);
                
                if (expectedStdErr != null)
                    stderr.Should().Be(expectedStdErr);
                else
                    stderr.Should().NotBeNullOrWhiteSpace();
            }
        }
    }
}