using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITPCfSQL.Azure.Exceptions
{
    [Serializable]
    public class AzureException :Exception
    {
        public AzureException() { }

        public AzureException(string txt, Exception innerException)
            : base(txt, innerException) { }
    }
}
