using LinqToFlatFile;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace UnitTests.FixedFiles
{
    [TestFixture()]
    public class TabFileWriterTests
    {
        [Test()]
        public void EncodingPersonToFixedStringPasses()
        {
            var person = new Person() { Id = 123, Name = "Elvis Presley" };
            var writer = new TabFileWriter<Person>();
            var line = writer.MakeLine(person);
            Assert.AreEqual("123\tElvis Presley", line);
        }
        [Test()]
        public void EncodingPersonHeaderToFixedStringPasses()
        {
            var person = new Person() { Id = 123, Name = "Elvis Presley" };
            var writer = new TabFileWriter<Person>();
            var line = writer.MakeHeader(person);
            Assert.AreEqual("Id\tName", line);
        }

        [Test()]
        [ExpectedException(typeof(AttributeException))]
        public void EncodingEntityWithoutAttributeFailes()
        {
            var person = new PersonWithoutAttributes() { Id = 123, Name = "Elvis Presley" };
            var writer = new TabFileWriter<PersonWithoutAttributes>();
            var line = writer.MakeHeader(person);
        }
    }
}