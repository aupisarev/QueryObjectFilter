using QueryObjectFilter.Converting;
using QueryObjectFilter.Filtration;
using System;
using System.Linq.Expressions;

namespace QueryObjectFilter.Conversion.ToExpression
{
    /// <summary>
    /// Поставщик методов сравнения в формате Expression
    /// </summary>
    public class ExpressionCompareMethodProvider : ICompareMethodProvider<Expression>
    {
        public ExpressionCompareMethodProvider()
        { }

        public Expression GetComparation(Criteria criteria, Expression parameter)
        {
            var expressionProperty = Expression.Property(parameter, criteria.SourceProperty.Name);

            return criteria.CompareMethod switch
            {
                var value when value == CompareMethod.Equal =>
                    Expression.Equal(expressionProperty, ConvertFilterValue(criteria)),

                var value when value == CompareMethod.LessThan =>
                    Expression.LessThan(expressionProperty, ConvertFilterValue(criteria)),

                var value when value == CompareMethod.GreaterThan =>
                    Expression.GreaterThan(expressionProperty, ConvertFilterValue(criteria)),

                var value when value == CompareMethod.LessThanOrEqual =>
                    Expression.LessThanOrEqual(expressionProperty, ConvertFilterValue(criteria)),

                var value when value == CompareMethod.GreaterThanOrEqual =>
                    Expression.GreaterThanOrEqual(expressionProperty, ConvertFilterValue(criteria)),

                var value when value == CompareMethod.Contains =>
                    Expression.Call(expressionProperty, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), ConvertFilterValue(criteria)),

                var value when value == CompareMethod.StartsWith =>
                    Expression.Call(expressionProperty, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), ConvertFilterValue(criteria)),

                var value when value == CompareMethod.In =>
                    Expression.Call(Expression.Constant(criteria.FilterValue), criteria.FilterValue.GetType().GetMethod("Contains", new Type[] { criteria.SourceProperty.PropertyType }), expressionProperty),

                _ => throw new ArgumentException($"Метод сравнения {criteria.CompareMethod.Code} не поддерживается")

                //var value when value == CompareMethod.Contains =>
                //    Expression.Call(ToLower(expressionProperty), typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), Expression.Constant(criteria.FilterValue)),
            };
        }

        private Expression ConvertFilterValue(Criteria criteria)
        {
            if (criteria.FilterValue.GetType() != criteria.SourceProperty.PropertyType)
                return Expression.Convert(Expression.Constant(criteria.FilterValue), criteria.SourceProperty.PropertyType);

            return Expression.Constant(criteria.FilterValue);
        }

        private MethodCallExpression ToLower(MemberExpression expressionProperty)
        {
            return Expression.Call(expressionProperty, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
        }
    }
}
