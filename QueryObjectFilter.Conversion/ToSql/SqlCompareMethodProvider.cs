using QueryObjectFilter.Converting;
using QueryObjectFilter.Filtration;
using System;
using System.Collections;
using System.Globalization;

namespace QueryObjectFilter.Conversion.ToSql
{
    /// <summary>
    /// Поставщик методов сравнения в формате Sql
    /// </summary>
    public class SqlCompareMethodProvider : ICompareMethodProvider<string>
    {
        public SqlCompareMethodProvider()
        { }

        public string GetComparation(Criteria criteria, string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                parameter = string.Empty;
            else
                parameter = $"{parameter}.";

            return criteria.CompareMethod switch
            {
                var value when value == CompareMethod.Equal =>
                    $"{parameter}{criteria.SourceProperty.Name} = {ConvertFilterValue(criteria.FilterValue, criteria.SourceProperty.PropertyType)}",

                var value when value == CompareMethod.LessThan =>
                    $"{parameter}{criteria.SourceProperty.Name} < {ConvertFilterValue(criteria.FilterValue, criteria.SourceProperty.PropertyType)}",

                var value when value == CompareMethod.GreaterThan =>
                    $"{parameter}{criteria.SourceProperty.Name} > {ConvertFilterValue(criteria.FilterValue, criteria.SourceProperty.PropertyType)}",

                var value when value == CompareMethod.LessThanOrEqual =>
                    $"{parameter}{criteria.SourceProperty.Name} <= {ConvertFilterValue(criteria.FilterValue, criteria.SourceProperty.PropertyType)}",

                var value when value == CompareMethod.GreaterThanOrEqual =>
                    $"{parameter}{criteria.SourceProperty.Name} >= {ConvertFilterValue(criteria.FilterValue, criteria.SourceProperty.PropertyType)}",

                var value when value == CompareMethod.Contains =>
                    $"{parameter}{criteria.SourceProperty.Name} LIKE '%{criteria.FilterValue}%'",

                var value when value == CompareMethod.StartsWith =>
                    $"{parameter}{criteria.SourceProperty.Name} LIKE '{criteria.FilterValue}%'",

                var value when value == CompareMethod.In =>
                    $"{parameter}{criteria.SourceProperty.Name} IN ({ConvertFilterItems(criteria.FilterValue, criteria.SourceProperty.PropertyType)})",

                _ => throw new ArgumentException($"Метод сравнения {criteria.CompareMethod.Code} не поддерживается")
            };
        }

        private string ConvertFilterValue(object filterValue, Type type)
        {
            if (type == typeof(string))
                return $"'{filterValue}'";

            if (type == typeof(DateTime))
                return $"'{(DateTime)filterValue:yyyy.MM.dd HH:mm:ss}'";

            if (type == typeof(decimal))
                return $"{((decimal)filterValue).ToString(new NumberFormatInfo() { CurrencyDecimalSeparator = "." })}";

            return filterValue.ToString();
        }

        private string ConvertFilterItems(object filterValue, Type itemType)
        {
            string itemsString = string.Empty;

            foreach (var item in (IEnumerable)filterValue)
                itemsString += $"{ConvertFilterValue(item, itemType)}, ";

            if (itemsString != string.Empty)
                itemsString = itemsString.Substring(0, itemsString.Length - 2);

            return itemsString;
        }
    }
}
