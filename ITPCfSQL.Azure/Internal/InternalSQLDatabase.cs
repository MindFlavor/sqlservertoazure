using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Internal
{
    public class InternalSQLDatabase
    {
        public const string AZURE_VERSION = "2013-06-01";
        internal static string GetManagementURI()
        {
            return "https://management.core.windows.net:8443";
        }

        #region List Servers
        public static Responses.SQLDatabase.ListServers.ResponseWithGeneric ListServersWithGeneric(
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate,
            Guid subscriptionId)
        {
            Uri uri = new Uri(
                GetManagementURI() + "/" +
                subscriptionId.ToString() + "/" +
                "services" + "/" +
                "sqlservers/servers?contentview=generic");

            return InternalMethods.PerformSimpleGet<Responses.SQLDatabase.ListServers.ResponseWithGeneric>(uri, certificate, AZURE_VERSION);
        }

        public static Responses.SQLDatabase.ListServers.ResponseWithoutGeneric ListServersWithoutGeneric(
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate,
            Guid subscriptionId)
        {
            Uri uri = new Uri(
                GetManagementURI() + "/" +
                subscriptionId.ToString() + "/" +
                "services" + "/" +
                "sqlservers/servers");

            return InternalMethods.PerformSimpleGet<Responses.SQLDatabase.ListServers.ResponseWithoutGeneric>(uri, certificate, AZURE_VERSION);
        }
        #endregion


        public static Responses.SQLDatabase.ListDatabases.Response ListDatabases  (
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate,
            Guid subscriptionId,
            string serverName)
        {
            Uri uri = new Uri(
                GetManagementURI() + "/" +
                subscriptionId.ToString() + "/" +
                "services" + "/" +
                "sqlservers/servers/" +
                serverName + "/" +
                "databases?contentview=generic");

            return InternalMethods.PerformSimpleGet<Responses.SQLDatabase.ListDatabases.Response>(uri, certificate, AZURE_VERSION);
        }

        public static Responses.SQLDatabase.ListDatabases.Response GetDataMaskingPolicy(
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate,
            Guid subscriptionId,
            string serverName)
        {
            Uri uri = new Uri(
                GetManagementURI() + "/" +
                subscriptionId.ToString() + "/" +
                "services" + "/" +
                "sqlservers/servers/" +
                serverName + "/" +
                "databases?contentview=generic");

            return InternalMethods.PerformSimpleGet<Responses.SQLDatabase.ListDatabases.Response>(uri, certificate, AZURE_VERSION);
        }
    }
}
