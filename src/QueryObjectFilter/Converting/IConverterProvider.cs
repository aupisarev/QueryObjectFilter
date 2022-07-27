using QueryObjectFilter.Filtration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Converting
{
    /// <summary>
    /// Поставщик методов для конвертации
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConverterProvider<T>
    {
        /// <summary>
        /// Условие-константа, возвращающся true ("1 = 1")
        /// </summary>
        public T DefaultCondition { get; }

        /// <summary>
        /// Поставщик методов сравнения для типа T
        /// </summary>
        public ICompareMethodProvider<T> CompareMethodProvider { get; }

        /// <summary>
        /// Комбинация условий через операцию AND
        /// </summary>
        /// <param name="firstCondition">Первое условие</param>
        /// <param name="secondCondition">Второе условие</param>
        /// <returns></returns>
        public T AndCondition(T firstCondition, T secondCondition);

        /// <summary>
        /// Комбинация условий через операцию OR
        /// </summary>
        /// <param name="firstCondition">Первое условие</param>
        /// <param name="secondCondition">Второе условие</param>
        /// <returns></returns>
        public T OrCondition(T firstCondition, T secondCondition);

        /// <summary>
        /// Получить параметр типа TSource
        /// </summary>
        /// <param name="parameterName">Наименование параметра</param>
        /// <returns></returns>
        public T GetParameter<TSource>(string parameterName);

        /// <summary>
        /// Конвертировать условие в окончательный вариант условного выражения типа T
        /// </summary>
        /// <typeparam name="TSource">Тип фильтруемого объекта</typeparam>
        /// <returns></returns>
        public T Convert<TSource>(T condition, T parameter);

        /// <summary>
        /// Получить параметр коллекции
        /// </summary>
        /// <param name="collectionPropertyInfo"></param>
        /// <returns></returns>
        public T GetCollectionParameter(PropertyInfo collection);

        /// <summary>
        /// Получить условие по коллекции
        /// </summary>
        /// <param name="sourceParameter">Параметр объекта, который содержит коллекцию</param>
        /// <param name="collection">Коллекция</param>
        /// <param name="collectionCondition">Условия по элементам коллекции</param>
        /// <param name="collectionParameter">Параметр элемента коллекции</param>
        /// <returns>Условие по коллекции</returns>
        public T GetCollectionCondition(T sourceParameter, PropertyInfo collection, T collectionCondition, T collectionParameter);

        /// <summary>
        /// Форматировать условное выражение группы критериев
        /// </summary>
        /// <typeparam name="TSource">Тип фильтруемого объекта</typeparam>
        /// <typeparam name="TFilter">Тип фильтра</typeparam>
        /// <param name="criteriaGroup">Группа критериев</param>
        /// <param name="condition">Условное выражение, расчитанное от группы критериев</param>
        /// <returns></returns>
        public T FormatGroupCondition<TSource, TFilter>(CriteriaGroup<TSource, TFilter> criteriaGroup, T condition);
    }
}
