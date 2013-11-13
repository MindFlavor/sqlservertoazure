using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.CLR.Configuration
{
    public class EmbeddedConfiguration
    {
        #region Members
        public string LogicalName { get; set; }
        public string AccountName { get; set; }
        public string SharedKey { get; set; }
        public bool UseHTTPS { get; set; }
        #endregion

        public static EmbeddedConfiguration GetConfigurationFromEmbeddedFile(string logicalConnectionName)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            using (System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ITPCfSQL.Azure.CLR.Configuration.Connections.xml"))
            {
                doc.Load(s);
            }

            string query = "ConnectionList/Connection[LogicalName='" + logicalConnectionName + "']";
            
            //Microsoft.SqlServer.Server.SqlContext.Pipe.Send(query);

            System.Xml.XmlNode node = doc.SelectSingleNode(query);

            if (node == null)
                throw new ArgumentException("Cannot find \"" + logicalConnectionName + "\" logical connection. Are you sure you spelled it right?");

            try
            {
                return new EmbeddedConfiguration()
            {
                LogicalName = node.SelectSingleNode("LogicalName").InnerText,
                AccountName = node.SelectSingleNode("AccountName").InnerText,
                SharedKey = node.SelectSingleNode("SharedKey").InnerText,
                UseHTTPS = bool.Parse(node.SelectSingleNode("UseHTTPS").InnerText)
            };
            }
            catch (Exception exce)
            {
                throw new ArgumentException("There is an error in the configuration file. Some node is missing.", exce);
            }
        }
    }
}
