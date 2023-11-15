using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TozawaNGO.Extension
{
    public class ListGuidConverter : ValueConverter<List<Guid>, string>
    {
        private static readonly string Delimiter = ";";
        public ListGuidConverter() : base(x=> ToString(x), x => ToList(x))
        {
        }

        private static List<Guid> ToList(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {

                return new List<Guid>();
            }

            return s.Split(Delimiter).Select(Guid.Parse).ToList();
        }

        private static string ToString(List<Guid> guids)
        {
            return string.Join(Delimiter, guids);
        }
    }
}