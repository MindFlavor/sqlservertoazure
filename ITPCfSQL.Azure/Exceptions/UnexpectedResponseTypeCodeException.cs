using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITPCfSQL.Azure.Exceptions
{
    public class UnexpectedResponseTypeCodeException:AzureException
    {
        public System.Net.HttpStatusCode ExpectedStatusCode;
        public System.Net.HttpStatusCode ReceivedStatusCode;


        public UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode expectedStatusCode, System.Net.HttpStatusCode receivedStatusCode)
            : base(string.Format( "Unexpected status code response recived. Expected {0:S}, received {1:S}.", 
            expectedStatusCode.ToString(), receivedStatusCode.ToString()), null)
        {
            this.ExpectedStatusCode = expectedStatusCode;
            this.ReceivedStatusCode = receivedStatusCode;
        }
    }
}
