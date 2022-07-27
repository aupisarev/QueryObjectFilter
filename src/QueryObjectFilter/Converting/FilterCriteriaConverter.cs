using QueryObjectFilter.Filtration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Converting
{
    /// <summary>
    /// Конвертер критериев фильтрации в формат T
    /// </summary>
    public class FilterCriteriaConverter<T> : IFilterCriteriaConverter<T>
    {
        private readonly IConverterProvider<T> converterProvider;

        public FilterCriteriaConverter(IConverterProvider<T> converterProvider)
        {
            this.converterProvider = converterProvider ?? throw new ArgumentNullException(nameof(converterProvider));
        }

        private ICompareMethodProvider<T> CompareMethodProvider => converterProvider.CompareMethodProvider;

        private T DefaultCondition => converterProvider.DefaultCondition;

        /// <summary>
        /// Преобразовать критерий
        /// </summary>
        private T ConvertCriteria(Criteria criteria, T parameter)
        {
            T condition = DefaultCondition;

            if (criteria.FilterValue != null)
            {
                condition = CompareMethodProvider.GetComparation(criteria, parameter);
            }

            return condition;
        }

        /// <summary>
        /// Преобразовать группу критериев
        /// </summary>
        private T ConvertCriteriaGroup<TSource, TFilter>(CriteriaGroup<TSource, TFilter> criteriaGroup, T parameter)
        {
            T condition = DefaultCondition;

            if (criteriaGroup.Criterias.Any())
            {
                condition = ConvertCriteria(criteriaGroup.Criterias.First(), parameter);
                foreach (var criteria in criteriaGroup.Criterias.Skip(1))
                {
                    var secondCondition = ConvertCriteria(criteria, parameter);
                    condition = CreateBinaryCondition(condition, secondCondition, criteriaGroup.Operation);
                }
            }

            if (criteriaGroup.Groups.Any())
            {
                foreach (var group in criteriaGroup.Groups)
                {
                    var secondCondition = ConvertCriteriaGroup(group, parameter);
                    condition = CreateBinaryCondition(condition, secondCondition, criteriaGroup.Operation);
                }
            }

            if (criteriaGroup.CollectionCriterias.Any())
            {
                var collections = criteriaGroup.CollectionCriterias.Keys;

                foreach (var collection in collections)
                {
                    var criterias = criteriaGroup.CollectionCriterias[collection];

                    T collectionCondition = DefaultCondition;
                    var collectionParam = converterProvider.GetCollectionParameter(collection);

                    foreach (var criteria in criterias)
                    {
                        var secondCondition = ConvertCriteria(criteria, collectionParam);
                        collectionCondition = CreateBinaryCondition(collectionCondition, secondCondition, criteriaGroup.Operation);
                    }

                    collectionCondition = converterProvider.GetCollectionCondition(parameter, collection, collectionCondition, collectionParam);

                    condition = CreateBinaryCondition(condition, collectionCondition, criteriaGroup.Operation);
                }
            }

            return converterProvider.FormatGroupCondition(criteriaGroup, condition);
        }

        /// <summary>
        /// Получить элемент сравнения двух условий
        /// </summary>
        private T CreateBinaryCondition(T firstCondition, T secondCondition, GroupOperation operation)
        {
            if (secondCondition.ToString() == DefaultCondition.ToString())
                return firstCondition;

            return operation switch
            {
                var value when value == GroupOperation.And => firstCondition.ToString() == DefaultCondition.ToString() ? secondCondition : converterProvider.AndCondition(firstCondition, secondCondition),
                var value when value == GroupOperation.Or => firstCondition.ToString() == DefaultCondition.ToString() ? secondCondition : converterProvider.OrCondition(firstCondition, secondCondition),
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Конвертировать критерии фильтрации в тип T
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TFilter"></typeparam>
        /// <param name="filterCriteria"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public T Convert<TSource, TFilter>(FilterCriteria<TSource, TFilter> filterCriteria, string parameterName = null)
        {
            var parameter = converterProvider.GetParameter<TSource>(parameterName);
            return converterProvider.Convert<TSource>(ConvertCriteriaGroup(filterCriteria.CriteriaGroup, parameter), parameter);
        }
    }
}
