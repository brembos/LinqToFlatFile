using System.Collections.Generic;
using System.IO;

namespace LinqToFlatFile
{
    public interface IFileWriter<in TEntity> where TEntity :  new()
    {
        Stream WriteFile(IEnumerable<TEntity> collection, bool headerRow);
        string MakeLine(TEntity entity);
        string MakeHeader(TEntity entity);
    }
}