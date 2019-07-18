using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using MilSpace.Core.Actions.Interfaces;

namespace MilSpace.Core.Actions.Base
{
    [XmlRoot("ActionParam")]
    [Serializable]
    public sealed class ActionParam<T> : IActionParam, IConvertible
    {
        private const string VALUR_PROPERTY_NAME = "Value";
        private string paramName;
        private T paramVal;

        public ActionParam()
        {
        }
        [XmlElement("Name")]
        public string ParamName
        {
            get
            {
                return this.paramName;
            }
            set
            {
                this.paramName = value;
            }
        }

        [XmlElement("Value")]
        public T Value
        {
            get
            {
                return this.paramVal;
            }
            set
            {
                this.paramVal = value;
            }
        }

        public bool IsDefault
        {
            get
            {
                return this.Value.Equals(default(T));
            }
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return this.IsDefault;
        }

        public byte ToByte(IFormatProvider provider)
        {
            if (typeof(T) == typeof(int) || (typeof(T) == typeof(byte)))
            {
                return Convert.ToByte(this.Value);
            }

            throw new InvalidCastException("Cannot cast to Byte");
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            int result = 0;
            if (typeof(T) == typeof(int) || (typeof(T) == typeof(byte)) || typeof(T) == typeof(double) || typeof(T) == typeof(long) || typeof(T) == typeof(decimal))
            {
                return Convert.ToInt32(this.Value);
            }
            if (typeof(T) == typeof(string))
            {
                int.TryParse(this.Value.ToString(), out result);
            }

            return result;
        }

        public long ToInt64(IFormatProvider provider)
        {
            long result = 0;
            if (typeof(T) == typeof(int) || (typeof(T) == typeof(byte)) || typeof(T) == typeof(double))
            {
                return Convert.ToInt64(this.Value);
            }
            if (typeof(T) == typeof(string))
            {
                long.TryParse(this.Value.ToString(), out result);
            }

            return result;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            return this.Value.ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {

            if (conversionType.Equals(this.GetType()))
            {
                return this;
            }

            //Get property info for Value poperty
            PropertyInfo pi = conversionType.GetProperty(VALUR_PROPERTY_NAME);

            //Create a new ActionParam
            ConstructorInfo ctor = conversionType.GetConstructors().First(c => c.GetParameters().Count() == 0);
            LoadedActions.ObjectActivator<IActionParam> createdActivator = LoadedActions.GetActivator<IActionParam>(ctor);
            IActionParam art = createdActivator();
            

            //Convert the value into new type of ActionParam
            pi.SetValue(art, Convert.ChangeType(this.Value, pi.PropertyType, System.Globalization.CultureInfo.InvariantCulture), null);

            return art;
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return this.ToString(null);
        }

        public Type GetValueType
        {
            get { return this.Value.GetType(); }
        }

        public Type GetParameterType
        {
            get { return typeof(T); }
        }

        public T GetDefaultInstance()
        {
            return LoadedActions.GetDefaultInstance<T>();
        }
        public string ToXml()
        {

            XmlSerializer serializer = new XmlSerializer(this.Value.GetType());
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, this.Value);
                writer.Close();
                return writer.ToString();
            }
        }
    }
}
