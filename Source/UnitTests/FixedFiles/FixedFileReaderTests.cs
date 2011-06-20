using LinqToFlatFile;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace UnitTests.FixedFiles
{
    [TestFixture()]
    public class FixedFileReaderTests
    {
        [Test()]
        public void ReadLineTest()
        {
            var writer = new FixedFileReader<Person>();
            var line = writer.ReadLine("00123Elvis Presley       ");
            Assert.AreEqual(line.Id, 123);
            Assert.AreEqual(line.Name, "Elvis Presley");
        }
    }
}