using QueryObjectFilter.Converting;
using QueryObjectFilter.Filtration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Conversion.ToExpression
{
    /// <summary>
    /// Конвертер критериев фильтрации в Expression
    /// </summary>
    public class FilterCriteriaExpressionConverter : FilterCriteriaConverter<Expression>, IFilterCriteriaExpressionConverter
    {
        public FilterCriteriaExpressionConverter(IConverterProvider<Expression> converterProvider) : base(converterProvider)
        {
        }

        public Expression<Func<TSource, bool>> GetExpression<TSource, TFilter>(FilterCriteria<TSource, TFilter> filterCriteria, string parameterName) =>
            (Expression<Func<TSource, bool>>)Convert(filterCriteria, parameterName);
    }
}
