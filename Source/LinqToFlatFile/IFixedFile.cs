using System.Collections.Generic;
using System.IO;

namespace LinqToFlatFile
{
    public interface IFixedFile
    {
        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name = "headerRow">if set to <c>true</c> file contains [header row] (skip header==true).</param>
        /// <returns>
        /// IEnumerable&lt;TEntity&gt;
        /// </returns>
        IEnumerable<TEntity> ReadFile<TEntity>(Stream stream, bool headerRow) where TEntity : IFixedEntity, new();

        /// <summary>
        /// Writes the file to the returned stream.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="headerRow">if set to <c>true</c> properties name are set as [header row].</param>
        /// <returns>
        /// Stream
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        Stream WriteFile<TEntity>(IEnumerable<TEntity> collection, bool headerRow) where TEntity : IFixedEntity, new();
    }
}