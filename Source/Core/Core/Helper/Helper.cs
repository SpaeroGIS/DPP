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


        private static string milSpaceRegistriPath = @"SOFTWARE\WOW6432Node\MilSpace\";

        public static bool Convert(SimpleDataTypesEnum typeTo, string value, out object result)
        {
            Type convertTo = MilSpace.Core.Helper.SimpleDataTypes[typeTo];
            try
            {
                TypeConverter tc = TypeDescriptor.GetConverter(convertTo);
                result = tc.ConvertFromString(value);
                return true;
            }
            catch (Exception ex)
            {
                //TODO: create logging
                result = DefaultValueSimpleDataTypes[typeTo].Invoke();
            }
            return false;
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
            numericString = numericString
                .Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                .Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            return double.TryParse(numericString, out result);
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
                //Logger.Debug("Rty to convert to Guid:" + ex.Message);
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


        public static string InvariantFormat(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        public static void SetConfiguration()
        {
            if (string.IsNullOrWhiteSpace(MilSpaceConfiguration.ConfigurationFilePath))
            {
                var configFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
                MilSpaceConfiguration.ConfigurationFilePath = configFile.DirectoryName;
            }
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

        public static string DateFormat => "yyyy-MM-dd H:mm";
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

        public static string GetRegistryValue(string registrypath)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(milSpaceRegistriPath))
            {
                if (key == null)
                {
                    return null;
                }

                var val = key.GetValue(registrypath);
                return val.ToString();
            }
        }
    }
}