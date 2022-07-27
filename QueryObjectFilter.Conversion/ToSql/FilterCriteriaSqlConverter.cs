using QueryObjectFilter.Conversion.ToExpression;
using QueryObjectFilter.Converting;
using QueryObjectFilter.Filtration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Conversion.ToSql
{
    /// <summary>
    /// Конвертер критериев фильтрации в SQL-строку
    /// </summary>
    public class FilterCriteriaSqlConverter : FilterCriteriaConverter<string>, IFilterCriteriaSqlConverter
    {
        public FilterCriteriaSqlConverter(IConverterProvider<string> converterProvider) : base(converterProvider)
        {
        }

        public string GetSql<TSource, TFilter>(FilterCriteria<TSource, TFilter> filterCriteria, string parameterName) =>
            Convert(filterCriteria, parameterName);
    }
}
