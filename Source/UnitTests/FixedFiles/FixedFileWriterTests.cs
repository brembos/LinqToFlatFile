﻿using LinqToFlatFile;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace UnitTests.FixedFiles
{
    [TestFixture()]
    public class FixedFileWriterTests
    {
        [Test()]
        public void WriteLineTest()
        {
            var person = new Person() { Id = 123, Name = "Elvis Presley" };
            var writer = new FixedFileWriter<Person>();
            var line = writer.MakeLine(person);
            Assert.AreEqual("00123Elvis Presley       ", line);
        }
    }
}