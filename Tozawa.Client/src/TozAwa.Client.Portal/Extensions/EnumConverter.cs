
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tozawa.Client.Portal.Extensions
{
    public static class EnumConverter
    {
        public static String ConvertToString(this Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }

        public static IEnumerable<Enum> GetEnumCollection(this Type type)
        {
            return Enum.GetValues(type).Cast<Enum>();
        }

        public static EnumType ConverToEnum<EnumType>(this String enumValue)
        {
            return (EnumType)Enum.Parse(typeof(EnumType), enumValue);
        }
    }
}