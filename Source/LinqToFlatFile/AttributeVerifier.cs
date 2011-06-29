using System;
using System.Globalization;
using System.Linq;

namespace LinqToFlatFile
{
    public class AttributeVerifier
    {
        private readonly object entity;

        public AttributeVerifier(object entity)
        {
            this.entity = entity;
        }

        public void Contains(Type attribute)
        {
            if (entity.GetType().GetProperties().Any(
                    propertyInfo => propertyInfo.GetCustomAttributes(attribute, false).Count() > 0))
            {
                return;
            }
            throw new AttributeException(string.Format(CultureInfo.CurrentCulture, "No attribute of type {0} found", attribute));
        }
    }
}