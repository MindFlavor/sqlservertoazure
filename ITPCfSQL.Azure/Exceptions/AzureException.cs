using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Exceptions
{
    public class AzureException :Exception
    {
        public AzureException() { }

        public AzureException(string txt, Exception innerException)
            : base(txt, innerException) { }
    }
}
