using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace LinqToFlatFile
{
    [Serializable]
    public abstract class TabPositionFixedEntity : IFixedEntity
    {
        public void ReadLine(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                var array = input.Split('\t');
                foreach (PropertyInfo property in GetType().GetProperties())
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
                                            outbool = false;// substring.ToUpperInvariant() == "J";//else J = Ja (true) N=Nei (false).
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
                                                int year = Int32.Parse(substring.Substring(0, 4), CultureInfo.InvariantCulture);
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
                                            throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Converting YYYYMMDD to DateTime failed for value:{0} {1}", substring, property.PropertyType), ex);
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

        }

        public string MakeLine()
        {
            var values = new SortedDictionary<int, string>();
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                foreach (TabPositionAttribute fixedFileAttribute in property.GetCustomAttributes(typeof(TabPositionAttribute), false))
                {
                    if (fixedFileAttribute != null)
                    {
                        object theValue = property.GetValue(this, null);
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
            return values.Values.Aggregate<string, string>(null, (current, value) => current + (value + "\t"));
        }

        public string MakeHeader()
        {
            var values = new SortedDictionary<int, string>();
            foreach (PropertyInfo property in GetType().GetProperties())
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
            return values.Values.Aggregate<string, string>(null, (current, value) => current + (value + "\t"));
        }

        public virtual bool IsValid()
        {
            //Here can logic be added to verify that the entity is valid.
            return true;
        }
    }
}