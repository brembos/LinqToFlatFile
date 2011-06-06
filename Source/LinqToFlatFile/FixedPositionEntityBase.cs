
using System;
using System.Globalization;
using System.Reflection;

namespace LinqToFlatFile
{
    [Serializable]
    public abstract class FixedPositionEntityBase : IFixedEntity
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

        public void ReadLine(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                foreach (PropertyInfo property in GetType().GetProperties())
                {
                    foreach (
                        FixedPositionAttribute fixedFileAttribute in
                            property.GetCustomAttributes(typeof(FixedPositionAttribute), false))
                    {
                        if (null != fixedFileAttribute)
                        {
                            string substring = string.Empty;
                            if (fixedFileAttribute.StartPosition <= input.Length - 1)
                            {
                                substring = input.Substring(fixedFileAttribute.StartPosition,
                                                            Math.Min(
                                                                (fixedFileAttribute.EndPosition -
                                                                 fixedFileAttribute.StartPosition + 1),
                                                                input.Length - fixedFileAttribute.StartPosition));
                            }
                            switch (_trimInput)
                            {
                                case TrimInputMode.Trim:
                                    substring = substring.Trim();
                                    break;
                                case TrimInputMode.TrimStart:
                                    substring = substring.TrimStart();
                                    break;
                                case TrimInputMode.TrimEnd:
                                    substring = substring.TrimEnd();
                                    break;
                            }

                            try
                            {
                                object theValue = null;
                                switch (property.PropertyType.ToString())
                                {
                                    case "System.Boolean":
                                        bool outbool;
                                        if (Boolean.TryParse(substring, out outbool))
                                        {
                                            outbool = false;
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
                                        //<example>070228</example>yymmdd
                                        try
                                        {
                                            if (substring.Equals("000000"))
                                                theValue = DateTime.MinValue;
                                            else
                                            {
                                                int year = Int32.Parse(substring.Substring(0, 2),
                                                                       CultureInfo.InvariantCulture);
                                                if (year < 50)
                                                    year += 2000;
                                                else
                                                    year += 1900;
                                                int day = 1;
                                                int month = 1;
                                                if (substring.Length > 2)
                                                {
                                                    month = Int32.Parse(substring.Substring(2, 2),
                                                                        CultureInfo.InvariantCulture);
                                                    day = Int32.Parse(substring.Substring(4, 2),
                                                                      CultureInfo.InvariantCulture);
                                                }
                                                theValue = new DateTime(year, month, day);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new ArgumentOutOfRangeException(
                                                "Converting YYMMDD to DateTime failed for value:" + substring, ex);
                                        }
                                        break;
                                    case "System.Decimal":
                                        var MyCultureInfo = new CultureInfo("en-US");
                                        theValue = Decimal.Parse(substring, MyCultureInfo);
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

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public string MakeLine()
        {
            string result = string.Empty;

            foreach (PropertyInfo property in GetType().GetProperties())
            {
                foreach (
                    FixedPositionAttribute fixedFileAttribute in
                        property.GetCustomAttributes(typeof(FixedPositionAttribute), false))
                {
                    if (fixedFileAttribute != null)
                    {
                        object theValue = property.GetValue(this, null);
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

        public string MakeHeader()
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
