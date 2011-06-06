using System;

namespace LinqToFlatFile
{
    ///<summary>
    ///  Represents a class for custom attributes.
    ///</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class TabPositionAttribute : Attribute
    {
        public TabPositionAttribute(int index)
        {
            if (index >= 0)
            {
                Index = index;
            }
            else
            {
                throw new ArgumentException("Index must be greater than zero.");
            }
        }

        public TabPositionAttribute(int index, bool mandatory)
        {
            if (index >= 0)
            {
                Index = index;
            }
            else
            {
                throw new ArgumentException("Index must be greater than zero.");
            }
            Mandatory = mandatory;
        }

        public int Index { get; private set; }

        public bool Mandatory { get; private set; }
    }
}