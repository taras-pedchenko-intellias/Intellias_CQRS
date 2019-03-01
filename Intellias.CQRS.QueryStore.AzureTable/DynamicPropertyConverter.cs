﻿using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Intellias.CQRS.Core.Config;

namespace Intellias.CQRS.QueryStore.AzureTable
{
    internal static class DynamicPropertyConverter
    {
        public const string DefaultPropertyNameDelimiter = "_";

        public static Dictionary<string, EntityProperty> Flatten(object root)
        {
            if (root == null)
            {
                return null;
            }

            var propertyDictionary = new Dictionary<string, EntityProperty>();
            var antecedents = new HashSet<object>(new ObjectReferenceEqualityComparer());
            return Flatten(propertyDictionary, root, string.Empty, antecedents) ? propertyDictionary : null;
        }

        public static T ConvertBack<T>(IDictionary<string, EntityProperty> flattenedEntityProperties)
        {
            if (flattenedEntityProperties == null)
            {
                return default(T);
            }

            var uninitializedObject = (T)FormatterServices.GetUninitializedObject(typeof(T));
            return ((IEnumerable<KeyValuePair<string, EntityProperty>>)flattenedEntityProperties).Aggregate(uninitializedObject, (current, kvp) => (T)SetProperty(current, kvp.Key, kvp.Value.PropertyAsObject));
        }

        private static bool Flatten(Dictionary<string, EntityProperty> propertyDictionary, object current, string objectPath, HashSet<object> antecedents)
        {
            if (current == null)
            {
                return true;
            }

            Type type;
            EntityProperty propertyWithType;
            while (true)
            {
                type = current.GetType();
                propertyWithType = CreateEntityPropertyWithType(current, type);
                if (propertyWithType == null)
                {
                    if (current is IEnumerable)
                    {
                        current = string.Format(CultureInfo.InvariantCulture, "<|>jsonSerializedIEnumerableProperty<|>={0}", new object[1]
                        {
                            JsonConvert.SerializeObject(current, CqrsSettings.JsonConfig())
                        });
                    }
                    else
                    {
                        goto label_6;
                    }
                }
                else
                {
                    break;
                }
            }
            propertyDictionary.Add(objectPath, propertyWithType);
            return true;
        label_6:
            var properties = (IEnumerable<PropertyInfo>)type.GetProperties();
            if (!properties.Any())
            {
                throw new SerializationException(string.Format(CultureInfo.InvariantCulture, "Unsupported type : {0} encountered during conversion to EntityProperty. Object Path: {1}", new object[2]
                {
                   type,
                   objectPath
                }));
            }

            var processed = false;
            if (!type.IsValueType)
            {
                if (antecedents.Contains(current))
                {
                    throw new SerializationException(string.Format(CultureInfo.InvariantCulture, "Recursive reference detected. Object Path: {0} Property Type: {1}.", new object[2]
                    {
                         objectPath,
                         type
                    }));
                }

                antecedents.Add(current);
                processed = true;
            }

            var successful = properties.Where(propertyInfo => !ShouldSkip(propertyInfo)).All(propertyInfo =>
            {
                if (propertyInfo.Name.Contains(DefaultPropertyNameDelimiter))
                {
                    throw new SerializationException(string.Format(CultureInfo.InvariantCulture, "Property delimiter: {0} exists in property name: {1}. Object Path: {2}", DefaultPropertyNameDelimiter, propertyInfo.Name, objectPath));
                }

                object current1;
                try
                {
                    current1 = propertyInfo.GetValue(current, null);
                }
                catch (Exception)
                {
                    current1 = string.Format(CultureInfo.InvariantCulture, "<|>jsonSerializedIEnumerableProperty<|>={0}", new object[1]
                    {
                        JsonConvert.SerializeObject(current, CqrsSettings.JsonConfig())
                    });
                }
                return Flatten(propertyDictionary, current1, string.IsNullOrWhiteSpace(objectPath) ? propertyInfo.Name : objectPath + DefaultPropertyNameDelimiter + propertyInfo.Name, antecedents);
            });
            if (processed)
            {
                antecedents.Remove(current);
            }

            return successful;
        }

        private static EntityProperty CreateEntityPropertyWithType(object value, Type type)
        {
            if (type == typeof(string))
            {
                return new EntityProperty((string)value);
            }

            if (type == typeof(byte[]))
            {
                return new EntityProperty((byte[])value);
            }

            if (type == typeof(byte))
            {
                return new EntityProperty(new byte[1]
                {
                    (byte) value
                });
            }

            if (type == typeof(bool))
            {
                return new EntityProperty(new bool?((bool)value));
            }

            if (type == typeof(bool?))
            {
                return new EntityProperty((bool?)value);
            }

            if (type == typeof(DateTime))
            {
                return new EntityProperty(new DateTime?((DateTime)value));
            }

            if (type == typeof(DateTime?))
            {
                return new EntityProperty((DateTime?)value);
            }

            if (type == typeof(DateTimeOffset))
            {
                return new EntityProperty(new DateTimeOffset?((DateTimeOffset)value));
            }

            if (type == typeof(DateTimeOffset?))
            {
                return new EntityProperty((DateTimeOffset?)value);
            }

            if (type == typeof(double))
            {
                return new EntityProperty(new double?((double)value));
            }

            if (type == typeof(double?))
            {
                return new EntityProperty((double?)value);
            }

            if (type == typeof(Guid?))
            {
                return new EntityProperty((Guid?)value);
            }

            if (type == typeof(Guid))
            {
                return new EntityProperty(new Guid?((Guid)value));
            }

            if (type == typeof(int))
            {
                return new EntityProperty(new int?((int)value));
            }

            if (type == typeof(int?))
            {
                return new EntityProperty((int?)value);
            }

            if (type == typeof(uint))
            {
                return new EntityProperty(new int?((int)Convert.ToUInt32(value, CultureInfo.InvariantCulture)));
            }

            if (type == typeof(uint?))
            {
                return new EntityProperty(new int?((int)Convert.ToUInt32(value, CultureInfo.InvariantCulture)));
            }

            if (type == typeof(long))
            {
                return new EntityProperty(new long?((long)value));
            }

            if (type == typeof(long?))
            {
                return new EntityProperty((long?)value);
            }

            if (type == typeof(ulong))
            {
                return new EntityProperty(new long?((long)Convert.ToUInt64(value, CultureInfo.InvariantCulture)));
            }

            if (type == typeof(ulong?))
            {
                return new EntityProperty(new long?((long)Convert.ToUInt64(value, CultureInfo.InvariantCulture)));
            }

            if (type.IsEnum)
            {
                return new EntityProperty(value.ToString());
            }

            if (type == typeof(TimeSpan))
            {
                return new EntityProperty(value.ToString());
            }

            if (type == typeof(TimeSpan?))
            {
                return new EntityProperty(value?.ToString());
            }

            return null;
        }

