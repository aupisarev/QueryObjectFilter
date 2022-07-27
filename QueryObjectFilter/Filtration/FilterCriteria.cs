using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Filtration
{
    /// <summary>
    /// Критерии фильтрации
    /// </summary>
    /// <typeparam name="TSource">Тип фильтруемого объекта</typeparam>
    /// <typeparam name="TFilter">Тип фильтра</typeparam>
    public class FilterCriteria<TSource, TFilter>
    {
        private readonly CriteriaGroup<TSource, TFilter> mainGroup;

        /// <summary>
        /// Критерии фильтрации
        /// </summary>
        /// <param name="filter">Фильтр</param>
        public FilterCriteria(TFilter filter)
        {
            mainGroup = new CriteriaGroup<TSource, TFilter>(filter, GroupOperation.And);
        }

        /// <summary>
        /// Главная группа критериев
        /// </summary>
        public CriteriaGroup<TSource, TFilter> CriteriaGroup => mainGroup;

        /// <summary>
        /// Новая группа критериев
        /// </summary>
        /// <param name="operation">Операция комбинации критериев внутри группый</param>
        /// <returns>Новая группа критериев</returns>
        public CriteriaGroup<TSource, TFilter> Group(GroupOperation operation)
        {
            return mainGroup.Group(operation);
        }

        /// <summary>
        /// Новая группа критериев с комбинацией через операцию "И"
        /// </summary>
        /// <returns>Новая группа критериев</returns>
        public CriteriaGroup<TSource, TFilter> AndGroup()
        {
            return Group(GroupOperation.And);
        }

        /// <summary>
        /// Новая группа критериев с комбинацией через операцию "ИЛИ"
        /// </summary>
        /// <returns>Новая группа критериев</returns>
        public CriteriaGroup<TSource, TFilter> OrGroup()
        {
            return Group(GroupOperation.Or);
        }
    }
}
