using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace GenFinderTests
{
    public enum RelevantGenLocationInChunk
    {
        Start,
        Any,
        End,
        Omit
    }

    [TestFixture]
    public class GenFinderUnitTests
    {
        private readonly Random _random = new Random();
        private readonly List<string> _testGenes = new() 
        { 
            "AAAAAAAAAAAGCGCGCTTAGG", 
            "AAAAAAAAAAAGCTTTTTTG",
            "AAAAAAAAAAAGCTTCGCGGGGGGCGTTCCCGGGGTTTTG"
        };

        [OneTimeSetUp]
        public virtual void SetUp()
        {
        }

        [TestCase("TestInputFile", 2048, 20)]
        //[TestCase("TestInputFile", 1024, 10)]
        public void Generate_Test_DnaFile(string fileName, int chunkSize, int chunksNumber)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + fileName + ".dat";
            File.Delete(path);
            var usedGenesList = new List<string>();
            using (StreamWriter sr = File.AppendText(path))
            {
                for (var i = 0; i < chunksNumber; i++)
                {
                    var gen = _testGenes[_random.Next(_testGenes.Count)];
                    var tempChunk = "";
                    switch (i)
                    {
                        case 3:
                            tempChunk = GenerateRandomDnaChunk(chunkSize, gen, false, 
                                RelevantGenLocationInChunk.Start);
                            usedGenesList.Add(gen);
                            break;
                        case 5:
                            tempChunk = GenerateRandomDnaChunk(chunkSize, gen, false, 
                                RelevantGenLocationInChunk.Any);
                            usedGenesList.Add(gen);
                            break;
                        case 9:
                            tempChunk = GenerateRandomDnaChunk(chunkSize, gen, false, 
                                RelevantGenLocationInChunk.End);
                            usedGenesList.Add(gen);
                            break;
                        default:
                            tempChunk = GenerateRandomDnaChunk(chunkSize, gen);
                            break;
                    }
                    sr.Write(tempChunk);
                }
                sr.Close();
            }

            var fileContent = File.ReadAllText(path);

            foreach (var genFromList in usedGenesList)
            {
                Assert.IsTrue(fileContent.Contains(genFromList), $"not found gen: {genFromList}");
            }

        }

        private string GenerateRandomDnaChunk(int chunkSize, 
            string gen, 
            bool randomGenLocation = false,
            RelevantGenLocationInChunk locationInChunk = RelevantGenLocationInChunk.Omit)
        {
            const string chars = "GCAT";
            var randomDnaChunk = "";
            var relevantGenLocationValues = Enum.GetValues(typeof(RelevantGenLocationInChunk));

            // Generate chunk of requested length
            var tempChunk = new string(Enumerable.Repeat(chars, chunkSize)
                .Select(s => s[_random.Next(s.Length)]).ToArray());

            RelevantGenLocationInChunk relevantGenLocation = locationInChunk;
            if (randomGenLocation == true)
            {
                relevantGenLocation =
                    (RelevantGenLocationInChunk)relevantGenLocationValues.GetValue(_random.Next(relevantGenLocationValues.Length));
            }

            // Add/omit relevant gen to chunk
            switch (relevantGenLocation)
            {
                case RelevantGenLocationInChunk.Start:
                    randomDnaChunk = gen + tempChunk.Substring(gen.Length, tempChunk.Length - gen.Length);
                    break;
                case RelevantGenLocationInChunk.Any:
                    var anyLocationIndex = _random.Next(chunkSize);
                    randomDnaChunk = tempChunk.Substring(0, anyLocationIndex) + 
                                     gen;
                    if (randomDnaChunk.Length < chunkSize)
                    {
                        randomDnaChunk = randomDnaChunk + tempChunk.Substring(anyLocationIndex + gen.Length);
                    }
                    randomDnaChunk = randomDnaChunk.Substring(0, chunkSize);
                    break;
                case RelevantGenLocationInChunk.End:
                    randomDnaChunk = tempChunk.Substring(0, tempChunk.Length - gen.Length) + gen;
                    break;
                case RelevantGenLocationInChunk.Omit:
                    randomDnaChunk = tempChunk;
                    break;
            }

            return randomDnaChunk;
        }

        private string GenerateRandomChunk(int chunkSize)
        {
            const string chars = "GCAT";
            var randomDnaChunk = "";
            var relevantGenLocationValues = Enum.GetValues(typeof(RelevantGenLocationInChunk));

            // Generate chunk of requested length
            var tempChunk = new string(Enumerable.Repeat(chars, chunkSize)
                .Select(s => s[_random.Next(s.Length)]).ToArray());

            return randomDnaChunk;
        }
    }
}
