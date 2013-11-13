using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data.SqlTypes;

namespace ITPCfSQL.Azure.CLR
{
    public class Utils
    {
        [SqlFunction
        (IsDeterministic = true,
        IsPrecise = true,
        DataAccess = DataAccessKind.None,
        SystemDataAccess = SystemDataAccessKind.None)]
        public static string ToXmlDate(DateTime dt)
        {
            return System.Xml.XmlConvert.ToString(dt, System.Xml.XmlDateTimeSerializationMode.Utc);// dt.ToString("o");
        }

        [SqlFunction
            (IsDeterministic = true,
            IsPrecise = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static string ToXmlString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            string ret = str;
            ret = ret.Replace("'", "&apos;");
            ret = ret.Replace("\"", "&quot;");
            ret = ret.Replace(">", "&gt;");
            ret = ret.Replace("<", "&lt;");
            ret = ret.Replace("&", "&amp;");
            return ret;
        }

        [SqlFunction
            (IsDeterministic = true,
            IsPrecise = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static string ToXmlInt64(Int64 i64)
        {
            return System.Xml.XmlConvert.ToString(i64);
        }

        [SqlFunction
            (IsDeterministic = true,
            IsPrecise = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static string ToXmlDouble(double d)
        {
            return System.Xml.XmlConvert.ToString(d);
        }

        [SqlFunction
            (IsDeterministic = true,
            IsPrecise = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static string ToXmlBinary(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder(buffer.Length * 2);
            foreach (byte b in buffer)
                sb.AppendFormat("{0:X2}", b);
            return sb.ToString();
        }

        [SqlFunction
            (IsDeterministic = true,
            IsPrecise = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static string ToXmlGuid(Guid guid)
        {
            return ToXmlBinary(guid.ToByteArray());
        }

        [SqlFunction
        (IsDeterministic = false,
        IsPrecise = true,
        DataAccess = DataAccessKind.Read,
        SystemDataAccess = SystemDataAccessKind.Read)]
        public static string ToXmlStatement(string statement)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection("context connection=true"))
            {
                SqlCommand cmd = new SqlCommand(statement, con);
                using (SqlDataAdapter ada = new SqlDataAdapter(cmd))
                {
                    ada.Fill(dt);
                }
            }

            StringBuilder sb = new StringBuilder();

            System.Xml.XmlWriterSettings xws = new System.Xml.XmlWriterSettings();
            xws.OmitXmlDeclaration = true;

            using (System.Xml.XmlWriter wr = System.Xml.XmlWriter.Create(sb, xws))
            {
                wr.WriteStartElement("ResultSet");

                for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                {
                    wr.WriteStartElement("Record");

                    for (int iCol = 0; iCol < dt.Columns.Count; iCol++)
                    {
                        wr.WriteStartElement(dt.Columns[iCol].ColumnName);

                        if (dt.Rows[iRow][iCol] != DBNull.Value)
                        {
                            if (dt.Rows[iRow][iCol] is Guid)
                                wr.WriteValue(((Guid)dt.Rows[iRow][iCol]).ToString());
                            else
                                wr.WriteValue(dt.Rows[iRow][iCol]);
                        }

                        wr.WriteEndElement();
                    }

                    wr.WriteEndElement();
                }

                wr.WriteEndElement();

                wr.Flush();
            }

            return sb.ToString();
        }


        [SqlFunction
        (IsDeterministic = true,
        IsPrecise = true,
        DataAccess = DataAccessKind.None,
        SystemDataAccess = SystemDataAccessKind.None)]
        public static SqlString ComputeMD5AsBase64(SqlBinary byteArray)
        {
            if (byteArray.IsNull)
                return SqlString.Null;

            System.Security.Cryptography.MD5 sscMD5 = System.Security.Cryptography.MD5.Create();
            byte[] mHash = sscMD5.ComputeHash(byteArray.Value);
            return Convert.ToBase64String(mHash);
        }

        #region Non exported methods
        internal static void PushSingleRecordResult(object result, System.Data.SqlDbType sqlDBType)
        {
            //SqlContext.Pipe.Send("Response output:\n");
            //SqlContext.Pipe.Send(result.ToString());

            SqlDataRecord record = null;

            switch (sqlDBType)
            {
                case System.Data.SqlDbType.NVarChar:
                case System.Data.SqlDbType.VarChar:
                    record = new SqlDataRecord(new SqlMetaData[] { new SqlMetaData("Result", sqlDBType, -1) });
                    record.SetString(0, result.ToString());
                    break;
                case System.Data.SqlDbType.Xml:
                    record = new SqlDataRecord(new SqlMetaData[] { new SqlMetaData("Result", sqlDBType) });

                    SqlXml xml;
                    using(System.Xml.XmlReader reader = System.Xml.XmlReader.Create(new System.IO.StringReader(result.ToString())))
                    {
                        xml = new SqlXml(reader);
                    }
                    
                    record.SetSqlXml(0, xml);
                    break;
                case System.Data.SqlDbType.Int:
                    record = new SqlDataRecord(new SqlMetaData[] { new SqlMetaData("Result", sqlDBType) });
                    record.SetInt32(0, (Int32)result);
                    break;
                default:
                    throw new ArgumentException("SqlDbType " + sqlDBType.ToString() + " is not supported by PushSingleRecordResult.");
            }

            SqlContext.Pipe.SendResultsStart(record);
            SqlContext.Pipe.SendResultsRow(record);
            SqlContext.Pipe.SendResultsEnd();
        }
        #endregion

        #region Blog methods
        [SqlFunction
            (IsDeterministic = true,
            IsPrecise = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static string NoAccess(string str)
        {
            return str.ToUpper();
        }

        [SqlFunction
            (IsDeterministic = true,
            IsPrecise = true,
            DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static string WithAccess(string str)
        {
            return str.ToUpper();
        }
        #endregion
    }
}
