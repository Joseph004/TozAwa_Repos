using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TozawaNGO.Extension
{
    public static class ValueConversionExtensions
    {
        public static PropertyBuilder<T> HasListGuidConverter<T>(this PropertyBuilder<T> propertyBuilder)
        {           
            var converter = new ListGuidConverter();
            var comparer = new ValueComparer<List<Guid>>(
                (l, r) => l.CompleteMatch(r), 
                guids => guids == null ? 0 : guids.GetHashCode(),
                guids => new List<Guid>(guids));
            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);            

            return propertyBuilder;
        }        
    }
}