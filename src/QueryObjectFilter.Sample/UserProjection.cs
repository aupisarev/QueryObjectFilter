using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryObjectFilter.Sample
{
    public class UserProjection
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string ActiveDirectoryName { get; set; }

        public List<LoginData> Logins { get; set; }
    }

    public class LoginData
    {
        public string LoginValue { get; set; }

        public string LoginProviderName { get; set; }
    }
}
