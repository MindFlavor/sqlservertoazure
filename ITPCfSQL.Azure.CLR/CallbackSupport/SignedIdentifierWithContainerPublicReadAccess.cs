using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.CLR.CallbackSupport
{
    public class SignedIdentifierWithContainerPublicReadAccess
    {
        public SharedAccessSignature.SignedIdentifier SignedIdentifier { get; set; }
        public Enumerations.ContainerPublicReadAccess ContainerPublicReadAccess { get; set; }
    }
}
