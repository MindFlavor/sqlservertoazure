using ITPCfSQL.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Console
{
    class ThreadStack
    {
        public Table Table { get; set; }
        public int Idx { get; set; }
        public string Row { get; set; }

        public System.Threading.ManualResetEvent mre = new System.Threading.ManualResetEvent(false);
    }
    class TestTable
    {
        public static void Do()
        {
            //var tables = ITPCfSQL.Azure.Internal.JSON.InternalMethods.QueryTables(
            //    "enosg", "secret", true);

            //var ret= ITPCfSQL.Azure.Internal.JSON.InternalMethods.QueryTable(
            //    "enosg", "secret", true,
            //    "HaOnAzureEndpoint");

            //var tables = ITPCfSQL.Azure.Internal.InternalMethods.CreateTable(
            //    "enosg", "secret", true, "testvs");

            ITPCfSQL.Azure.AzureTableService ats = new ITPCfSQL.Azure.AzureTableService("enosg", "secret", true);
            var lTables = ats.ListTables();

            var table = lTables.FirstOrDefault(item => item.Name == "testload");
            if (table == null)
                table = ats.CreateTable("testload");

            //table.Drop();

            #region Load data
            System.Console.WriteLine("Data load...");
            List<string> lItems = new List<string>();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(
                new System.IO.FileStream("C:\\temp\\SacramentocrimeJanuary2006.csv", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read)))
            {
                string str;
                while ((str = sr.ReadLine()) != null)
                    lItems.Add(str);
            }
            System.Console.WriteLine("Data load completed.");
            #endregion

            // Go Channel!
        }

        private static void Callback(object obj)
        {
            ThreadStack ts = (ThreadStack)obj;
            //System.Console.WriteLine("Started task " + ts.Idx);

            TableEntity te = new TableEntity() { PartitionKey = "00", RowKey = DateTime.Now.Ticks.ToString() + ts.Idx.ToString() };
            te.Attributes.Add("DateTime", DateTime.Now.ToLongTimeString());
            te.Attributes.Add("Hostname", System.Net.Dns.GetHostName());
            te.Attributes.Add("Row", ts.Row);

            ts.Table.InsertOrUpdate(te);

            //System.Console.WriteLine("Ended task " + ts.Idx);

            ts.mre.Set();
        }
    }
}
