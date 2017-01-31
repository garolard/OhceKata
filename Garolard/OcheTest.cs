using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace OhceKata.Garolard
{
    public class OcheTest
    {
        [Fact]
        public void ShouldSaluteOnStart()
        {
            OcheTestHelper.SetUpInputBuffer();
            OcheTestHelper.SetUpOutputBuffer();
            var oche = new Oche(OcheTestHelper.GetStartInputReader(), OcheTestHelper.GetCumulativeOutputWriter());

            oche.NextInput();

            Assert.Equal(true, oche.IsStartInput());
            OcheTestHelper.OutputBuffer[0].Should().Be("Hola Gabriel");
        }

        [Fact]
        public void ShouldGoodByeOnStop()
        {
            OcheTestHelper.SetUpInputBuffer();
            OcheTestHelper.SetUpOutputBuffer();
            var oche = new Oche(OcheTestHelper.GetStartAndStopInputReader(), OcheTestHelper.GetCumulativeOutputWriter());

            oche.NextInput();
            oche.IsStartInput().Should().BeTrue();

            oche.NextInput();
            oche.IsStopInput().Should().BeTrue();
            OcheTestHelper.OutputBuffer[1].Should().Be("Adios Gabriel");
        }

        [Fact]
        public void ShouldReverseSingleInput()
        {
            OcheTestHelper.SetUpInputBuffer();
            OcheTestHelper.SetUpOutputBuffer();
            var oche = new Oche(OcheTestHelper.GetDogInputReader(), OcheTestHelper.GetCumulativeOutputWriter());

            oche.NextInput();
            oche.Reverse();

            OcheTestHelper.OutputBuffer.Should().HaveCount(1);
            OcheTestHelper.OutputBuffer[0].Should().Be("orrep");
        }

        [Fact]
        public void ShouldBeGladWithPalindrome()
        {
            OcheTestHelper.SetUpInputBuffer();
            OcheTestHelper.SetUpOutputBuffer();
            var oche = new Oche(OcheTestHelper.GetPalindromeInputReader(), OcheTestHelper.GetCumulativeOutputWriter());

            oche.NextInput();
            oche.Reverse();

            OcheTestHelper.OutputBuffer.Should().HaveCount(2);
            OcheTestHelper.OutputBuffer[0].Should().Be("oso");
            OcheTestHelper.OutputBuffer[1].Should().Be("Bonita palabra!");
        }
    }

    // IMPLEMENTACIÓN OHCE
    public class Oche : IOche
    {
        private string lastInput;
        private string name;

        private readonly Func<string> readInput;
        private readonly Action<string> writeOutput;

        public Oche(Func<string> readInput, Action<string> writeOutput)
        {
            this.readInput = readInput;
            this.writeOutput = writeOutput;
        }

        public void NextInput()
        {
            lastInput = readInput();
        }

        public bool IsStartInput()
        {
            if (lastInput.StartsWith("ohce") && lastInput.Split(' ').Length == 2)
            {
                name = lastInput.Split(' ')[1];
                writeOutput("Hola " + name);
                return true;
            }
            return false;
        }

        public bool IsStopInput()
        {
            if (lastInput == "Stop!")
            {
                writeOutput("Adios " + name);
                return true;
            }

            return false;
        }

        public void Reverse()
        {
            var charArray = lastInput.ToCharArray();
            Array.Reverse(charArray);
            var result = new string(charArray);

            writeOutput(result);
            if (result == lastInput)
            {
                writeOutput("Bonita palabra!");
            }
        }
    }

    // HELPERS PARA TEST
    internal static class OcheTestHelper
    {
        static List<string> InputBuffer = new List<string>() { "ohce Gabriel", "Stop!" };
        public static List<string> OutputBuffer = new List<string>();

        public static void SetUpInputBuffer()
        {
            InputBuffer = new List<string>() { "ohce Gabriel", "Stop!" };
        }

        public static void SetUpOutputBuffer()
        {
            OutputBuffer = new List<string>();
        }

        public static Func<string> GetDummyInputReader()
        {
            return () => "";
        }

        public static Action<string> GetDummyOutputWriter()
        {
            return (s) => { return; };
        }

        public static Func<string> GetStartInputReader()
        {
            return () => "ohce Gabriel";
        }
        
        public static Func<string> GetStartAndStopInputReader()
        {
            return () =>
            {
                var value = InputBuffer.FirstOrDefault();
                InputBuffer = InputBuffer.Skip(1).ToList();
                return value;
            };
        }

        public static Func<string> GetDogInputReader()
        {
            return () => "perro";
        }

        public static Func<string> GetPalindromeInputReader()
        {
            return () => "oso";
        }

        public static Action<string> GetCumulativeOutputWriter()
        {
            return (s) => OutputBuffer.Add(s);
        }
    }
}
