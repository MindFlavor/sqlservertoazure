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
            System.Console.WriteLine("Random:");
            Test.Console.TestScatter.DoRandom();
            
            System.Console.WriteLine("Evenly:");
            Test.Console.TestScatter.DoEven();

            //Test.Console.TestEventTimeNormalizer.Do();

            //Test.Console.TestBuffering.Do();
            //Test.Console.TestStreaming.StringSplit();
            //Test.Console.TestStreaming.XMLPlainLevelStreamerFromHTTPS();          
            //Test.Console.TestURIStream.Do();

            //Test.Console.TestQueueACL.Do();
            //Test.Console.TestTable.Do();

            //X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            //certStore.Open(OpenFlags.ReadOnly);

            //foreach(var certInner in certStore.Certificates)
            //{
            //    Console.WriteLine(certInner);
            //}

            return;



        ITPCfSQL.Azure.Internal.InternalMethods.DeleteTable(
            "<<account>>", "<<shared key>>", true,
            "HaOnAzure");

          var res=  ITPCfSQL.Azure.CLR.Utils.GeneratePolicyBlobSharedAccessSignatureURI(
                "https://frcognostorage01.blob.core.windows.net/demoacl",
                "<shared key>>>",
                "c",
                "frcogno Demo ACL Policy");



            //return;

            string certThumb = "671C39F8967B4C10142B9F9393E80B85629551D6";
            Guid gSub = new Guid("8e95e0bb-d7cc-4454-9443-75ca862d34c1");

            X509Certificate2 cert = ITPCfSQL.Azure.CLR.Management.RetrieveCertificateInStore(certThumb);

            var svc = ITPCfSQL.Azure.Internal.Management.GetServices(cert, gSub);
            
            var depl = ITPCfSQL.Azure.Internal.Management.GetDeployments(cert, gSub, "igor2014ctp2");

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
