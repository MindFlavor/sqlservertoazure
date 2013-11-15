using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace ITPCfSQL.Azure
{
    public class SimpleDict
    {
        public int ID { get; set; }
        //public SqlString Txt;
        //public long Intg;
        //public double FloatP;
        //public DateTime DateT;

        //public static implicit operator SqlDataRecord(SimpleDictTable inp)
        //{
        //    SqlDataRecord sdr = new SqlDataRecord(
        //        new SqlMetaData[] {
        //            new SqlMetaData("Txt", SqlDbType.NVarChar),
        //        });

        //    sdr.SetSqlString(0, inp.Txt);

        //    return sdr;
        //}
    }

    public class SimpleDictTable : List<SimpleDict>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord sdr = new SqlDataRecord(
                new SqlMetaData[] {
                        new SqlMetaData("ID", SqlDbType.Int),
                    });

            foreach (SimpleDict sd in this)
            {                
                sdr.SetInt32(0, sd.ID);
                yield return sdr;
            }
        }
    }
}
