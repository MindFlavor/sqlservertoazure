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
            //System.Console.WriteLine("Random:");
            //Test.Console.TestScatter.DoRandom();
<<<<<<< HEAD
=======

>>>>>>> f7ce2cad41f9ca33a7b562dcbd7b89819cfcc9ee
            //System.Console.WriteLine("Evenly:");
            //Test.Console.TestScatter.DoEven();

            //Test.Console.TestEventTimeNormalizer.Do();

            //Test.Console.TestBuffering.Do();
            //Test.Console.TestStreaming.StringSplit();
            //Test.Console.TestStreaming.XMLPlainLevelStreamerFromHTTPS();          
            //Test.Console.TestURIStream.Do();

            //Test.Console.TestQueueACL.Do();
            //Test.Console.TestTable.Do();

            string thumbprint = "‎A7B4A6B1B2FCA78E0AEC4308FB91AC2C6215D31D";

            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);

            X509Certificate2 cert = null;

            foreach (var certInner in certStore.Certificates)
            {
                if (certInner.Thumbprint.Equals(thumbprint, StringComparison.InvariantCultureIgnoreCase))
                {
                    cert = certInner;
                    break;
                }
            }

            if (cert == null)
            {
                throw new Exception("Not found!");
            }
            certStore.Close();

            Guid gSubscription = Guid.Parse("002842b4-7fb0-4fd2-b269-1959fa60b89d");

            //var serverWith = InternalSQLDatabase.ListServersWithGeneric(cert, gSubscription);
            //var serverWithout = InternalSQLDatabase.ListServersWithoutGeneric(cert, gSubscription);

            //var dbs = InternalSQLDatabase.ListDatabases(cert, gSubscription, "ep014xe0is");

            var r = InternalResourceGroups.ListResourceGroups(cert, gSubscription);

            return;
        }
    }
}
