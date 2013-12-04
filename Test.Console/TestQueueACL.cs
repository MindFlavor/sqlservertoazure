using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITPCfSQL.Azure.Internal;
using ITPCfSQL.Azure.SharedAccessSignature;
using ITPCfSQL.Azure;
using ITPCfSQL.Azure.Enumerations;

namespace Test.Console
{
    internal class TestQueueACL
    {
        public static void Do()
        {
            string accountName = "enosg";
            string sharedKey = "oooo+ps!";
            bool useHTTPS = true;

            string contName = "testsas";

            SharedAccessSignatureACL queueACL = new SharedAccessSignatureACL();
            queueACL.SignedIdentifier = new List<SignedIdentifier>();

            queueACL.SignedIdentifier.Add(new SignedIdentifier()
            {
                Id = "sisisisisisisvvv",
                AccessPolicy = new AccessPolicy()
                {
                    Start = DateTime.Now.AddYears(-1),
                    Expiry = DateTime.Now.AddYears(1),
                    Permission = "rwd"
                }
            });

            queueACL.SignedIdentifier.Add(new SignedIdentifier()
            {
                Id = "secondsigid",
                AccessPolicy = new AccessPolicy()
                {
                    Start = DateTime.Now.AddYears(-10),
                    Expiry = DateTime.Now.AddYears(10),
                    Permission = "rwdl"
                }
            });

            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(queueACL.GetType());

            // use this two lines to get rid of those useless namespaces :).
            System.Xml.Serialization.XmlSerializerNamespaces namespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (System.IO.FileStream fs = new System.IO.FileStream("C:\\temp\\QueueACL.xml", System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read))
            {
                ser.Serialize(fs, queueACL, namespaces);
            }

            using (System.IO.FileStream fs = new System.IO.FileStream("C:\\temp\\QueueACL.xml", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                queueACL = (SharedAccessSignatureACL)ser.Deserialize(fs);
            }

            var result = InternalMethods.SetContainerACL(accountName, sharedKey, useHTTPS, contName, null, queueACL, ITPCfSQL.Azure.Enumerations.ContainerPublicReadAccess.Blob);

            //return;

            AzureBlobService abs = new AzureBlobService(accountName, sharedKey, useHTTPS);

            List<Container> lConts = abs.ListContainers();
            if (lConts.FirstOrDefault(item => item.Name == contName) == null)
            {
                abs.CreateContainer("testsas", ITPCfSQL.Azure.Enumerations.ContainerPublicReadAccess.Blob);
                lConts = abs.ListContainers();
            }

            Container cTest = lConts.First(item => item.Name == contName);

            ContainerPublicReadAccess containerPublicAccess;
            var output = InternalMethods.GetContainerACL(accountName, sharedKey, useHTTPS, contName, out containerPublicAccess);

        }
    }
}
