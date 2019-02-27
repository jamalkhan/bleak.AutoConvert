using System;
using System.ComponentModel;
using System.Linq;

namespace bleak.AutoConvert
{
    public static class AutoMap
    {
        public static void Update(object source, object destination, bool recursive = false, uint recursionLevel = 1)
        {
            if (recursive && recursionLevel == 0)
            {
                throw new ArgumentOutOfRangeException("Cannot use recursive with a recursionLevel of 0");
            }

            if (source == null)
            {
                throw new ArgumentNullException("sourceObject");
            }
            if (destination == null)
            {
                throw new ArgumentNullException("destinationObject");
            }

            var sourceProperties = TypeDescriptor.GetProperties(source.GetType()).Cast<PropertyDescriptor>();
            var destionationProperties = TypeDescriptor.GetProperties(destination.GetType()).Cast<PropertyDescriptor>();
            foreach (var sourceProperty in sourceProperties)
            {
                var destinationProperty = destionationProperties.FirstOrDefault(prop => prop.Name == sourceProperty.Name);
                if (destinationProperty != null)
                {
                    if (sourceProperty.GetValue(source) != null)
                    {
                        PropertySetter.SetValue(destination, destinationProperty, sourceProperty.GetValue(source), recursive, recursionLevel);
                    }
                }
            }
        }
    }
}