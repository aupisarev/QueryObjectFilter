using QueryObjectFilter.Converting;
using QueryObjectFilter.Filtration;
using System.Linq;
using System.Reflection;

namespace QueryObjectFilter.Conversion.ToSql
{
    /// <summary>
    /// Поставщик методов для конвертации в SQL
    /// </summary>
    public class SqlConverterProvider : IConverterProvider<string>
    {
        private readonly ICompareMethodProvider<string> compareMethodProvider;

        public SqlConverterProvider(ICompareMethodProvider<string> compareMethodProvider)
        {
            this.compareMethodProvider = compareMethodProvider;
        }

        public string DefaultCondition => string.Empty;

        public ICompareMethodProvider<string> CompareMethodProvider => compareMethodProvider;

        public string AndCondition(string firstCondition, string secondCondition)
        {
            return $"{firstCondition} AND {secondCondition}";
        }

        public string OrCondition(string firstCondition, string secondCondition)
        {
            return $"{firstCondition} OR {secondCondition}";
        }

        public string GetParameter<TSource>(string parameterName)
        {
            return parameterName ?? string.Empty;
        }

        public string GetCollectionParameter(PropertyInfo collection)
        {
            return collection.Name;
        }

        public string GetCollectionCondition(string sourceParameter, PropertyInfo collection, string collectionCondition, string collectionParameter)
        {
            return collectionCondition;
        }

        public string Convert<TSource>(string condition, string parameter)
        {
            if (string.IsNullOrEmpty(condition))
                condition = "1 = 1";

            return condition;
        }

        public string FormatGroupCondition<TSource, TFilter>(CriteriaGroup<TSource, TFilter> criteriaGroup, string condition)
        {
            if (criteriaGroup.Criterias.Count() + criteriaGroup.Groups.Count() + criteriaGroup.CollectionCriterias.SelectMany(c => c.Value).Count() <= 1)
                return condition;

            return $"({condition})";
        }
    }
}
