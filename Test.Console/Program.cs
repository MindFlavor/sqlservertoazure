using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITPCfSQL.Azure.Internal;
using ITPCfSQL.Azure;
using System.Web;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Test.Console.TestQueueACL.Do();

            return;

            string certThumb = "xxxxx";
            Guid gSub = new Guid("00000000-d7cc-4454-9443-75ca862d34c1");

            X509Certificate2 cert = ITPCfSQL.Azure.CLR.Management.RetrieveCertificateInStore(certThumb);

            ITPCfSQL.Azure.CLR.Management.RemoveEndpointFromPersistentVM(
               certThumb,
               gSub,
               "xx2014ctp2",
               "Production",
               "xx2014ctp3",
               "TestEndpoint",
               true);

            ITPCfSQL.Azure.CLR.Management.AddInputEndpointToPersistentVM(
                certThumb, 
                gSub,
                "xx2014ctp2",
                "Production",
                "xx2014ctp3",
                "TestEndpoint",
                4000,
                false,
                4000,
                "tcp",
                "10.0.0.1",
                true);


        }
    }
}
