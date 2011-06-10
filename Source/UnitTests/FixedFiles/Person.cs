using LinqToFlatFile;

namespace UnitTests.FixedFiles
{
    public class Person
    {
        [FixedPosition(0, 4)]
        public int Id { get; set; }

        [FixedPosition(5, 24)]
        public string Name { get; set; }
    }
}
