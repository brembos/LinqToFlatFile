using System;

namespace LinqToFlatFile
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class FixedPositionAttribute : Attribute
    {
        public FixedPositionAttribute(int startPosition, int endPosition)
        {
            if (startPosition >= 0 && endPosition >= startPosition)
            {
                StartPosition = startPosition;
                EndPosition = endPosition;
            }
            else
            {
                throw new ArgumentException(
                    "StartPosition must be >= 0 and EndPosition must be greater than or equal to StartPosition");
            }
        }

        public int StartPosition { get; private set; }

        public int EndPosition { get; private set; }
    }
}