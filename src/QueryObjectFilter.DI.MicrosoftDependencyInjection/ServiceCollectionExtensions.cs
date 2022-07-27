using Microsoft.Extensions.DependencyInjection;
using QueryObjectFilter.Conversion.ToExpression;
using QueryObjectFilter.Conversion.ToSql;
using QueryObjectFilter.Converting;
using System.Linq.Expressions;

namespace QueryObjectFilter.DI.MicrosoftDependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Добавить QueryObjectFilter с конвертацией в Expression
        /// </summary>
        public static IServiceCollection AddQueryObjectFilterWithExpressionConversion(this IServiceCollection services)
        {
            services.AddSingleton<ICompareMethodProvider<Expression>, ExpressionCompareMethodProvider>();
            services.AddSingleton<IConverterProvider<Expression>, ExpressionConverterProvider>();
            services.AddSingleton<FilterCriteriaExpressionConverter>();
            services.AddSingleton<IFilterCriteriaConverter<Expression>>(sp => sp.GetRequiredService<FilterCriteriaExpressionConverter>());
            services.AddSingleton<IFilterCriteriaExpressionConverter>(sp => sp.GetRequiredService<FilterCriteriaExpressionConverter>());

            return services;
        }

        /// <summary>
        /// Добавить QueryObjectFilter с конвертацией в SQL-строку
        /// </summary>
        public static IServiceCollection AddQueryObjectFilterWithSqlConversion(this IServiceCollection services)
        {
            services.AddSingleton<ICompareMethodProvider<string>, SqlCompareMethodProvider>();
            services.AddSingleton<IConverterProvider<string>, SqlConverterProvider>();
            services.AddSingleton<FilterCriteriaSqlConverter>();
            services.AddSingleton<IFilterCriteriaConverter<string>>(sp => sp.GetRequiredService<FilterCriteriaSqlConverter>());
            services.AddSingleton<IFilterCriteriaSqlConverter>(sp => sp.GetRequiredService<FilterCriteriaSqlConverter>());

            return services;
        }
    }
}
