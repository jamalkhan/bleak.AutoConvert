using System;
using System.ComponentModel;
using System.Linq;

namespace bleak.AutoConvert
{

    public static class PropertySetter
    {
        public static void SetValue(object destination, PropertyDescriptor destinationProperty, object source, bool recursive = false, uint recursionLevel = 1)
        {
            if (destinationProperty.PropertyType.Name == "Nullable`1")
            {
                var genericType = destinationProperty.PropertyType.GenericTypeArguments.FirstOrDefault();
                SetValue(destination, destinationProperty, genericType, source, recursive, recursionLevel);
            }
            else
            {
                SetValue(destination, destinationProperty, destinationProperty.PropertyType, source, recursive, recursionLevel);
            }
        }

        public static void SetValue(object destination, PropertyDescriptor destinationProperty, Type destinationPropertyType, object source, bool recursive = false, uint recursionLevel = 1)
        {
            if (destinationPropertyType.Name == "String")
            {
                destinationProperty.SetValue(destination, Convert.ChangeType(source, destinationPropertyType));
            }
            else if (destinationPropertyType.IsEnum)
            {
                destinationProperty.SetValue(destination, Enum.Parse(destinationPropertyType, value: source.ToString(), ignoreCase: true));
            }
            else if (destinationPropertyType.Name == "Guid")
            {
                destinationProperty.SetValue(destination, Guid.Parse(source.ToString()));
            }
            else if (destinationPropertyType.IsClass)
            {
                if (source.GetType().IsClass)
                {
                    var subSourceProperties = TypeDescriptor.GetProperties(source.GetType()).Cast<PropertyDescriptor>();
                    var subDestionationProperties = TypeDescriptor.GetProperties(destination.GetType()).Cast<PropertyDescriptor>();
                    foreach (var subSourceProperty in subSourceProperties)
                    {
                        var subDestinationProperty = subDestionationProperties.FirstOrDefault(prop => prop.Name == subSourceProperty.Name);
                        if (destinationProperty != null)
                        {
                            if (subSourceProperty.GetValue(source) != null)
                            {
                                PropertySetter.SetValue(destination, destinationProperty, subSourceProperty.GetValue(source), recursive, recursionLevel);
                            }
                        }
                    }
                }
            }
            else
            {
                destinationProperty.SetValue(destination, Convert.ChangeType(source, destinationPropertyType));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
        }
    }
}
