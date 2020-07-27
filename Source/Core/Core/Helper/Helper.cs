using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Globalization;
using System.IO;
using MilSpace.Configurations;
using System.Reflection;
using ESRI.ArcGIS.Geometry;
using Microsoft.Win32;
using ESRI.ArcGIS.Geodatabase;
using System.Linq.Expressions;

namespace MilSpace.Core
{

    public static class Helper
    {

        public static Dictionary<SimpleDataTypesEnum, Type> SimpleDataTypes = new Dictionary<SimpleDataTypesEnum, Type>()
        { { SimpleDataTypesEnum.DateTime, typeof(DateTime)},
          { SimpleDataTypesEnum.Integer, typeof(int)},
          { SimpleDataTypesEnum.Numeric, typeof(decimal)},
          { SimpleDataTypesEnum.String, typeof(string)},
          { SimpleDataTypesEnum.Undefined , typeof(string)}};


        public static Dictionary<SimpleDataTypesEnum, Func<object>> DefaultValueSimpleDataTypes = new Dictionary<SimpleDataTypesEnum, Func<object>>()
        { { SimpleDataTypesEnum.DateTime, () => { return default(DateTime);}} ,
          { SimpleDataTypesEnum.Integer , () => { return default(int);}} ,
          { SimpleDataTypesEnum.Numeric , () =>{ return default(double);}} ,
          { SimpleDataTypesEnum.String, () =>{ return default(string);}} ,
          { SimpleDataTypesEnum.Undefined , () =>{ return default(string);}}};


        public static Dictionary<esriFieldType, Func<object, object>> GdbFieldsTypes = new Dictionary<esriFieldType, Func<object, object>>
        {
            { esriFieldType.esriFieldTypeString, (value) => { return System.Convert.ToString(value); } },
            { esriFieldType.esriFieldTypeDate, (value) => {return System.Convert.ToDateTime(value); }},
            { esriFieldType.esriFieldTypeDouble, (value) => { return System.Convert.ToDouble(value); }},
            { esriFieldType.esriFieldTypeInteger, (value) => { return System.Convert.ToInt32(value); }},
            { esriFieldType.esriFieldTypeOID, (value) => { return System.Convert.ToInt32(value); }},
            { esriFieldType.esriFieldTypeSmallInteger, (value) => { return System.Convert.ToInt16(value); }},
        };

        public static Dictionary<esriFieldType, Func<object>> DefaultValueEsriDataTypes = new Dictionary<esriFieldType, Func<object>>()
        { { esriFieldType.esriFieldTypeDate, () => { return default(DateTime);}} ,
          { esriFieldType.esriFieldTypeInteger , () => { return default(int);}} ,
          { esriFieldType.esriFieldTypeDouble , () =>{ return default(double);}} ,
          { esriFieldType.esriFieldTypeString, () =>{ return default(string);}} ,
          { esriFieldType.esriFieldTypeOID , () =>{ return default(int);}},
          { esriFieldType.esriFieldTypeSmallInteger , () =>{ return default(short);}}
        };

        private static string milSpaceRegistryPath = @"SOFTWARE\WOW6432Node\MilSpace\";

        public static bool ConvertFromFieldType<T>(esriFieldType fieldType,
                                                    object value,
                                                    out T result,
                                                    out string message)
        {
            message = string.Empty;

            if(value == DBNull.Value)
            {
                result = (T)System.Convert.ChangeType(DefaultValueEsriDataTypes[fieldType].Invoke(), typeof(T));
                return true;
            }

            try
            {
                result = (T)System.Convert.ChangeType(GdbFieldsTypes[fieldType].Invoke(value), typeof(T));
            }
            catch
            {
                message = $"Cannot convert {value} to {typeof(T)}";
                result = default(T);
                return false;
            }

            return true;
        }

        public static T Convert<T>(string value)
        {

            if (SimpleDataTypes.Any(tp => tp.Value.Equals(typeof(T))))
            {
                var convertMetadata = SimpleDataTypes.First(tp => tp.Value.Equals(typeof(T)));

                T result;

                try
                {
                    TypeConverter tc = TypeDescriptor.GetConverter(convertMetadata.Value);
                    result = (T)tc.ConvertFromString(value);

                }
                catch (Exception ex)
                {
                    Logger.Warn($"> Convert. Exception: {ex.Message}");
                    result = (T)DefaultValueSimpleDataTypes[convertMetadata.Key].Invoke();
                }

                return (T)result;
            }

            throw new NotSupportedException($"Type {typeof(T)} is not supportde");
        }

        public static SimpleDataTypesEnum ConvertToSimpleDatatTypeEnum(string stringType)
        {

            if (!string.IsNullOrEmpty(stringType) && Enum.IsDefined(typeof(SimpleDataTypesEnum), stringType))
            {
                return (SimpleDataTypesEnum)Enum.Parse(typeof(SimpleDataTypesEnum), stringType);
            }

            return SimpleDataTypesEnum.Undefined;
        }

        public static bool TryParceToDouble(this string numericString, out double result)
        {
            string snumericString = numericString.Trim();
            string snumericString2;

            if (snumericString.Contains('.'))
            {
                snumericString2 = snumericString.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            }
            else
            {
                snumericString2 = snumericString.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            }

            return double.TryParse(snumericString2, out result);
        }

