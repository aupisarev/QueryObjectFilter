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
    /// Конвертер критериев фильтрации в Sql-строку
    /// </summary>
    public interface IFilterCriteriaSqlConverter : IFilterCriteriaConverter<string>
    {
        /// <summary>
        /// Получить условное SQL-выражение по критериям фильтра
        /// </summary>
        /// <typeparam name="TSource">Тип фильтруемого объекта</typeparam>
        /// <typeparam name="TFilter">Тип фильтра</typeparam>
        /// <param name="filterCriteria">Критерии фильтрации</param>
        /// <param name="parameterName">Имя параметра, по которому будет построено условное выражение</param>
        /// <returns>Условное SQL-выражение</returns>
        string GetSql<TSource, TFilter>(FilterCriteria<TSource, TFilter> filterCriteria, string parameterName = null);
    }
}
