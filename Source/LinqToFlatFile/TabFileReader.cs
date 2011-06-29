using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace LinqToFlatFile
{
    public class TabFileReader<TEntity> : IFileReader<TEntity> where TEntity : new()
    {
        public IEnumerable<TEntity> ReadFile(Stream stream, bool headerRow)
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
                    var item = ReadLine(line);
                    yield return item;
                }
            }
        }

        public TEntity ReadLine(string line)
        {
            var entity = new TEntity();
            if (!String.IsNullOrEmpty(line))
            {
                var array = line.Split('\t');
                foreach (PropertyInfo property in entity.GetType().GetProperties())
                {
                    foreach (
                        TabPositionAttribute attribute in
                            property.GetCustomAttributes(typeof(TabPositionAttribute), false))
                    {
                        if (attribute != null)
                        {
                            var index = attribute.Index;
                            string substring = array[index].Trim();
                            try
                            {
                                object theValue = null;
                                switch (property.PropertyType.ToString())
                                {
                                    case "System.Boolean":
                                        bool outbool;
                                        //Try first ordinary bool parse
                                        if (!Boolean.TryParse(substring, out outbool))
                                        {
                                            outbool = false;
                                            // substring.ToUpperInvariant() == "J";//else J = Ja (true) N=Nei (false).
                                        }
                                        theValue = outbool;
                                        break;
                                    case "System.Int32":
                                        theValue = Int32.Parse(substring, CultureInfo.InvariantCulture);
                                        break;
                                    case "System.Int16":
                                        theValue = Int16.Parse(substring, CultureInfo.InvariantCulture);
                                        break;
                                    case "System.DateTime":
                                        //<example>YYYYMMDD</example>
                                        try
                                        {
                                            if (!substring.Equals("00000000") && !string.IsNullOrEmpty(substring))
                                            {
                                                int year = Int32.Parse(substring.Substring(0, 4),
                                                                       CultureInfo.InvariantCulture);
                                                int day = 1;
                                                int month = 1;
                                                if (substring.Length > 4)
                                                {
                                                    month = Int32.Parse(substring.Substring(4, 2),
                                                                        CultureInfo.InvariantCulture);
                                                }
                                                if (substring.Length > 6)
                                                {
                                                    day = Int32.Parse(substring.Substring(6, 2),
                                                                      CultureInfo.InvariantCulture);
                                                }
                                                theValue = new DateTime(year, month, day);
                                            }
                                            break;
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new ArgumentOutOfRangeException(
                                                string.Format(CultureInfo.InvariantCulture,
                                                              "Converting YYYYMMDD to DateTime failed for value:{0} {1}",
                                                              substring, property.PropertyType), ex);
                                        }
                                    case "System.Decimal":
                                        var myCultureInfo = new CultureInfo("en-US");
                                        theValue = Decimal.Parse(substring, myCultureInfo);
                                        if (!substring.Contains("."))
                                            theValue = (decimal)theValue / 100;
                                        break;
                                    case "System.String":
                                        theValue = substring;
                                        break;
                                    default:
                                        break;
                                }
                                property.SetValue(this, theValue, null);
                            }
                            catch (Exception ex)
                            {
                                throw new ArgumentOutOfRangeException(
                                    "Converting from fixed file, field " + property.Name + " (" + property.PropertyType +
                                    ") failed for value: <" + substring + ">", ex);
                            }
                            break;
                        }
                    }
                }
            }
            return entity;
        }
    }
}