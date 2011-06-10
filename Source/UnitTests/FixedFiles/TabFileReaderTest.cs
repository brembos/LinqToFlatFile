using LinqToFlatFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.FixedFiles
{
    [TestClass()]
    public class FixedPositionWriterTests
    {
        [TestMethod()]
        public void WriteLineTest()
        {
            var person = new Person() {Id = 123, Name = "Elvis Presley"};
            var writer = new LinqToFlatFile.FixedFileWriter<Person>();
            var line = writer.MakeLine(person);
            Assert.AreEqual("00123Elvis Presley       ",line);
        }
    }
}
