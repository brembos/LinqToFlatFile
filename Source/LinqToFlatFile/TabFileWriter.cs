using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinqToFlatFile
{
    public class TabFileWriter<TEntity> : IFileWriter<TEntity> where TEntity : new()
    {
        public Stream WriteFile(IEnumerable<TEntity> collection, bool headerRow)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            var memoryStream = new MemoryStream();
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var streamWriter = new StreamWriter(memoryStream, encoding);
            if (headerRow)
            {
                var header = new TEntity();
                streamWriter.WriteLine(MakeHeader(header));
            }
            foreach (var line in collection)
            {
                streamWriter.WriteLine(MakeLine(line));
            }
            streamWriter.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }



        public string MakeLine(TEntity entity)
        {
            var values = new SortedDictionary<int, string>();
            foreach (PropertyInfo property in entity.GetType().GetProperties())
            {
                foreach (TabPositionAttribute fixedFileAttribute in property.GetCustomAttributes(typeof(TabPositionAttribute), false))
                {
                    if (fixedFileAttribute != null)
                    {
                        object theValue = property.GetValue(entity, null);
                        string propertyValue = theValue != null ? theValue.ToString() : string.Empty;


                        switch (property.PropertyType.ToString())
                        {
                            //    case "System.Int32":
                            //    case "System.Int16":
                            //        propertyValue = propertyValue.PadLeft(width, _paddingNumber);
                            //        break;
                            //    case "System.Decimal":
                            //        var MyCultureInfo = new CultureInfo("en-US");
                            //        propertyValue = propertyValue.ToString(MyCultureInfo).Replace(",", "").PadLeft(width, _paddingNumber);
                            //        break;
                            case "System.DateTime":
                                //<example>20070228</example>yyyymmdd
                                DateTime dateTime = DateTime.Parse(propertyValue, new CultureInfo("nb-NO"));
                                if (!dateTime.Equals(DateTime.MinValue))
                                    propertyValue = dateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                                else
                                    propertyValue = "";
                                break;
                            //    case "System.Boolean":
                            //        propertyValue = propertyValue == "True" ? "J" : "N";
                            //        break;
                        }

                        int index = fixedFileAttribute.Index;
                        values.Add(index, propertyValue);
                    }
                    break;
                }
            }
            return values.Values.Aggregate<string, string>(null, (current, value) => string.IsNullOrEmpty(current) ? value : current + "\t" + value);
        }

        public string MakeHeader(TEntity entity)
        {
            VerifyAttributes(entity);
            var values = new SortedDictionary<int, string>();
            foreach (PropertyInfo property in entity.GetType().GetProperties())
            {
                foreach (TabPositionAttribute fixedFileAttribute in property.GetCustomAttributes(typeof(TabPositionAttribute), false))
                {
                    if (fixedFileAttribute != null)
                    {
                        string name = property.Name;
                        int index = fixedFileAttribute.Index;
                        values.Add(index, name);
                    }
                    break;
                }
            }
            return values.Values.Aggregate<string, string>(null, (current, value) => string.IsNullOrEmpty(current) ? value : current + "\t" + value);
        }

        private void VerifyAttributes(TEntity entity)
        {
            bool noAttributes;
            foreach (var propertyInfo in entity.GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttributes(typeof(TabPositionAttribute), false).Count() > 0)
                {
                    return;
                }
            }
            throw new AttributeException();
        }
    }
}