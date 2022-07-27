using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Filtration
{
    /// <summary>
    /// Критерий сравнения свойств фильтруемого объекта и фильтра
    /// </summary>
    public class Criteria
    {
        private readonly PropertyInfo sourceProperty;
        private readonly object filterValue;
        private readonly CompareMethod compareMethod;

        /// <summary>
        /// Критерий сравнения свойств фильтруемого объекта и фильтра
        /// </summary>
        /// <param name="sourceProperty">Свойство фильтруемого объекта для сравнения</param>
        /// <param name="filterValue">Значение фильтра</param>
        /// <param name="compareMethod">Метод сравнения значений</param>
        public Criteria(PropertyInfo sourceProperty, object filterValue, CompareMethod compareMethod)
        {
            this.filterValue = filterValue ?? throw new ArgumentNullException(nameof(filterValue));
            this.compareMethod = compareMethod ?? throw new ArgumentNullException(nameof(compareMethod));
            this.sourceProperty = sourceProperty ?? throw new ArgumentNullException(nameof(sourceProperty));
        }

        /// <summary>
        /// Свойство фильтруемого объекта для сравнения
        /// </summary>
        public PropertyInfo SourceProperty => sourceProperty;

        /// <summary>
        /// Значение фильтра
        /// </summary>
        public object FilterValue => filterValue;

        /// <summary>
        /// Метод сравнения значений
        /// </summary>
        public CompareMethod CompareMethod => compareMethod;
    }
}
