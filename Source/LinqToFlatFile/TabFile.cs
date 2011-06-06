using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace LinqToFlatFile
{
    public class TabFile : IFixedFile
    {
        #region IFixedFile Members

        public IEnumerable<TEntity> ReadFile<TEntity>(Stream stream, bool headerRow) where TEntity : IFixedEntity, new()
        {
            using (var reader = new StreamReader(stream))
            {
                string line;
                if (headerRow)
                {
                    //skip first row
                    reader.ReadLine();
                }
                while ((line = reader.ReadLine()) != null)
                {
                    var item = new TEntity();
                    item.ReadLine(line);
                    yield return item;
                }
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"),
         SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public Stream WriteFile<TEntity>(IEnumerable<TEntity> collection, bool headerRow) where TEntity : IFixedEntity, new()
        {
            if (collection == null) throw new ArgumentNullException("collection");
            var memoryStream = new MemoryStream();
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var streamWriter = new StreamWriter(memoryStream, encoding);
            if (headerRow)
            {
                var header = new TEntity();
                streamWriter.WriteLine(header.MakeHeader());
            }
            foreach (var line in collection)
            {
                streamWriter.WriteLine(line.MakeLine());
            }
            streamWriter.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        #endregion
    }
}