        private static object SetProperty(object root, string propertyPath, object propertyValue)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            if (propertyPath == null)
            {
                throw new ArgumentNullException(nameof(propertyPath));
            }

            try
            {
                var tupleStack = new Stack<Tuple<object, object, PropertyInfo>>();
                var strArray = propertyPath.Split(new string[1]
                {
                    DefaultPropertyNameDelimiter
                }, StringSplitOptions.RemoveEmptyEntries);
                var obj = root;
                var flag = false;
                for (var index = 0; index < strArray.Length - 1; ++index)
                {
                    var property = obj.GetType().GetProperty(strArray[index]);
                    var uninitializedObject = property.GetValue(obj, null);
                    var propertyType = property.PropertyType;
                    if (uninitializedObject == null)
                    {
                        uninitializedObject = FormatterServices.GetUninitializedObject(propertyType);
                        property.SetValue(obj, ChangeType(uninitializedObject, property.PropertyType), null);
                    }
                    if (flag || propertyType.IsValueType)
                    {
                        flag = true;
                        tupleStack.Push(new Tuple<object, object, PropertyInfo>(uninitializedObject, obj, property));
                    }
                    obj = uninitializedObject;
                }
                var property1 = obj.GetType().GetProperty(((IEnumerable<string>)strArray).Last());
                var str2 = (propertyValue as string);
                if (str2 != null && property1.PropertyType != typeof(string) && str2.StartsWith("<|>jsonSerializedIEnumerableProperty<|>=", StringComparison.InvariantCulture))
                {
                    property1.SetValue(obj, Deserialise(str2.Substring("<|>jsonSerializedIEnumerableProperty<|>=".Length), property1.PropertyType), null);
                }
                else
                {
                    property1.SetValue(obj, ChangeType(propertyValue, property1.PropertyType), null);
                }

                var propertyValue1 = obj;
                while ((uint)tupleStack.Count > 0U)
                {
                    var tuple = tupleStack.Pop();
                    tuple.Item3.SetValue(tuple.Item2, ChangeType(propertyValue1, tuple.Item3.PropertyType), null);
                    propertyValue1 = tuple.Item2;
                }
                return root;
            }
            catch (Exception ex)
            {
                var data = ex.Data;
                data["ObjectRecompositionError"] = data["ObjectRecompositionError"]+ $"Exception thrown while trying to set property value. Property Path: {propertyPath} Property Value: {propertyValue}. Exception Message: {ex.Message}";
                throw;
            }
        }

        private static object ChangeType(object propertyValue, Type propertyType)
        {
            var type1 = Nullable.GetUnderlyingType(propertyType);
            if (type1 is null)
            {
                type1 = propertyType;
            }

            var type2 = type1;
            if (type2.IsEnum)
            {
                return Enum.Parse(type2, propertyValue.ToString());
            }

            if (type2 == typeof(DateTimeOffset))
            {
                return new DateTimeOffset((DateTime)propertyValue);
            }

            if (type2 == typeof(TimeSpan))
            {
                return TimeSpan.Parse(propertyValue.ToString(), CultureInfo.InvariantCulture);
            }

            if (type2 == typeof(uint))
            {
                return (uint)(int)propertyValue;
            }

            if (type2 == typeof(ulong))
            {
                return (ulong)(long)propertyValue;
            }

            if (type2 == typeof(byte))
            {
                return ((byte[])propertyValue)[0];
            }

            return Convert.ChangeType(propertyValue, type2, CultureInfo.InvariantCulture);
        }

        private static bool ShouldSkip(PropertyInfo propertyInfo)
        {
            return !propertyInfo.CanWrite || !propertyInfo.CanRead || Attribute.IsDefined(propertyInfo, typeof(IgnorePropertyAttribute));
        }

        private static object Deserialise(string json, Type type)
        {
            using (var stringReader = new StringReader(json))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return JsonSerializer.Create(CqrsSettings.JsonConfig()).Deserialize(jsonTextReader, type);
                }
            }
        }

        private class ObjectReferenceEqualityComparer : IEqualityComparer<object>
        {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            public bool Equals(object x, object y)
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
            {
                return x == y;
            }

            public int GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }
    }

}
