using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace bleak.AutoConvert
{
    public static class AutoConvertExtensionMethods
    {
        /// <summary>
        /// Converts an input string into a scalar type, such as an integer
        /// </summary>
        /// <returns>The convert.</returns>
        /// <param name="input">Input.</param>
        /// <typeparam name="TDestination">The 1st type parameter.</typeparam>
        public static TDestination Convert<TDestination>(this string input)
        {
            try
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(TDestination));
                return (TDestination)tc.ConvertFrom(input);
            }
            catch { }
            var type = typeof(TDestination);
            if (!type.IsValueType || Nullable.GetUnderlyingType(type) != null)
            {
                return default(TDestination);
            }
            throw new ArgumentOutOfRangeException($"AutoConvert {input} to {type.Name} failed");
        }

        /// <summary>
        /// Registers the type converter. Generally, does not need to be called by a developer
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <typeparam name="TC">The 2nd type parameter.</typeparam>
        public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {
            TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
        }

        /// <summary>
        /// Converts a Dictionary to an Object
        /// </summary>
        /// <returns>The convert.</returns>
        /// <param name="source">Source.</param>
        /// <typeparam name="TDestination">The Type of the Output</typeparam>
        /// <remarks>Does not recurse, therefore does not AutoConvert subclasses</remarks>
        public static TDestination Convert<TDestination>(this Dictionary<string, object> source)
        {
            if (source == null)
            {
                return default(TDestination);
            }

            Type destinationType = typeof(TDestination);

            var destination = Activator.CreateInstance(destinationType);
            var destinationProperties = TypeDescriptor.GetProperties(typeof(TDestination)).Cast<PropertyDescriptor>();
            foreach (var kv in source)
            {
                var destinationProperty = destinationProperties.FirstOrDefault(prop => prop.Name == kv.Key);
                if (destinationProperty != null)
                {
                    if (kv.Value != null)
                    {
                        PropertySetter.SetValue(destination, destinationProperty, kv.Value);
                    }
                }
            }
            return (TDestination)destination;
        }

        

        /// <summary>
        /// Converts an Object to another type with properties of the same name.
        /// </summary>
        /// <returns>The convert.</returns>
        /// <param name="source">Input.</param>
        /// <typeparam name="TDestination">The 1st type parameter.</typeparam>
        /// <remarks>Does not recurse, therefore does not AutoConvert subclasses</remarks>
        public static TDestination Convert<TDestination>(this object source) where TDestination : new()
        {
            if (source == null)
            {
                return default(TDestination);
            }

            var destinationProperties = TypeDescriptor.GetProperties(typeof(TDestination)).Cast<PropertyDescriptor>();
            var sourceProperties = TypeDescriptor.GetProperties(source).Cast<PropertyDescriptor>();
            var destination = new TDestination();
            foreach (var entityProperty in sourceProperties)
            {
                var property = entityProperty;
                var destinationProperty = destinationProperties.FirstOrDefault(prop => prop.Name == property.Name);
                if (destinationProperty != null)
                {
                    if (entityProperty.GetValue(source) != null)
                    {
                        PropertySetter.SetValue(destination, destinationProperty, entityProperty.GetValue(source));
                    }
                }
            }
            return destination;
        }
    }
}
