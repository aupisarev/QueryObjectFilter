using QueryObjectFilter.Filtration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Converting
{
    /// <summary>
    /// Поставщик методов сравнения значений
    /// </summary>
    public interface ICompareMethodProvider<T>
    {
        /// <summary>
        /// Получить сравнение
        /// </summary>
        /// <param name="criteria">Критерий</param>
        /// <param name="parameter">Параметр, на который построен критерий</param>
        /// <returns>Объект сравнения</returns>
        T GetComparation(Criteria criteria, T parameter);
    }
}
