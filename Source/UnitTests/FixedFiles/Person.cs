using LinqToFlatFile;

namespace UnitTests.FixedFiles
{
    public class Person
    {
        [FixedPosition(0, 4)]
        [TabPosition(0)]
        public int Id { get; set; }

        [FixedPosition(5, 24)]
        [TabPosition(1)]
        public string Name { get; set; }
    }
}
