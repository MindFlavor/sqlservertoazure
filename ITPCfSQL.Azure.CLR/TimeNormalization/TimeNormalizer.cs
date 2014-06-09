using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using ITPCfSQL.Azure.TimeNormalization;
using System.Data.SqlClient;

namespace ITPCfSQL.Azure.CLR.TimeNormalization
{
    public class TimeNormalizer
    {
        [SqlFunction(
                  DataAccess = DataAccessKind.Read,
                  SystemDataAccess = SystemDataAccessKind.Read,
                  FillRowMethodName = "_TimeNormalize",
                  IsDeterministic = false,
                  IsPrecise = true,
                  TableDefinition = (@"EventTime DATETIME, NormalizedValue FLOAT"))]
        public static System.Collections.IEnumerable TimeNormalize(SqlString statement, SqlInt32 days, SqlInt32 hours, SqlInt32 minutes, SqlInt32 seconds, SqlInt32 milliseconds)
        {
            using (SqlConnection conn = new SqlConnection("context connection = true"))
            {
                conn.Open();
                List<DateValuePair> lDVPs = new List<DateValuePair>();

                StreamNormalizer sn = new StreamNormalizer(new TimeSpan(
                    days.Value, hours.Value, minutes.Value, seconds.Value, milliseconds.Value));

                using (SqlCommand cmd = new SqlCommand(statement.Value, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!(reader[0] is DateTime))
                                throw new ArgumentException("Field 0 must be a " + typeof(DateTime).FullName + ". It is " + reader[0].GetType().FullName + " instead.");

                            DateValuePair dvp = new DateValuePair { Date = reader.GetDateTime(0) };
                            object oVal = reader[1];

                            try
                            {
                                if (oVal is Int32)
                                    dvp.Value = (int)oVal;
                                else if (oVal is double)
                                    dvp.Value = (double)oVal;
                                else
                                    dvp.Value = Decimal.ToDouble((Decimal)oVal);
                            }
                            catch (Exception exce)
                            {
                                throw new ArgumentException("Field 1 must be a numeric value. It is " + oVal.GetType().FullName + " instead. (Internal exception: " + exce.Message + ").");
                            }

                            lDVPs.AddRange(sn.Push(dvp));
                        }
                    }
                }

                DateValuePair dvpFinish = sn.Finish();

                if (dvpFinish != null)
                    lDVPs.Add(dvpFinish);

                return lDVPs;
            }
        }

        protected static void _TimeNormalize(object obj,
            out SqlDateTime EventTime, out SqlDouble NormalizedValue)
        {
            EventTime = SqlDateTime.Null;
            NormalizedValue = SqlDouble.Null;

            DateValuePair dvp = obj as DateValuePair;

            EventTime = dvp.Date;
            NormalizedValue = dvp.Value;
        }


        #region Type test
        [SqlFunction(
          DataAccess = DataAccessKind.Read,
          SystemDataAccess = SystemDataAccessKind.Read,
          FillRowMethodName = "_TimeNormalize_TestType",
          IsDeterministic = false,
          IsPrecise = true,
          TableDefinition = (@"Result NVARCHAR(MAX)"))]
        public static System.Collections.IEnumerable TimeNormalize_TestType(SqlString statement, SqlInt32 days, SqlInt32 hours, SqlInt32 minutes, SqlInt32 seconds, SqlInt32 milliseconds)
        {
            using (SqlConnection conn = new SqlConnection("context connection = true"))
            {
                conn.Open();
                List<string> lDVPs = new List<string>();

                StreamNormalizer sn = new StreamNormalizer(new TimeSpan(
                    days.Value, hours.Value, minutes.Value, seconds.Value, milliseconds.Value));

                using (SqlCommand cmd = new SqlCommand(statement.Value, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lDVPs.Add(reader[0].GetType().FullName + " - " + reader[1].GetType().FullName);
                        }
                    }
                }


                return lDVPs;
            }
        }

        protected static void _TimeNormalize_TestType(object obj,
            out string ret)
        {
            ret = (string)obj;
        }
        #endregion

    }
}
