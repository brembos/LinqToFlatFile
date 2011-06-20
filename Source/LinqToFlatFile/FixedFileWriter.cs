using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace LinqToFlatFile
{
    public class FixedFileWriter<TEntity> : IFileWriter<TEntity> where TEntity : new()
    {
        private const char _paddingNumber = '0';
        private char _paddingChar = ' ';
        private TrimInputMode _trimInput = TrimInputMode.Trim;

        public TrimInputMode TrimInput
        {
            get { return _trimInput; }
            set { _trimInput = value; }
        }

        public char PaddingChar
        {
            get { return _paddingChar; }
            set { _paddingChar = value; }
        }


        public Stream WriteFile(IEnumerable<TEntity> collection, bool headerRow)
        {
            throw new NotImplementedException();
        }

        public string MakeLine(TEntity entity)
        {
            string result = string.Empty;

            foreach (PropertyInfo property in entity.GetType().GetProperties())
            {
                foreach (
                    FixedPositionAttribute fixedFileAttribute in
                        property.GetCustomAttributes(typeof(FixedPositionAttribute), false))
                {
                    if (fixedFileAttribute != null)
                    {
                        object theValue = property.GetValue(entity, null);
                        string propertyValue = theValue != null ? theValue.ToString() : string.Empty;

                        int width = fixedFileAttribute.EndPosition - fixedFileAttribute.StartPosition + 1;

                        switch (property.PropertyType.ToString())
                        {
                            case "System.Int32":
                            case "System.Int16":
                                propertyValue = propertyValue.PadLeft(width, _paddingNumber);
                                break;
                            case "System.Decimal":
                                var MyCultureInfo = new CultureInfo("en-US");
                                propertyValue = propertyValue.ToString(MyCultureInfo).Replace(",", "").PadLeft(width, _paddingNumber);
                                break;
                            case "System.DateTime":
                                //<example>070228</example>yymmdd
                                DateTime dateTime = DateTime.Parse(propertyValue, new CultureInfo("nb-NO"));
                                if (!dateTime.Equals(DateTime.MinValue))
                                    propertyValue = dateTime.Year.ToString(CultureInfo.InvariantCulture).Substring(2, 2) +
                                                    dateTime.Month + dateTime.Day;
                                else
                                    propertyValue = "000000";
                                break;
                            case "System.Boolean":
                                propertyValue = propertyValue == "True" ? "J" : "N";
                                break;
                        }

                        if (fixedFileAttribute.StartPosition > 0 && result.Length < fixedFileAttribute.StartPosition)
                            result = result.PadRight(fixedFileAttribute.StartPosition, _paddingChar);
                        if (propertyValue.Length > width)
                            propertyValue = propertyValue.Substring(0, width);

                        string left = string.Empty;
                        string right = string.Empty;

                        if (fixedFileAttribute.StartPosition > 0)
                            left = result.Substring(0, fixedFileAttribute.StartPosition);

                        if (result.Length > fixedFileAttribute.EndPosition + 1)
                            right = result.Substring(fixedFileAttribute.EndPosition + 1);

                        if (propertyValue.Length < fixedFileAttribute.EndPosition - fixedFileAttribute.StartPosition + 1)
                            propertyValue =
                                propertyValue.PadRight(
                                    fixedFileAttribute.EndPosition - fixedFileAttribute.StartPosition + 1, _paddingChar);
                        result = left + propertyValue + right;
                    }
                    break;
                }
            }
            return result;
        }

        public string MakeHeader(TEntity entity)
        {
            throw new NotImplementedException();
        }


    }
}