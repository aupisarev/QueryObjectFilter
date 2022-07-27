using QueryObjectFilter.Converting;
using QueryObjectFilter.Filtration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Conversion.ToExpression
{
    /// <summary>
    /// Поставщик методов для конвертации в Expression
    /// </summary>
    public class ExpressionConverterProvider : IConverterProvider<Expression>
    {
        private readonly ICompareMethodProvider<Expression> compareMethodProvider;

        public ExpressionConverterProvider(ICompareMethodProvider<Expression> compareMethodProvider)
        {
            this.compareMethodProvider = compareMethodProvider;
        }

        public Expression DefaultCondition => Expression.Equal(Expression.Constant(1), Expression.Constant(1));

        public ICompareMethodProvider<Expression> CompareMethodProvider => compareMethodProvider;

        public Expression AndCondition(Expression firstCondition, Expression secondCondition)
        {
            return Expression.AndAlso(firstCondition, secondCondition);
        }

        public Expression OrCondition(Expression firstCondition, Expression secondCondition)
        {
            return Expression.OrElse(firstCondition, secondCondition);
        }

        public Expression GetParameter<TSource>(string parameterName)
        {
            return Expression.Parameter(typeof(TSource), parameterName);
        }

        public Expression GetCollectionParameter(PropertyInfo collection)
        {
            return Expression.Parameter(collection.PropertyType.GenericTypeArguments.First());
        }

        public Expression GetCollectionCondition(Expression sourceParameter, PropertyInfo collection, Expression collectionCondition, Expression collectionParameter)
        {
            // Генерация метода Any() на коллекцию с внутренними условиями
            var collectionProperty = Expression.Property(sourceParameter, collection.Name);
            var anyMethod = typeof(Enumerable).GetMethods()
                                              .Where(m => m.Name == "Any" && m.GetParameters().Count() == 2)
                                              .First()
                                              .MakeGenericMethod(collection.PropertyType.GenericTypeArguments.First());
            return Expression.Call(anyMethod, collectionProperty, Expression.Lambda(collectionCondition, (ParameterExpression)collectionParameter));
        }

        public Expression Convert<TSource>(Expression condition, Expression parameter)
        {
            return Expression.Lambda<Func<TSource, bool>>(condition, (ParameterExpression)parameter);
        }

        public Expression FormatGroupCondition<TSource, TFilter>(CriteriaGroup<TSource, TFilter> criteriaGroup, Expression condition)
        {
            //не требуется форматирование, движок Expression делает все сам
            return condition;
        }
    }
}
