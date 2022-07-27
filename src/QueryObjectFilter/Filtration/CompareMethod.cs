using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Filtration
{
    /// <summary>
    /// Метод сравнения двух значений
    /// </summary>
    public class CompareMethod
    {
        private CompareMethod(string code)
        {
            Code = code;
        }

        /// <summary>
        /// Код сравнения
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Равно
        /// </summary>
        public static readonly CompareMethod Equal = new CompareMethod("Equal");

        /// <summary>
        /// Содержит
        /// </summary>
        public static readonly CompareMethod Contains = new CompareMethod("Contains");

        /// <summary>
        /// Начинается с
        /// </summary>
        public static readonly CompareMethod StartsWith = new CompareMethod("StartsWith");

        /// <summary>
        /// Больше чем
        /// </summary>
        public static readonly CompareMethod GreaterThan = new CompareMethod("GreaterThan");

        /// <summary>
        /// Больше или равно
        /// </summary>
        public static readonly CompareMethod GreaterThanOrEqual = new CompareMethod("GreaterThanOrEqual");

        /// <summary>
        /// Меньше чем
        /// </summary>
        public static readonly CompareMethod LessThan = new CompareMethod("LessThan");

        /// <summary>
        /// Меньше или равно
        /// </summary>
        public static readonly CompareMethod LessThanOrEqual = new CompareMethod("LessThanOrEqual");

        /// <summary>
        /// Включен в коллекцию
        /// </summary>
        public static readonly CompareMethod In = new CompareMethod("In");
    }
}
