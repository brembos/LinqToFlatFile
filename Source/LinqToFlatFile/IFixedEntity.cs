namespace LinqToFlatFile
{
    public interface IFixedEntity
    {
        /// <summary>
        ///   Parses the specified input.
        /// </summary>
        /// <param name = "input">The input.</param>
        void ReadLine(string input);

        /// <summary>
        ///   Returns a <see cref = "T:System.String" /> that represents the current <see cref = "T:System.Object" />.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.String" /> that represents the current <see cref = "T:System.Object" />.
        /// </returns>
        string MakeLine();

        /// <summary>
        ///   Makes the header.
        /// </summary>
        /// <returns>
        ///   String
        /// </returns>
        string MakeHeader();

        /// <summary>
        ///   Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        bool IsValid();
    }
}