        public static double ParceToDouble(this string numericString)
        {
            string snumericString = numericString.Trim();
            string snumericString2;

            double result = double.NaN;

            double.TryParse(numericString, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            return result;

            //if (snumericString.Contains('.'))
            //{
            //    snumericString2 = snumericString.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            //}
            //else
            //{
            //    snumericString2 = snumericString.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            //}


            //double.TryParse(snumericString2, out result);
            //return result;
        }

        public static string AutoEllipses(this string str, int length = 5)
        {
            return str.Substring(0, Math.Min(length, str.Length)) + (str.Length > length ? "..." : "");
        }

        public static bool IsGuid(this string guidString, out Guid guidValue)
        {

            guidValue = Guid.Empty;
            if (string.IsNullOrEmpty(guidString))
            {
                return false;
            }
            try
            {
                guidValue = new Guid(guidString);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Debug(">IsGuid. Unable to convert to Guid: " + ex.Message);
            }
            return false;

        }

        public static Dictionary<string, T> GetEnumDictionary<T>(this object enumeration)
        {
            Type tp = typeof(T);
            if (!tp.IsEnum)
            {
                return null;
            }

            var tre = Enum.GetValues(tp).Cast<T>();

            Dictionary<string, T> result = new Dictionary<string, T>();

            foreach (var el in tre)
            {
                var elem = tp.GetField(el.ToString());
                var attrs = elem.GetCustomAttributes(typeof(XmlEnumAttribute), false);
                if (attrs.Length == 1)
                {
                    var attr = attrs.First() as XmlEnumAttribute;
                    result.Add(attr.Name, el);
                }
                else
                {
                    result.Add(el.ToString(), el);
                }
            }
            if (!typeof(T).IsEnum)
            {
                return null;
            }

            return result;
        }

        public static Dictionary<T, string> GetEnumToDictionary<T>(this Type enumeration)
        {
            Type tp = enumeration;
            if (!tp.IsEnum)
            {
                return null;
            }

            var tre = Enum.GetValues(tp).Cast<T>();

            Dictionary<T, string> result = new Dictionary<T, string>();

            foreach (var el in tre)
            {
                var elem = tp.GetField(el.ToString());
                var attrs = elem.GetCustomAttributes(typeof(XmlEnumAttribute), false);
                if (attrs.Length == 1)
                {
                    var attr = attrs.First() as XmlEnumAttribute;
                    result.Add(el, attr.Name);
                }
                else
                {
                    result.Add(el, el.ToString());
                }
            }
            if (!typeof(T).IsEnum)
            {
                return null;
            }

            return result;
        }


        public static string InvariantFormat(this string format, params object[] args)
        {
            return (format.Contains("{") && format.Contains("}")) ? string.Format(CultureInfo.InvariantCulture, format, args) : format;
        }

        public static void SetConfiguration()
        {
            Logger.Debug("> SetConfiguration START");
            if (string.IsNullOrWhiteSpace(MilSpaceConfiguration.ConfigurationFilePath))
            {
                var configFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
                MilSpaceConfiguration.ConfigurationFilePath = configFile.DirectoryName;
            }
            Logger.Debug("SetConfiguration. MilSpaceConfiguration.ConfigurationFilePath: " + MilSpaceConfiguration.ConfigurationFilePath);
            Logger.Debug("> SetConfiguration END");
        }

        public static IEnumerable<IPoint> Vertices(this IPolyline polyline)
        {
            IPointCollection collection = polyline as IPointCollection;
            IPoint[] vertices = new IPoint[collection.PointCount];
            for (int i = 0; i < collection.PointCount; i++)
            {
                vertices[i] = collection.Point[i];
            }
            return vertices;
        }

        public static string DateFormat => "yyyy-MM-dd HH:mm";
        public static string DateFormatSmall => "yyyy-MM-dd";

        public static double Azimuth(this ILine line)
        {
            var degrees = (line.Angle * 180 / Math.PI);

            if (degrees > 90)
            {
                return 90 - degrees;
            }

            if (degrees < -90)
            {
                return Math.Abs(degrees) - 270;
            }

            return Math.Abs(degrees - 90);
        }

        public static double PosAzimuth(this ILine line)
        {
            var degrees = (line.Angle * 180 / Math.PI);

            if (degrees > 90)
            {
                return 360 - (degrees - 90);
            }

            if (degrees < -90)
            {
                return Math.Abs(degrees) + 90;
            }

            return Math.Abs(degrees - 90);
        }


        public static IPoint GetCentroid(this IEnvelope envelope)
        {
            var x = (envelope.XMin + envelope.XMax) / 2;
            var y = (envelope.YMin + envelope.YMax) / 2;
            return new PointClass { X = x, Y = y };
        }

        public static string GetRegistryValue(string registrypath)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(milSpaceRegistryPath))
            {
                if (key == null)
                {
                    return null;
                }

                var val = key.GetValue(registrypath);
                return val.ToString();
            }
        }

        public static string ToFormattedString(this double pointCoord, int signs = 5)
        {
            if (Double.IsNaN(pointCoord))
            {
                return string.Empty;
            }

            var pointString = pointCoord.ToString($"F{signs}");
            return pointString.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".");
        }


        public static T Clone<T>(object cloneFrom)
        {
            var creator = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
            var cloneTo = creator.Invoke();

            foreach (PropertyInfo sourcePropertyInfo in cloneFrom.GetType().GetProperties())
            {
                PropertyInfo destPropertyInfo = cloneTo.GetType().GetProperty(sourcePropertyInfo.Name);

                destPropertyInfo.SetValue(
                    cloneTo,
                    sourcePropertyInfo.GetValue(cloneFrom, null),
                    null);
            }

            foreach (FieldInfo sourcePropertyInfo in cloneFrom.GetType().GetFields())
            {
                FieldInfo destPropertyInfo = cloneTo.GetType().GetField(sourcePropertyInfo.Name);

                destPropertyInfo.SetValue(
                    cloneTo,
                    sourcePropertyInfo.GetValue(cloneFrom));
            }

            return cloneTo;
        }

    }
}