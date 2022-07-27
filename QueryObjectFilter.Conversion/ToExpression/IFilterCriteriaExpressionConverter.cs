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
    public interface IFilterCriteriaExpressionConverter : IFilterCriteriaConverter<Expression>
    {
        /// <summary>
        /// Получить условное Expression-выражение по критериям фильтра
        /// </summary>
        /// <typeparam name="TSource">Тип фильтруемого объекта</typeparam>
        /// <typeparam name="TFilter">Тип фильтра</typeparam>
        /// <param name="filterCriteria">Критерии фильтрации</param>
        /// <param name="parameterName">Имя параметра, по которому будет построено условное выражение</param>
        /// <returns>Условное Expression-выражение</returns>
        Expression<Func<TSource, bool>> GetExpression<TSource, TFilter>(FilterCriteria<TSource, TFilter> filterCriteria, string parameterName);
    }
}
