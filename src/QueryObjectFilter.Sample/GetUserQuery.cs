using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Sample
{
    public class GetUserQuery
    {
        public long? Id { get; set; } //все свойства запроса nullable - если значения свойства нет, фильтрация по этому свойству не происходит

        public string Email { get; set; } //имена свойств запроса лучше делать равными именам результата для упрощения кода

        public string Name { get; set; } //если имена различаются, нужно будет явно указывать фильтр

        public string ActiveDirectoryName { get; set; }

        public string Login { get; set; }

        public string LoginProviderName { get; set; }

        public List<int> Statuses { get; set; } //может быть коллекция возможных значений

        public int? Skip { get; set; } //в запросе могут быть дополнительные данные, не относящиеся к фильтрации

        public int? Take { get; set; }
    }
}
