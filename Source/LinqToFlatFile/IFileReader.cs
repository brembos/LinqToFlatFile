using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace LinqToFlatFile
{
    public interface IFileReader<out TEntity> where TEntity : new()
    {
        IEnumerable<TEntity> ReadFile(Stream stream, bool headerRow);
        TEntity ReadLine(string line);
    }
}