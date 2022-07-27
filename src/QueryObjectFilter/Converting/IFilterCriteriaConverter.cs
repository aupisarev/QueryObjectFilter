using QueryObjectFilter.Filtration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Converting
{
    /// <summary>
    /// Конвертер критериев фильтрации
    /// </summary>
    public interface IFilterCriteriaConverter<T>
    {
        /// <summary>
        /// Преобразовать критерии фильтрации в условное выражение
        /// </summary>
        /// <typeparam name="TSource">Тип фильтруемого объекта</typeparam>
        /// <typeparam name="TFilter">Тип фильтра</typeparam>
        /// <param name="filterCriteria">Критерии фильтрации</param>
        /// <param name="parameterName">Имя параметра, по которому будут построено условное выражение</param>
        /// <returns>Условное выражение</returns>
        T Convert<TSource, TFilter>(FilterCriteria<TSource, TFilter> filterCriteria, string parameterName = null);
    }
}
