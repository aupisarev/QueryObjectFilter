using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Filtration
{
    /// <summary>
    /// Операция комбинации критериев
    /// </summary>
    public class GroupOperation
    {
        private GroupOperation(string code)
        {
            Code = code;
        }

        /// <summary>
        /// Код операции
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Комбинация через "И"
        /// </summary>
        public static readonly GroupOperation And = new GroupOperation("And");

        /// <summary>
        /// Комбинация через "ИЛИ"
        /// </summary>
        public static readonly GroupOperation Or = new GroupOperation("Or");
    }
}
