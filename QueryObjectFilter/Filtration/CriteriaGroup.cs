using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Filtration
{
    /// <summary>
    /// Группа критериев
    /// </summary>
    /// <typeparam name="TSource">Тип фильтруемого объекта</typeparam>
    /// <typeparam name="TFilter">Тип фильтра</typeparam>
    public class CriteriaGroup<TSource, TFilter>
    {
        private readonly TFilter filter;
        private readonly GroupOperation operation;
        private readonly CriteriaGroup<TSource, TFilter> parentGroup;
        private readonly List<CriteriaGroup<TSource, TFilter>> groups = new();
        private readonly Dictionary<PropertyInfo, List<Criteria>> collectionCriterias = new();
        private readonly List<Criteria> criterias = new();

        internal CriteriaGroup(TFilter filter, GroupOperation operation)
        {
            this.operation = operation ?? throw new ArgumentNullException(nameof(operation));
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
            parentGroup = null;
        }

        public CriteriaGroup(TFilter filter, GroupOperation operation, CriteriaGroup<TSource, TFilter> parentGroup) : this(filter, operation)
        {
            this.parentGroup = parentGroup ?? throw new ArgumentNullException(nameof(parentGroup));
        }


        /// <summary>
        /// Список вложенных групп критериев
        /// </summary>
        public IReadOnlyList<CriteriaGroup<TSource, TFilter>> Groups => groups;

        /// <summary>
        /// Список критериев
        /// </summary>
        public IReadOnlyList<Criteria> Criterias => criterias;

        /// <summary>
        /// Критерии вложенных коллекций
        /// </summary>
        public IReadOnlyDictionary<PropertyInfo, List<Criteria>> CollectionCriterias => collectionCriterias;

        /// <summary>
        /// Операция комбинации критериев
        /// </summary>
        public GroupOperation Operation => operation;


        /// <summary>
        /// Добавить критерий по вложенной коллекции
        /// </summary>
        /// <typeparam name="TCollectionElement">Тип элемента коллекции</typeparam>
        /// <typeparam name="TSourceProperty">Тип свойства фильтруемого объекта</typeparam>
        /// <typeparam name="TFilterProperty">Тип свойства объекта фильтра</typeparam>
        /// <param name="collection">Фильтруемая коллекция</param>
        /// <param name="sourceProperty">Свойство фильтруемого объекта</param>
        /// <param name="filterProperty">Своство объекта фильтра</param>
        /// <param name="compareMethod">Метод сравнения значений указанных cвойств фильтруемого объекта и объекта фильтра</param>
        /// <returns>Текущая группа критериев</returns>
        /// <exception cref="ArgumentException"></exception>
        public CriteriaGroup<TSource, TFilter> AddCriteria<TCollectionElement, TSourceProperty, TFilterProperty>(
                                                    Expression<Func<TSource, IEnumerable<TCollectionElement>>> collection,
                                                    Expression<Func<TCollectionElement, TSourceProperty>> sourceProperty,
                                                    Func<TFilter, TFilterProperty> filterProperty,
                                                    CompareMethod compareMethod)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(filterProperty);
            ArgumentNullException.ThrowIfNull(sourceProperty);

            var filterValue = filterProperty(filter);

            if (filterValue != null)
            {
                var collectionExpression = collection.Body as MemberExpression ?? throw new ArgumentException("Некорректное значение collection");
                var collectionPropertyInfo = (PropertyInfo)collectionExpression.Member;
                var memberExpression = sourceProperty.Body as MemberExpression ?? throw new ArgumentException("Некорректное значение sourceProperty");
                var memberPropertyInfo = (PropertyInfo)memberExpression.Member;

                var criteria = new Criteria(memberPropertyInfo, filterValue, compareMethod);

                if (collectionCriterias.TryGetValue(collectionPropertyInfo, out var criterias))
                    criterias.Add(criteria);
                else
                    collectionCriterias.TryAdd(collectionPropertyInfo, new List<Criteria>() { criteria });
            }

            return this;
        }

        /// <summary>
        /// Добавить критерий
        /// </summary>
        /// <typeparam name="TSourceProperty">Тип свойства фильтруемого объекта</typeparam>
        /// <param name="sourceProperty">Свойство фильтруемого объекта</param>
        /// <param name="filterValue">Значение фильтра</param>
        /// <param name="compareMethod">Метод сравнения значений указанного свойства фильтруемого объекта и значения фильтра</param>
        /// <returns>Текущая группа критериев</returns>
        public CriteriaGroup<TSource, TFilter> AddCriteria<TSourceProperty>(
                                                    Expression<Func<TSource, TSourceProperty>> sourceProperty,
                                                    TSourceProperty filterValue,
                                                    CompareMethod compareMethod)
        {
            return AddCriteria(sourceProperty, _ => filterValue, compareMethod);
        }

        /// <summary>
        /// Добавить критерий
        /// </summary>
        /// <typeparam name="TSourceProperty">Тип свойства фильтруемого объекта</typeparam>
        /// <typeparam name="TFilterProperty">Тип свойства объекта фильтра</typeparam>
        /// <param name="sourceProperty">Свойство фильтруемого объекта</param>
        /// <param name="filterProperty">Своство объекта фильтра</param>
        /// <param name="compareMethod">Метод сравнения значений указанных cвойств фильтруемого объекта и объекта фильтра</param>
        /// <returns>Текущая группа критериев</returns>
        public CriteriaGroup<TSource, TFilter> AddCriteria<TSourceProperty, TFilterProperty>(
                                                    Expression<Func<TSource, TSourceProperty>> sourceProperty,
                                                    Func<TFilter, TFilterProperty> filterProperty,
                                                    CompareMethod compareMethod)
        {
            ArgumentNullException.ThrowIfNull(filterProperty);
            ArgumentNullException.ThrowIfNull(sourceProperty);

            var filterValue = filterProperty(filter);

            if (filterValue != null)
            {
                var memberExpression = sourceProperty.Body as MemberExpression;
                if (memberExpression == null)
                    throw new ArgumentException("Invalid sourceProperty");

                AddCriteria((PropertyInfo)memberExpression.Member, filterValue, compareMethod);
            }

            return this;
        }

        /// <summary>
        /// Добавить критерии сравнения свойств фильтруемого объекта с одноименными свойствами фильтра (метод сравнения по умолчанию для типа свойства)
        /// </summary>
        /// <returns>Текущая группа критериев</returns>
        /// <remarks>Не работает с вложенными коллекциями</remarks>
        public CriteriaGroup<TSource, TFilter> AddCriteriaFromProperties()
        {
            return AddCriteriaFromProperties(source => source);
        }

        /// <summary>
        /// Добавить критерии сравнения свойств фильтруемого объекта с одноименными свойствами фильтра (метод сравнения по умолчанию для типа свойства)
        /// </summary>
        /// <typeparam name="TResult">Anonymous type</typeparam>
        /// <param name="properties">Список свойств фильтруемого объекта для сравнения</param>
        /// <returns>Текущая группа критериев</returns>
        /// <remarks>Не работает с вложенными коллекциями</remarks>
        public CriteriaGroup<TSource, TFilter> AddCriteriaFromProperties<TResult>(Func<TSource, TResult> properties)
        {
            ArgumentNullException.ThrowIfNull(properties);

            var sourceProperties = typeof(TResult).GetProperties();
            AddCriteriasFromSourceProperties(sourceProperties);

            return this;
        }

        /// <summary>
        /// Добавить критерии сравнения свойств фильтруемого объекта с одноименными свойствами фильтра (метод сравнения по умолчанию для типа свойства источника)
        /// </summary>
        /// <param name="properties">Свойства фильтруемого объекта</param>
        /// <remarks>Не работает с вложенными коллекциями</remarks>
        private void AddCriteriasFromSourceProperties(IEnumerable<PropertyInfo> properties)
        {
            var filterProperties = typeof(TFilter).GetProperties();
            foreach (var sourceProperty in properties)
            {
                var filterValue = filterProperties.Where(p => p.Name == sourceProperty.Name).FirstOrDefault()?.GetValue(filter);

                if (filterValue == null)
                    continue;

                CompareMethod compareMethod = CompareMethod.Equal;
                if (sourceProperty.PropertyType == typeof(string))
                    compareMethod = CompareMethod.Contains;

                AddCriteria(sourceProperty, filterValue, compareMethod);
            }
        }

        /// <summary>
        /// Добавить критерии сравнения неиспользуемых свойств фильтруемого объекта с одноименными свойствами фильтра (метод сравнения по умолчанию для типа свойства источника)
        /// </summary>
        /// <returns>Текущая группа критериев</returns>
        /// <remarks>Не работает с вложенными коллекциями</remarks>
        public CriteriaGroup<TSource, TFilter> AddCriteriaFromUnusedProperties()
        {
            var sourceProperties = typeof(TSource).GetProperties(); //исключить коллекции, обработать вложенные классы
            var usedProperties = FindUsedProperties();
            var unusedProperties = sourceProperties.Except(usedProperties);

            AddCriteriasFromSourceProperties(unusedProperties);

            return this;
        }

        /// <summary>
        /// Добавить критерий
        /// </summary>
        /// <param name="propertyInfo">Метеданные свойства</param>
        /// <param name="filterValue">Значение фильтра</param>
        /// <param name="compareMethod">Метод сравнения</param>
        /// <returns></returns>
        private CriteriaGroup<TSource, TFilter> AddCriteria(PropertyInfo propertyInfo, object filterValue, CompareMethod compareMethod)
        {
            criterias.Add(new Criteria(propertyInfo, filterValue, compareMethod));
            return this;
        }

        /// <summary>
        /// Найти все используемые свойства фильтруемого объекта на всем дереве групп критериев
        /// </summary>
        /// <returns>Список используемых свойств</returns>
        private List<PropertyInfo> FindUsedProperties()
        {
            CriteriaGroup<TSource, TFilter> mainGroup = this;
            do mainGroup = mainGroup.parentGroup;
            while (mainGroup.parentGroup != null);

            var allCriterias = FindCriterias(mainGroup);
            return allCriterias.Select(c => c.SourceProperty).ToList();
        }

        /// <summary>
        /// Найти все критерии по группе и внутренним группам
        /// </summary>
        /// <returns>Список критериев</returns>
        private List<Criteria> FindCriterias(CriteriaGroup<TSource, TFilter> group)
        {
            var criterias = new List<Criteria>(group.criterias);
            foreach (var childGroup in group.Groups)
                criterias.AddRange(FindCriterias(childGroup));

            return criterias;
        }


        /// <summary>
        /// Новая группа критериев
        /// </summary>
        /// <param name="operation">Операция комбинации критериев внутри группы</param>
        /// <returns>Новая группа критериев</returns>
        public CriteriaGroup<TSource, TFilter> Group(GroupOperation operation)
        {
            var group = new CriteriaGroup<TSource, TFilter>(filter, operation, this);
            groups.Add(group);
            return group;
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

        /// <summary>
        /// Закрыть группу критериев
        /// </summary>
        /// <returns>Родительская группа; текущая группа, если родительская группа отсутствует</returns>
        public CriteriaGroup<TSource, TFilter> CloseGroup()
        {
            if (parentGroup != null)
                return parentGroup;

            return this;
        }
    }
}
