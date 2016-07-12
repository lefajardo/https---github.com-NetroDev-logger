﻿/**********************************************************************************************************************
 * The contents of this file are subject to the SugarCRM Public License Version 1.1.3 ("License"); You may not use this
 * file except in compliance with the License. You may obtain a copy of the License at http://www.sugarcrm.com/SPL
 * Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 * express or implied.  See the License for the specific language governing rights and limitations under the License.
 *
 * All copies of the Covered Code must include on each user interface screen:
 *    (i) the "Powered by SugarCRM" logo and
 *    (ii) the SugarCRM copyright notice
 *    (iii) the SplendidCRM copyright notice
 * in the same form as they appear in the distribution.  See full license for requirements.
 *
 * The Original Code is: SplendidCRM Open Source
 * The Initial Developer of the Original Code is SplendidCRM Software, Inc.
 * Portions created by SplendidCRM Software are Copyright (C) 2005 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;

using System.Data;
using System.Data.Common;
using System.Xml;
using System.Text;
using System.Globalization;
using System.Diagnostics;
//using Microsoft.VisualBasic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

using System.Web;

namespace NetroLogger_WorkerRole
{
    /// <summary>
    /// Summary description for Sql.
    /// </summary>
    public class Sql
    {
        public static string EncodeKey(string key)
        {
            if (key == null)
            {
                return null;
            }

            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(key));
            // HttpServerUtility.UrlTokenEncode(

        }

        public static string DecodeKey(string encodedKey)
        {
            if (encodedKey == null)
            {
                return null;
            }

            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedKey));
        }

        ///<summary>
        /// Base 64 Encoding with URL and Filename Safe Alphabet using UTF-8 character set.
        ///</summary>
        ///<param name="str">The origianl string</param>
        ///<returns>The Base64 encoded string</returns>
        public static string Base64UrlEncode(string str)
        {
            byte[] encbuff = Encoding.UTF8.GetBytes(str);
            return HttpServerUtility.UrlTokenEncode(encbuff);
        }
        ///<summary>
        /// Decode Base64 encoded string with URL and Filename Safe Alphabet using UTF-8.
        ///</summary>
        ///<param name="str">Base64 code</param>
        ///<returns>The decoded string.</returns>
        public static string Base64UrlDecode(string str)
        {
            byte[] decbuff = HttpServerUtility.UrlTokenDecode(str);
            return Encoding.UTF8.GetString(decbuff);
        }

        public static bool isDate(string str)
        {
            try
            {
                DateTime dt = DateTime.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDecimal(string theValue)
        {
            try
            {
                Convert.ToDouble(theValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string formatTime(double seconds)
        {
            long ticksForSecond = 10000000;
            string retValue = ""; // seconds.ToString();
            TimeSpan ts = new TimeSpan((long)seconds * ticksForSecond);

            if (ts.Days > 0)
            {
                retValue += ts.Days.ToString("#0") + " Days ";
            }
            if (ts.Hours > 0)
            {
                retValue += ts.Hours.ToString("#0") + (ts.Hours > 1 ? " Hours " : " Hour ");
            }

            if (ts.Minutes > 0)
            {
                retValue += ts.Minutes.ToString("#0") + " Min. ";
            }

            if (ts.Seconds > 0)
            {
                retValue += ts.Seconds.ToString("#0") + " Sec. ";
            }


            if (retValue.Length == 0)
            {
                retValue = "Less than 1 Sec";
            }

            return retValue;



        }

        public static string HexEncode(byte[] aby)
        {
            string hex = "0123456789abcdef";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < aby.Length; i++)
            {
                sb.Append(hex[(aby[i] & 0xf0) >> 4]);
                sb.Append(hex[aby[i] & 0x0f]);
            }
            return sb.ToString();
        }

        public static string FormatSQL(string s, int nMaxLength)
        {
            if (Sql.IsEmptyString(s))
                s = "null";
            else
                s = "'" + Sql.EscapeSQL(s) + "'";
            if (nMaxLength > s.Length)
                return s + Strings.Space(nMaxLength - s.Length);
            return s;
        }

        public static string EscapeSQL(string str)
        {
            str = str.Replace("\'", "\'\'");
            return str;
        }

        public static string EscapeSQLLike(string str)
        {
            str = str.Replace(@"\", @"\\");
            str = str.Replace("%", @"\%");
            str = str.Replace("_", @"\_");
            return str;
        }

        public static string EscapeJavaScript(string str)
        {
            str = str.Replace(@"\", @"\\");
            str = str.Replace("\'", "\\\'");
            str = str.Replace("\"", "\\\"");
            // 07/31/2006 Paul.  Stop using VisualBasic library to increase compatibility with Mono. 
            //str = str.Replace("\t", "\\t");
            //str = str.Replace("\r", "\\r");
            //str = str.Replace("\n", "\\n");
            return str;
        }

        public static string MakeWebSafe(string str)
        {
            str = EscapeSQL(str);
            str = EscapeSQLLike(str);
            str = EscapeJavaScript(str);
            return str;
        }

        public static bool IsEmptyString(string str)
        {
            if (str == null || str == String.Empty)
                return true;
            return false;
        }

        public static bool IsEmptyString(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return true;
            if (obj.ToString() == String.Empty)
                return true;
            return false;
        }

        public static string ToString(string str)
        {
            if (str == null)
                return String.Empty;
            return str;
        }

        public static string ToString(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return String.Empty;
            return obj.ToString();
        }

        public static object ToDBString(string str)
        {
            if (str == null)
                return DBNull.Value;
            if (str == String.Empty)
                return DBNull.Value;
            return str;
        }

        public static object ToDBString(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return DBNull.Value;
            string str = obj.ToString();
            if (str == String.Empty)
                return DBNull.Value;
            return str;
        }

        public static byte[] ToBinary(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return new byte[0];
            return (byte[])obj;
        }

        public static object ToDBBinary(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return DBNull.Value;
            return obj;
        }

        public static object ToDBBinary(byte[] aby)
        {
            if (aby == null)
                return DBNull.Value;
            else if (aby.Length == 0)
                return DBNull.Value;
            return aby;
        }

        public static DateTime ToDateTime(DateTime dt)
        {
            return dt;
        }

        public static DateTime ToDateTime(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return DateTime.MinValue;
            // If datatype is DateTime, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.DateTime"))
                return Convert.ToDateTime(obj);
            if (!Information.IsDate(obj))
                return DateTime.MinValue;
            return Convert.ToDateTime(obj);
        }

        public static string ToDateString(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return String.Empty;
            // If datatype is DateTime, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.DateTime"))
                return Convert.ToDateTime(obj).ToShortDateString();
            if (!Information.IsDate(obj))
                return String.Empty;
            return Convert.ToDateTime(obj).ToShortDateString();
        }

        // 05/27/2007 Paul.  It looks better to show nothing than to show 01/01/0001 12:00:00 AM. 
        public static string ToString(DateTime dt)
        {
            // If datatype is DateTime, then nothing else is necessary. 
            if (dt == DateTime.MinValue)
                return String.Empty;
            return dt.ToString();
        }

        public static string ToDateString(DateTime dt)
        {
            // If datatype is DateTime, then nothing else is necessary. 
            if (dt == DateTime.MinValue)
                return String.Empty;
            return dt.ToShortDateString();
        }

        public static string ToTimeString(DateTime dt)
        {
            // If datatype is DateTime, then nothing else is necessary. 
            if (dt == DateTime.MinValue)
                return String.Empty;
            return dt.ToShortTimeString();
        }

        public static object ToDBDateTime(DateTime dt)
        {
            if (dt == DateTime.MinValue)
                return DBNull.Value;
            return dt;
        }

        public static object ToDBDateTime(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return DBNull.Value;
            if (!Information.IsDate(obj))
                return DBNull.Value;
            DateTime dt = Convert.ToDateTime(obj);
            if (dt == DateTime.MinValue)
                return DBNull.Value;
            return dt;
        }

        public static bool IsEmptyGuid(Guid g)
        {
            if (g == Guid.Empty)
                return true;
            return false;
        }

        public static bool IsEmptyGuid(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return true;
            string str = obj.ToString();
            if (str == String.Empty)
                return true;
            Guid g = XmlConvert.ToGuid(str);
            if (g == Guid.Empty)
                return true;
            return false;
        }

        public static Guid ToGuid(Guid g)
        {
            return g;
        }

        public static Guid ToGuid(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return Guid.Empty;
            // If datatype is Guid, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.Guid"))
                return (Guid)obj;
            // 08/09/2005 Paul.  Oracle returns RAW(16). 
            // 08/10/2005 Paul.  Attempting to use RAW has too many undesireable consequences.  Use CHAR(36) instead. 
            /*
            if ( obj.GetType() == Type.GetType("System.Byte[]") )
            {
                //MemoryStream ms = new MemoryStream(16);
                //BinaryFormatter b = new BinaryFormatter();
                //b.Serialize(ms, obj);
                //return new Guid(ms.ToArray());
                //Byte[] b = (System.Array) obj;
                System.Array a = obj as System.Array;
                Byte[] b = a as Byte[];
                //return new Guid(b);
                // 08/09/2005 Paul.  Convert byte array to a true Guid. 
                Guid g = new Guid((b[0]+(b[1]+(b[2]+b[3]*256)*256)*256),(short)(b[4]+b[5]*256),(short)(b[6]+b[7]*256),b[8],b[9],b[10],b[11],b[12],b[13],b[14],b[15]);
                return g;
            }
            */
            string str = obj.ToString();
            if (str == String.Empty)
                return Guid.Empty;
            return XmlConvert.ToGuid(str);
        }

        public static object ToDBGuid(Guid g)
        {
            if (g == Guid.Empty)
                return DBNull.Value;
            return g;
        }

        public static object ToDBGuid(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return DBNull.Value;
            // If datatype is Guid, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.Guid"))
                return obj;
            string str = obj.ToString();
            if (str == String.Empty)
                return DBNull.Value;
            Guid g = XmlConvert.ToGuid(str);
            if (g == Guid.Empty)
                return DBNull.Value;
            return g;
        }


        public static Int32 ToInteger(Int32 n)
        {
            return n;
        }

        public static Int32 ToInteger(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return 0;
            // If datatype is Integer, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.Int32"))
                return Convert.ToInt32(obj);
            else if (obj.GetType() == Type.GetType("System.Boolean"))
                return (Int32)(Convert.ToBoolean(obj) ? 1 : 0);
            else if (obj.GetType() == Type.GetType("System.Single"))
                return Convert.ToInt32(Math.Floor((System.Single)obj));
            string str = obj.ToString();
            if (str == String.Empty)
                return 0;
            return Int32.Parse(str, NumberStyles.Any);
        }

        // 01/05/2009 Paul.  Some NetroMedia custom fields use 64-bit integers. 
        public static Int64 ToInt64(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return 0;
            // If datatype is Integer, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.Int64"))
                return Convert.ToInt64(obj);
            string str = obj.ToString();
            if (str == String.Empty)
                return 0;
            return Int64.Parse(str, NumberStyles.Any);
        }

        public static long ToLong(long n)
        {
            return n;
        }

        public static long ToLong(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return 0;
            // If datatype is Integer, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.Int64"))
                return Convert.ToInt64(obj);
            string str = obj.ToString();
            if (str == String.Empty)
                return 0;
            return Int64.Parse(str, NumberStyles.Any);
        }

        public static short ToShort(short n)
        {
            return n;
        }

        public static short ToShort(int n)
        {
            return (short)n;
        }

        public static short ToShort(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return 0;
            // 12/02/2005 Paul.  Still need to convert object to Int16. Cast to short will not work. 
            if (obj.GetType() == Type.GetType("System.Int32") || obj.GetType() == Type.GetType("System.Int16"))
                return Convert.ToInt16(obj);
            string str = obj.ToString();
            if (str == String.Empty)
                return 0;
            return Int16.Parse(str, NumberStyles.Any);
        }

        public static object ToDBInteger(Int32 n)
        {
            return n;
        }

        public static object ToDBInteger(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return DBNull.Value;
            // If datatype is Integer, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.Int32"))
                return obj;
            string str = obj.ToString();
            if (str == String.Empty || !Information.IsNumeric(str))
                return DBNull.Value;
            return Int32.Parse(str, NumberStyles.Any);
        }


        public static float ToFloat(float f)
        {
            return f;
        }

        public static float ToFloat(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return 0;
            // If datatype is Double, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.Double"))
                return (float)Convert.ToSingle(obj);
            string str = obj.ToString();
            if (str == String.Empty || !Information.IsNumeric(str))
                return 0;
            return float.Parse(str, NumberStyles.Any);
        }

        public static float ToFloat(string str)
        {
            if (str == null)
                return 0;
            if (str == String.Empty || !Information.IsNumeric(str))
                return 0;
            return float.Parse(str, NumberStyles.Any);
        }

        public static object ToDBFloat(float f)
        {
            return f;
        }

        public static object ToDBFloat(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return DBNull.Value;
            // If datatype is Double, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.Double"))
                return obj;
            string str = obj.ToString();
            if (str == String.Empty || !Information.IsNumeric(str))
                return DBNull.Value;
            return float.Parse(str, NumberStyles.Any);
        }


        public static double ToDouble(double d)
        {
            return d;
        }

        public static double ToDouble(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return 0;
            // If datatype is Double, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.Double"))
                return Convert.ToDouble(obj);
            string str = obj.ToString();
            if (str == String.Empty || !Information.IsNumeric(str))
                return 0;
            return double.Parse(str, NumberStyles.Any);
        }

        public static double ToDouble(string str)
        {
            if (str == null)
                return 0;
            if (str == String.Empty || !Information.IsNumeric(str))
                return 0;
            return double.Parse(str, NumberStyles.Any);
        }


        public static Decimal ToDecimal(Decimal d)
        {
            return d;
        }

        public static Decimal ToDecimal(double d)
        {
            return Convert.ToDecimal(d);
        }

        public static Decimal ToDecimal(float f)
        {
            return Convert.ToDecimal(f);
        }

        public static Decimal ToDecimal(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return 0;
            // If datatype is Decimal, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.Decimal"))
                return Convert.ToDecimal(obj);
            string str = obj.ToString();
            if (str == String.Empty)
                return 0;
            return Decimal.Parse(str, NumberStyles.Any);
        }

        public static object ToDBDecimal(Decimal d)
        {
            return d;
        }

        public static object ToDBDecimal(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return DBNull.Value;
            // If datatype is Decimal, then nothing else is necessary. 
            if (obj.GetType() == Type.GetType("System.Decimal"))
                return obj;
            string str = obj.ToString();
            if (str == String.Empty || !Information.IsNumeric(str))
                return DBNull.Value;
            return Decimal.Parse(str, NumberStyles.Any);
        }


        public static Boolean ToBoolean(Boolean b)
        {
            return b;
        }

        public static Boolean ToBoolean(Int32 n)
        {
            return (n == 0) ? false : true;
        }

        public static Boolean ToBoolean(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return false;
            if (obj.GetType() == Type.GetType("System.Int32"))
                return (Convert.ToInt32(obj) == 0) ? false : true;
            // 01/15/2007 Paul.  Allow a Byte field to also be treated as a boolean. 
            if (obj.GetType() == Type.GetType("System.Byte"))
                return (Convert.ToByte(obj) == 0) ? false : true;
            // 12/19/2005 Paul.  MySQL 5 returns SByte for a TinyInt. 
            if (obj.GetType() == Type.GetType("System.SByte"))
                return (Convert.ToSByte(obj) == 0) ? false : true;
            // 12/17/2005 Paul.  Oracle returns booleans as Int16. 
            if (obj.GetType() == Type.GetType("System.Int16"))
                return (Convert.ToInt16(obj) == 0) ? false : true;
            // 03/06/2006 Paul.  Oracle returns SYNC_CONTACT as decimal.
            if (obj.GetType() == Type.GetType("System.Decimal"))
                return (Convert.ToDecimal(obj) == 0) ? false : true;
            if (obj.GetType() == Type.GetType("System.String"))
            {
                string s = obj.ToString().ToLower();
                return (s == "true" || s == "on" || s == "1") ? true : false;
            }
            if (obj.GetType() != Type.GetType("System.Boolean"))
                return false;
            return bool.Parse(obj.ToString());
        }

        public static object ToDBBoolean(Boolean b)
        {
            // 03/22/2006 Paul.  DB2 requires that we convert the boolean to an integer.  It makes sense to do so for all platforms. 
            return b ? 1 : 0;
        }

        public static object ToDBBoolean(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return DBNull.Value;
            if (obj.GetType() != Type.GetType("System.Boolean"))
            {
                // 10/01/2006 Paul.  Return 0 instead of false, as false can be converted to text. 
                string s = obj.ToString().ToLower();
                return (s == "true" || s == "on" || s == "1") ? 1 : 0;
            }
            // 03/22/2006 Paul.  DB2 requires that we convert the boolean to an integer.  It makes sense to do so for all platforms. 
            return Convert.ToBoolean(obj) ? 1 : 0;
        }

        public static bool IsSQLServer(IDbCommand cmd)
        {
            return (cmd != null) && (cmd.GetType().FullName == "System.Data.SqlClient.SqlCommand");
        }

        public static bool IsSQLServer(IDbConnection con)
        {
            return (con != null) && (con.GetType().FullName == "System.Data.SqlClient.SqlConnection");
        }

        public static bool IsOracleDataAccess(IDbCommand cmd)
        {
            // 08/15/2005 Paul.  Type.GetType("Oracle.DataAccess.Client.OracleCommand") is returning NULL.  Use FullName instead. 
            return (cmd != null) && (cmd.GetType().FullName == "Oracle.DataAccess.Client.OracleCommand");
        }

        public static bool IsOracleDataAccess(IDbConnection con)
        {
            return (con != null) && (con.GetType().FullName == "Oracle.DataAccess.Client.OracleConnection");
        }

        public static bool IsOracleSystemData(IDbCommand cmd)
        {
            // 08/15/2005 Paul.  Type.GetType("Oracle.DataAccess.Client.OracleCommand") is returning NULL.  Use FullName instead. 
            return (cmd != null) && (cmd.GetType().FullName == "System.Data.OracleClient.OracleCommand");
        }

        public static bool IsOracleSystemData(IDbConnection con)
        {
            return (con != null) && (con.GetType().FullName == "System.Data.OracleClient.OracleConnection");
        }

        public static bool IsOracle(IDbCommand cmd)
        {
            return IsOracleDataAccess(cmd) || IsOracleSystemData(cmd);
        }

        public static bool IsOracle(IDbConnection con)
        {
            return IsOracleDataAccess(con) || IsOracleSystemData(con);
        }

        // 08/29/2008 Paul.  Allow testing of PostgreSQL. 
        public static bool IsPostgreSQL(IDbCommand cmd)
        {
            return (cmd != null) && (cmd.GetType().FullName == "Npgsql.NpgsqlCommand");
        }

        public static bool IsPostgreSQL(IDbConnection con)
        {
            return (con != null) && (con.GetType().FullName == "Npgsql.NpgsqlConnection");
        }

        public static bool IsMySQL(IDbCommand cmd)
        {
            return (cmd != null) && (cmd.GetType().FullName == "MySql.Data.MySqlClient.MySqlCommand");
        }

        public static bool IsMySQL(IDbConnection con)
        {
            return (con != null) && (con.GetType().FullName == "MySql.Data.MySqlClient.MySqlConnection");
        }

        public static bool IsDB2(IDbCommand cmd)
        {
            return (cmd != null) && (cmd.GetType().FullName == "IBM.Data.DB2.DB2Command");
        }

        public static bool IsDB2(IDbConnection con)
        {
            return (con != null) && (con.GetType().FullName == "IBM.Data.DB2.DB2Connection");
        }

        public static bool IsSqlAnywhere(IDbCommand cmd)
        {
            return (cmd != null) && (cmd.GetType().FullName == "iAnywhere.Data.AsaClient.AsaCommand");
        }

        public static bool IsSqlAnywhere(IDbConnection con)
        {
            return (con != null) && (con.GetType().FullName == "iAnywhere.Data.AsaClient.AsaConnection");
        }

        public static bool IsSybase(IDbCommand cmd)
        {
            return (cmd != null) && (cmd.GetType().FullName == "Sybase.Data.AseClient.AseCommand");
        }

        public static bool IsSybase(IDbConnection con)
        {
            return (con != null) && (con.GetType().FullName == "Sybase.Data.AseClient.AseConnection");
        }

        // 09/06/2008 Paul.  PostgreSQL does not require that we stream the bytes, so lets explore doing this for all platforms. 
        public static bool StreamBlobs(IDbConnection con)
        {
            if (IsPostgreSQL(con)) return false;
            else if (IsSQLServer(con)) return false;
            else if (IsOracleDataAccess(con)) return true;
            else if (IsOracleSystemData(con)) return true;
            else if (IsDB2(con)) return true;
            else if (IsMySQL(con)) return true;
            else if (IsSqlAnywhere(con)) return true;
            else if (IsSybase(con)) return true;
            return true;
        }

        public static string ExpandParameters(IDbCommand cmd)
        {
            try
            {
                if (cmd.CommandType == CommandType.Text)
                {
                    string sSql = cmd.CommandText;
                    CultureInfo ciEnglish = CultureInfo.CreateSpecificCulture("en-US");
                    foreach (IDbDataParameter par in cmd.Parameters)
                    {
                        if (par.Value == null || par.Value == DBNull.Value)
                        {
                            sSql = sSql.Replace(par.ParameterName, "null");
                        }
                        else
                        {
                            switch (par.DbType)
                            {
                                case DbType.Boolean:
                                    // 04/26/2008 Paul.  DbType.Boolean is used by SQL Server. 
                                    sSql = sSql.Replace(par.ParameterName, par.Value.ToString());
                                    break;
                                case DbType.Int16:
                                    // 03/22/2006 Paul.  DbType.Boolean gets saved as DbType.Int16 (when using DB2). 
                                    sSql = sSql.Replace(par.ParameterName, par.Value.ToString());
                                    break;
                                case DbType.Int32:
                                    sSql = sSql.Replace(par.ParameterName, par.Value.ToString());
                                    break;
                                case DbType.Int64:
                                    sSql = sSql.Replace(par.ParameterName, par.Value.ToString());
                                    break;
                                case DbType.Decimal:
                                    sSql = sSql.Replace(par.ParameterName, par.Value.ToString());
                                    break;

                                default:
                                    if (Sql.IsEmptyString(par.Value))
                                        sSql = sSql.Replace(par.ParameterName, "null");
                                    else
                                        sSql = sSql.Replace(par.ParameterName, "\'" + par.Value.ToString() + "\'");
                                    break;
                            }
                        }
                    }
                    return sSql;
                }
                else if (cmd.CommandType == CommandType.StoredProcedure)
                {
                    StringBuilder sbSql = new StringBuilder();
                    sbSql.Append(cmd.CommandText);
                    int nParameterIndex = 0;
                    if (IsOracle(cmd) || Sql.IsDB2(cmd) || IsPostgreSQL(cmd))
                        sbSql.Append("(");
                    else
                        sbSql.Append(" ");

                    CultureInfo ciEnglish = CultureInfo.CreateSpecificCulture("en-US");
                    foreach (IDbDataParameter par in cmd.Parameters)
                    {
                        if (nParameterIndex > 0)
                            sbSql.Append(", ");
                        if (par.Value == null || par.Value == DBNull.Value)
                        {
                            sbSql.Append("null");
                        }
                        else
                        {
                            switch (par.DbType)
                            {
                                case DbType.Int16:
                                    // 03/22/2006 Paul.  DbType.Boolean gets saved as DbType.Int16 (when using DB2). 
                                    sbSql.Append(par.Value.ToString());
                                    break;
                                case DbType.Int32:
                                    sbSql.Append(par.Value.ToString());
                                    break;
                                case DbType.Int64:
                                    sbSql.Append(par.Value.ToString());
                                    break;
                                case DbType.Decimal:
                                    sbSql.Append(par.Value.ToString());
                                    break;
                                case DbType.DateTime:
                                    // 01/21/2006 Paul.  Brazilian culture is having a problem with date formats.  Try using the european format yyyy/MM/dd HH:mm:ss. 
                                    // 06/13/2006 Paul.  Italian has a problem with the time separator.  Use the value from the culture from CalendarControl.SqlDateTimeFormat. 
                                    // 06/14/2006 Paul.  The Italian problem was that it was using the culture separator, but DataView only supports the en-US format. 
                                    //sbSql.Append("\'" + Convert.ToDateTime(par.Value).ToString(CalendarControl.SqlDateTimeFormat, ciEnglish.DateTimeFormat) + "\'");
                                    break;
                                default:
                                    if (Sql.IsEmptyString(par.Value))
                                        sbSql.Append("null");
                                    else
                                        sbSql.Append("\'" + par.Value.ToString() + "\'");
                                    break;
                            }
                        }
                        nParameterIndex++;
                    }
                    if (IsOracle(cmd) || Sql.IsDB2(cmd) || IsPostgreSQL(cmd))
                        sbSql.Append(");");
                    return sbSql.ToString();
                }
            }
            catch
            {
            }
            return cmd.CommandText;
        }

        public static string ClientScriptBlock(IDbCommand cmd)
        {	// 11/05/2009 Matthew. Need to disable this for debugging sometimes
            return "";
            //return "<script type=\"text/javascript\">sDebugSQL += '" + Sql.EscapeJavaScript(Sql.ExpandParameters(cmd)) + "';</script>";
        }

        // 07/18/2006 Paul.  SqlFilterMode.Contains behavior has be deprecated. It is now the same as SqlFilterMode.StartsWith. 
        public enum SqlFilterMode
        {
            Exact
          ,
            StartsWith
                , Contains
        }

        public static IDbDataParameter FindParameter(IDbCommand cmd, string sName)
        {
            IDbDataParameter par = null;
            // 12/17/2005 Paul.  Convert the name to Oracle or MySQL parameter format. 
            if (!sName.StartsWith("@"))
                sName = "@" + sName;
            sName = CreateDbName(cmd, sName.ToUpper());
            if (cmd.Parameters.Contains(sName))
            {
                par = cmd.Parameters[sName] as IDbDataParameter;
            }
            return par;
        }

        public static void SetParameter(IDbDataParameter par, string sValue)
        {
            if (par != null)
            {
                switch (par.DbType)
                {
                    case DbType.Guid: par.Value = Sql.ToGuid(sValue); break;
                    case DbType.Int16: par.Value = Sql.ToDBInteger(sValue); break;
                    case DbType.Int32: par.Value = Sql.ToDBInteger(sValue); break;
                    case DbType.Int64: par.Value = Sql.ToDBInteger(sValue); break;
                    case DbType.Double: par.Value = Sql.ToDBFloat(sValue); break;
                    case DbType.Decimal: par.Value = Sql.ToDBDecimal(sValue); break;
                    case DbType.Byte: par.Value = Sql.ToDBBoolean(sValue); break;
                    // 10/01/2006 Paul.  DB2 wants to use the Boolean data type. 
                    case DbType.Boolean: par.Value = Sql.ToDBBoolean(sValue); break;
                    case DbType.DateTime: par.Value = Sql.ToDBDateTime(sValue); break;
                    //case DbType.Binary  : ;  par.Size = nLength;  break;
                    default:
                        // 01/09/2006 Paul.  use ToDBString. 
                        par.Value = Sql.ToDBString(sValue);
                        par.Size = sValue.Length;
                        break;
                }
            }
        }

        public static void SetParameter(IDbCommand cmd, string sName, string sValue)
        {
            IDbDataParameter par = FindParameter(cmd, sName);
            if (par != null)
            {
                // 10/30/2008 Paul.  PostgreSQL has issues treating integers as booleans and booleans as integers. 
                // When importing records, we need to fix the parameters so that we send to PostgreSQL an integer data type. 
                if (IsPostgreSQL(cmd) && par.DbType == DbType.Boolean)
                    par.DbType = DbType.Int32;
                SetParameter(par, sValue);
            }
        }

        // 04/04/2006 Paul.  SOAP needs a way to set a DateTime that has already been converted to server time. 
        public static void SetParameter(IDbCommand cmd, string sName, DateTime dtValueInServerTime)
        {
            IDbDataParameter par = FindParameter(cmd, sName);
            if (par != null)
            {
                par.Value = Sql.ToDBDateTime(dtValueInServerTime);
            }
        }

        // 09/19/2006 Paul.  Import needs an easier way to set a Guid parameter. 
        public static void SetParameter(IDbCommand cmd, string sName, Guid gValue)
        {
            IDbDataParameter par = FindParameter(cmd, sName);
            if (par != null)
            {
                if (Sql.IsEmptyGuid(gValue))
                {
                    par.Value = DBNull.Value;
                }
                else
                {
                    if (IsSQLServer(cmd) || Sql.IsSqlAnywhere(cmd))
                        par.Value = Sql.ToDBGuid(gValue);
                    else
                        par.Value = gValue.ToString().ToUpper();
                }
            }
        }



        // 09/28/2006 Paul.  Grant public access to CreateDbName so that we can access from the import function. 
        public static string CreateDbName(IDbCommand cmd, string sField)
        {
            // 09/06/2008 Paul.  @ seems to work for PostgreSQL, but the manual mentions the colon. 
            if (IsOracle(cmd) || IsPostgreSQL(cmd))
            {
                sField = sField.Replace("@", ":");
            }
            else if (IsMySQL(cmd))
            {
                // 12/20/2005 Paul.  The MySQL provider makes the parameter names upper case.  
                sField = sField.Replace("@", "?IN_").ToUpper();
            }
            else if (IsSqlAnywhere(cmd))
            {
                // 04/21/2006 Paul.  SQL Anywhere does not support named parameters. 
                // http://www.ianywhere.com/developer/product_manuals/sqlanywhere/0902/en/html/dbpgen9/00000527.htm
                // The Adaptive Server Anywhere .NET data provider uses positional parameters that are marked with a question mark (?) instead of named parameters.
                sField = "?";
            }
            return sField;
        }

        public static string ExtractDbName(IDbCommand cmd, string sParameterName)
        {
            string sField = sParameterName;
            // 09/06/2008 Paul.  @ seems to work for PostgreSQL, but the manual mentions the colon. 
            if (IsOracle(cmd) || IsPostgreSQL(cmd))
            {
                if (sField.StartsWith(":"))
                    sField = sField.Substring(1);
            }
            else if (IsOracleSystemData(cmd))
            {
                if (cmd.CommandType == CommandType.Text)
                {
                    if (sField.StartsWith(":"))
                        sField = sField.Substring(1);
                }
                else
                {
                    if (sField.StartsWith("IN_"))
                        sField = sField.Substring(3);
                }
            }
            else if (IsMySQL(cmd))
            {
                if (sField.StartsWith("?IN_"))
                    sField = sField.Substring(4);
            }
            else
            {
                if (sField.StartsWith("@"))
                    sField = sField.Substring(1);
            }
            return sField;
        }

        // 12/17/2008 Paul.  We need to be able to create a parameter while importing. 
        public static IDbDataParameter CreateParameter(IDbCommand cmd, string sField)
        {
            IDbDataParameter par = cmd.CreateParameter();
            if (par == null)
            {
                // 10/14/2005 Paul. MySql is not returning a value from CreateParameter.  It will have to be created from the factory. 
                //DbProviderFactory dbf = DbProviderFactories.GetFactory();
                //par = dbf.CreateParameter();
            }
            // 08/13/2005 Paul.  Oracle uses a different ParameterToken. 
            // 09/06/2008 Paul.  @ seems to work for PostgreSQL, but the manual mentions the colon. 
            if (IsOracleDataAccess(cmd) || IsPostgreSQL(cmd))
            {
                sField = sField.Replace("@", ":");
                if (cmd.CommandType == CommandType.Text)
                    cmd.CommandText = cmd.CommandText.Replace("@", ":");
            }
            else if (IsOracleSystemData(cmd))
            {
                if (cmd.CommandType == CommandType.Text)
                {
                    // 08/03/2006 Paul.  System.Data.OracleClient requires the colon for Text parameters. 
                    sField = sField.Replace("@", ":");
                    cmd.CommandText = cmd.CommandText.Replace("@", ":");
                }
                else
                {
                    // 08/03/2006 Paul.  System.Data.OracleClient does not like the colon in the parameter name, but the name must match precicely. 
                    // All SplendidCRM parameter names for Oracle start with IN_. 
                    sField = sField.Replace("@", "IN_");
                }
            }
            // 10/18/2005 Paul.  MySQL uses a different ParameterToken. 
            else if (IsMySQL(cmd))
            {
                // 12/20/2005 Paul.  The MySQL provider makes the parameter names upper case.  
                sField = sField.Replace("@", "?IN_").ToUpper();
                if (cmd.CommandType == CommandType.Text)
                    cmd.CommandText = cmd.CommandText.Replace("@", "?IN_");
            }
            else if (IsSqlAnywhere(cmd))
            {
                // 04/21/2006 Paul.  SQL Anywhere does not support named parameters. Replace with ?.
                // http://www.ianywhere.com/developer/product_manuals/sqlanywhere/0902/en/html/dbpgen9/00000527.htm
                // The Adaptive Server Anywhere .NET data provider uses positional parameters that are marked with a question mark (?) instead of named parameters.
                cmd.CommandText = cmd.CommandText.Replace(sField.ToUpper(), "?");
            }
            // 12/17/2005 Paul.  Make the parameter name uppercase so that it can be easily found in the SetParameter function. 
            par.ParameterName = sField.ToUpper();
            cmd.Parameters.Add(par);
            return par;
        }

        public static IDbDataParameter CreateParameter(IDbCommand cmd, string sField, string sCsType, int nLength)
        {
            IDbDataParameter par = Sql.CreateParameter(cmd, sField);
            switch (sCsType)
            {
                case "Guid":
                    if (Sql.IsSQLServer(cmd) || Sql.IsSqlAnywhere(cmd))
                    {
                        par.DbType = DbType.Guid;
                    }
                    else
                    {
                        // 08/11/2005 Paul.  Oracle does not support Guids, nor does MySQL. 
                        par.DbType = DbType.String;
                        par.Size = 36;  // 08/13/2005 Paul.  Only set size for variable length fields. 
                    }
                    break;
                case "short": par.DbType = DbType.Int16; break;
                case "Int32": par.DbType = DbType.Int32; break;
                case "Int64": par.DbType = DbType.Int64; break;
                case "float": par.DbType = DbType.Double; break;
                case "decimal": par.DbType = DbType.Decimal; break;
                case "bool":
                    // 10/01/2006 Paul.  DB2 seems to prefer Boolean.  Oracle wants Byte.  
                    // We are going to use Boolean for all but Oracle as this what we have tested extensively in the AddParameter(,,bool) function below. 
                    if (Sql.IsOracle(cmd))
                        par.DbType = DbType.Byte;
                    else
                        par.DbType = DbType.Boolean;
                    break;
                case "DateTime": par.DbType = DbType.DateTime; break;
                case "byte[]": par.DbType = DbType.Binary; par.Size = nLength; break;
                // 01/24/2006 Paul.  A severe error occurred on the current command. The results, if any, should be discarded. 
                // MS03-031 security patch causes this error because of stricter datatype processing.  
                // http://www.microsoft.com/technet/security/bulletin/MS03-031.mspx.
                // http://support.microsoft.com/kb/827366/
                case "ansistring": par.DbType = DbType.AnsiString; par.Size = nLength; break;
                //case "string"  :  par.DbType        = DbType.String    ;  par.Size = nLength;  break;
                default: par.DbType = DbType.String; par.Size = nLength; break;
            }
            return par;
        }

        public static IDbDataParameter AddParameter(IDbCommand cmd, string sField, short nValue)
        {
            IDbDataParameter par = CreateParameter(cmd, sField);
            par.DbType = DbType.Int16;
            //par.Size          = 4;
            par.Value = Sql.ToDBInteger(nValue);
            return par;
        }

        public static IDbDataParameter AddParameter(IDbCommand cmd, string sField, int nValue)
        {
            IDbDataParameter par = CreateParameter(cmd, sField);
            par.DbType = DbType.Int32;
            //par.Size          = 4;
            par.Value = Sql.ToDBInteger(nValue);
            return par;
        }

        public static IDbDataParameter AddParameter(IDbCommand cmd, string sField, long nValue)
        {
            IDbDataParameter par = CreateParameter(cmd, sField);
            par.DbType = DbType.Int64;
            //par.Size          = 4;
            par.Value = Sql.ToDBInteger(nValue);
            return par;
        }

        public static IDbDataParameter AddParameter(IDbCommand cmd, string sField, float fValue)
        {
            IDbDataParameter par = CreateParameter(cmd, sField);
            par.DbType = DbType.Double;
            //par.Size          = 8;
            par.Value = Sql.ToDBFloat(fValue);
            return par;
        }

        public static IDbDataParameter AddParameter(IDbCommand cmd, string sField, Decimal dValue)
        {
            IDbDataParameter par = CreateParameter(cmd, sField);
            par.DbType = DbType.Decimal;
            //par.Size          = 8;
            par.Value = Sql.ToDBDecimal(dValue);
            return par;
        }

        public static IDbDataParameter AddParameter(IDbCommand cmd, string sField, bool bValue)
        {
            IDbDataParameter par = CreateParameter(cmd, sField);
            // 03/22/2006 Paul.  Not sure why DbType.Byte was used when DbType.Boolean is available. 
            // 03/22/2006 Paul.  DB2 requires that we convert the boolean to an integer.  It makes sense to do so for all platforms. 
            // 03/31/2006 Paul.  Oracle does not like DbType.Boolean.  That must be why we used DbType.Byte.
            // 08/29/2008 Paul.  PostgreSQL has issues treating integers as booleans and booleans as integers. 
            if (IsOracle(cmd))
            {
                par.DbType = DbType.Byte;
                par.Value = Sql.ToDBBoolean(bValue);
            }
            else if (IsPostgreSQL(cmd))
            {
                par.DbType = DbType.Int32;
                //par.Size          = 4;
                par.Value = bValue ? 1 : 0;
            }
            else
            {
                par.DbType = DbType.Boolean;
                par.Value = Sql.ToDBBoolean(bValue);
            }
            return par;
        }

        public static IDbDataParameter AddParameter(IDbCommand cmd, string sField, Guid gValue)
        {
            IDbDataParameter par = CreateParameter(cmd, sField);
            // 10/18/2005 Paul.  SQL Server is the only one that accepts a native Guid data type. 
            if (IsSQLServer(cmd) || Sql.IsSqlAnywhere(cmd))
            {
                par.DbType = DbType.Guid;
                //par.Size          = 16;
                par.Value = Sql.ToDBGuid(gValue);
            }
            else
            {
                // 08/11/2005 Paul.  Oracle does not support Guids, nor does MySQL. 
                // 04/09/2006 Paul.  AnsiStringFixedLength is the most appropriate mapping.  
                // 04/09/2006 Paul.  Sybase is having a problem, but this does not help. 
                par.DbType = DbType.AnsiStringFixedLength;
                par.Size = 36;  // 08/13/2005 Paul.  Only set size for variable length fields. 
                if (Sql.IsEmptyGuid(gValue))
                    par.Value = DBNull.Value;
                else
                    par.Value = gValue.ToString().ToUpper();  // 08/15/2005 Paul.  Guids are stored in Oracle in upper case. 
            }
            return par;
        }

        public static IDbDataParameter AddParameter(IDbCommand cmd, string sField, DateTime dtValue)
        {
            IDbDataParameter par = CreateParameter(cmd, sField);
            par.DbType = DbType.DateTime;
            //par.Size          = 8;
            par.Value = Sql.ToDBDateTime(dtValue);
            return par;
        }

        public static IDbDataParameter AddParameter(IDbCommand cmd, string sField, string sValue)
        {
            IDbDataParameter par = CreateParameter(cmd, sField);
            par.DbType = DbType.String;
            // 08/13/2005 Paul.  Only set size for variable length fields. 
            // 07/17/2008 Paul.  sValue can be NULL. 
            par.Size = (sValue != null) ? sValue.Length : 0;
            par.Value = Sql.ToDBString(sValue);
            return par;
        }

        public static IDbDataParameter AddParameter(IDbCommand cmd, string sField, string sValue, bool bAllowEmptyString)
        {
            IDbDataParameter par = CreateParameter(cmd, sField);
            par.DbType = DbType.String;
            // 08/13/2005 Paul.  Only set size for variable length fields. 
            // 07/17/2008 Paul.  sValue can be NULL. 
            par.Size = (sValue != null) ? sValue.Length : 0;
            // 09/20/2005 Paul.  the SQL IN clause does not allow NULL. 
            par.Value = bAllowEmptyString ? sValue : Sql.ToDBString(sValue);
            return par;
        }

        public static IDbDataParameter AddAnsiParam(IDbCommand cmd, string sField, string sValue, int nSize)
        {
            // 08/13/2005 Paul.  Truncate the string if it exceeds the specified size. 
            // The field should have been validated on the client side, so this is just defensive programming. 
            // 10/09/2005 Paul. sValue can be null. 
            if (sValue != null)
            {
                // 04/29/2008 Paul.  Some custom fields have not been updating because the MAX_SIZE is 0. Use the actual string length. 
                if (nSize == 0)
                    nSize = sValue.Length;
                else if (sValue.Length > nSize)
                    sValue = sValue.Substring(0, nSize);
            }
            // 01/24/2006 Paul.  A severe error occurred on the current command. The results, if any, should be discarded. 
            // MS03-031 security patch causes this error because of stricter datatype processing.  
            // http://www.microsoft.com/technet/security/bulletin/MS03-031.mspx.
            // http://support.microsoft.com/kb/827366/
            IDbDataParameter par = CreateParameter(cmd, sField);
            par.DbType = DbType.AnsiString;
            par.Size = nSize;  // 08/13/2005 Paul.  Only set size for variable length fields. 
            par.Value = Sql.ToDBString(sValue);
            return par;
        }

        public static IDbDataParameter AddParameter(IDbCommand cmd, string sField, string sValue, int nSize)
        {
            // 08/13/2005 Paul.  Truncate the string if it exceeds the specified size. 
            // The field should have been validated on the client side, so this is just defensive programming. 
            // 10/09/2005 Paul. sValue can be null. 
            if (sValue != null)
            {
                // 04/29/2008 Paul.  Some custom fields have not been updating because the MAX_SIZE is 0. Use the actual string length. 
                if (nSize == 0)
                    nSize = sValue.Length;
                else if (sValue.Length > nSize)
                    sValue = sValue.Substring(0, nSize);
            }
            IDbDataParameter par = CreateParameter(cmd, sField);
            par.DbType = DbType.String;
            par.Size = nSize;  // 08/13/2005 Paul.  Only set size for variable length fields. 
            par.Value = Sql.ToDBString(sValue);
            return par;
        }

        public static IDbDataParameter AddParameter(IDbCommand cmd, string sField, byte[] byValue)
        {
            IDbDataParameter par = CreateParameter(cmd, sField);
            par.DbType = DbType.Binary;
            // 07/06/2008 Paul.  byValue might be NULL. 
            par.Size = (byValue != null) ? byValue.Length : 0;  // 08/13/2005 Paul.  Only set size for variable length fields. 
            par.Value = Sql.ToDBBinary(byValue);
            return par;
        }

        public static void AppendParameter(IDbCommand cmd, int nValue, string sField, bool bIsEmpty)
        {
            if (!bIsEmpty)
            {
                cmd.CommandText += "   and " + sField + " = @" + sField + ControlChars.CrLf;
                //cmd.Parameters.Add("@" + sField, SqlDbType.Int, 4).Value = nValue;
                Sql.AddParameter(cmd, "@" + sField, nValue);
            }
        }

        // 09/01/2006 Paul.  Add Float parameter. 
        public static void AppendParameter(IDbCommand cmd, float fValue, string sField, bool bIsEmpty)
        {
            if (!bIsEmpty)
            {
                cmd.CommandText += "   and " + sField + " = @" + sField + ControlChars.CrLf;
                //cmd.Parameters.Add("@" + sField, SqlDbType.Int, 4).Value = nValue;
                Sql.AddParameter(cmd, "@" + sField, fValue);
            }
        }

        public static void AppendParameter(IDbCommand cmd, Decimal dValue, string sField, bool bIsEmpty)
        {
            if (!bIsEmpty)
            {
                cmd.CommandText += "   and " + sField + " = @" + sField + ControlChars.CrLf;
                //cmd.Parameters.Add("@" + sField, DbType.Decimal, 8).Value = dValue;
                Sql.AddParameter(cmd, "@" + sField, dValue);
            }
        }

        // 04/27/2008 Paul.  The boolean AppendParameter now requires the IsEmpty flag. 
        // SearchView was the only place where the value was also used to determine if empty. 
        public static void AppendParameter(IDbCommand cmd, bool bValue, string sField, bool bIsEmpty)
        {
            if (!bIsEmpty)
            {
                cmd.CommandText += "   and " + sField + " = @" + sField + ControlChars.CrLf;
                //cmd.Parameters.Add("@" + sField, DbType.Byte, 1).Value = bValue;
                Sql.AddParameter(cmd, "@" + sField, bValue);
            }
        }

        // 12/26/2006 Paul.  We need to determine the next available placeholder name. 
        public static string NextPlaceholder(IDbCommand cmd, string sField)
        {
            int nPlaceholderIndex = 0;
            string sFieldPlaceholder = sField;
            IDataParameter par = FindParameter(cmd, sFieldPlaceholder);
            while (par != null)
            {
                // If the field placeholder exists, increment the index and search again. 
                nPlaceholderIndex++;
                sFieldPlaceholder = sField + nPlaceholderIndex.ToString();
                par = FindParameter(cmd, sFieldPlaceholder);
            }
            return sFieldPlaceholder;
        }

        public static void AppendParameter(IDbCommand cmd, Guid gValue, string sField)
        {
            // 12/26/2006 Paul.  We are having a problem with the ASSIGNED_USER_ID that is set 
            // during an ACL filter and the same field being set in a search criteria. 
            // To solve the problem, create an alternate placeholder name. 
            string sFieldPlaceholder = NextPlaceholder(cmd, sField);
            // 02/05/2006 Paul.  DB2 is the same as Oracle in that searches are case-significant. 

            // 09/18/2008 Paul.  DB2 has an issue with using a placeholder in a function. 
            // http://bytes.com/forum/thread182742.html
            // If you read the rules for function resolution in the SQL Reference, you'll see that they are very sensitive to the data types of the parameters. 
            // Unfortunately, a parameter marker doesn't have a type when it is precompiled, so DB2 doesn't know what type to use and you get the error.
            // ERROR [42610] [IBM][DB2/NT] SQL0418N A statement contains a use of a parameter marker that is not valid. SQLSTATE=42610
            // 09/18/2008 Paul.  Since .NET supplies the GUID in upper case, we can remove the upper() around the placeholder. 
            // In our new platforms, the GUID is stored in uppercase, so we don't need the upper() on either side. 
            if (IsOracle(cmd) || Sql.IsDB2(cmd) || IsPostgreSQL(cmd))
                cmd.CommandText += "   and " + sField + " = @" + sFieldPlaceholder + ControlChars.CrLf;
            else
                cmd.CommandText += "   and " + sField + " = @" + sFieldPlaceholder + ControlChars.CrLf;
            //cmd.Parameters.Add("@" + sField, DbType.Guid, 1).Value = gValue;
            Sql.AddParameter(cmd, "@" + sFieldPlaceholder, gValue);
        }

        public static void AppendParameter(IDbCommand cmd, Guid gValue, string sField, bool bIsEmpty)
        {
            if (!bIsEmpty)
            {
                AppendParameter(cmd, gValue, sField);
            }
        }

        public static void AppendParameter(IDbCommand cmd, DateTime dtValue, string sField)
        {
            if (dtValue != DateTime.MinValue)
            {
                cmd.CommandText += "   and " + sField + " = @" + sField + ControlChars.CrLf;
                //cmd.Parameters.Add("@" + sField, DbType.DateTime, 8).Value = dtValue;
                Sql.AddParameter(cmd, "@" + sField, dtValue);
            }
        }

        // 07/25/2006 Paul.  Support the Between clause for dates. 
        public static void AppendParameter(IDbCommand cmd, DateTime dtValue1, DateTime dtValue2, string sField)
        {
            // 07/25/2006 Paul.  The between clause is greater than or equal to Value1 and less than or equal to Value2.
            // We want the query to be less than Value2.
            //cmd.CommandText += "   and " + sField + " between @" + sField + "_1 and @" + sField + "_2" + ControlChars.CrLf;
            // 12/17/2007 Paul.  Allow either date value to be NULL so that we can do greater than or less than searches. 
            if (dtValue1 != DateTime.MinValue)
            {
                cmd.CommandText += "   and " + sField + " >= @" + sField + "_1" + ControlChars.CrLf;
                Sql.AddParameter(cmd, "@" + sField + "_1", dtValue1);
            }
            if (dtValue2 != DateTime.MinValue)
            {
                cmd.CommandText += "   and " + sField + " <  @" + sField + "_2" + ControlChars.CrLf;
                Sql.AddParameter(cmd, "@" + sField + "_2", dtValue2);
            }
        }

        public static void AppendParameter(IDbCommand cmd, string sValue, string sField)
        {
            if (!IsEmptyString(sValue))
            {
                cmd.CommandText += "   and " + sField + " = @" + sField + ControlChars.CrLf;
                Sql.AddParameter(cmd, "@" + sField, sValue, sValue.Length);
            }
        }

        public static void AppendParameter(IDbCommand cmd, string sValue, SqlFilterMode mode, string sField)
        {
            if (!IsEmptyString(sValue))
            {
                if (IsOracle(cmd) || Sql.IsDB2(cmd) || IsPostgreSQL(cmd))
                {
                    // 09/18/2008 Paul.  DB2 has an issue with using a placeholder in a function. 
                    // http://bytes.com/forum/thread182742.html
                    // If you read the rules for function resolution in the SQL Reference, you'll see that they are very sensitive to the data types of the parameters. 
                    // Unfortunately, a parameter marker doesn't have a type when it is precompiled, so DB2 doesn't know what type to use and you get the error.
                    // ERROR [42610] [IBM][DB2/NT] SQL0418N A statement contains a use of a parameter marker that is not valid. SQLSTATE=42610
                    // 09/18/2008 Paul.  Since this is just for searching, we can insert the value in uppercase instead of using the upper() function on the right. 
                    switch (mode)
                    {
                        case SqlFilterMode.Exact:
                            cmd.CommandText += "   and upper(" + sField + ") = @" + sField + ControlChars.CrLf;
                            Sql.AddParameter(cmd, "@" + sField, sValue.ToUpper(), sValue.Length);
                            break;
                        case SqlFilterMode.StartsWith:
                            // 08/29/2005 Paul.  Oracle uses || to concatenate strings. 
                            cmd.CommandText += "   and upper(" + sField + ") like @" + sField + " || '%'" + ControlChars.CrLf;
                            sValue = EscapeSQLLike(sValue);
                            // 07/16/2006 Paul.  MySQL requires that slashes be escaped, even in the escape clause. 
                            // 09/02/2008 Paul.  PostgreSQL requires two slashes. 
                            if (IsMySQL(cmd) || IsPostgreSQL(cmd))
                            {
                                sValue = sValue.Replace("\\", "\\\\");
                                cmd.CommandText += " escape '\\\\'";
                            }
                            else
                                cmd.CommandText += " escape '\\'";
                            Sql.AddParameter(cmd, "@" + sField, sValue.ToUpper(), sValue.Length);
                            break;
                        case SqlFilterMode.Contains:
                            // 08/29/2005 Paul.  Oracle uses || to concatenate strings. 
                            cmd.CommandText += "   and upper(" + sField + ") like '%' || @" + sField + " || '%'" + ControlChars.CrLf;
                            sValue = EscapeSQLLike(sValue);
                            // 07/16/2006 Paul.  MySQL requires that slashes be escaped, even in the escape clause. 
                            // 09/02/2008 Paul.  PostgreSQL requires two slashes. 
                            if (IsMySQL(cmd) || IsPostgreSQL(cmd))
                            {
                                sValue = sValue.Replace("\\", "\\\\");
                                cmd.CommandText += " escape '\\\\'";
                            }
                            else
                                cmd.CommandText += " escape '\\'";
                            Sql.AddParameter(cmd, "@" + sField, sValue.ToUpper(), sValue.Length);
                            break;
                    }
                }
                else
                {
                    switch (mode)
                    {
                        case SqlFilterMode.Exact:
                            cmd.CommandText += "   and " + sField + " = @" + sField + ControlChars.CrLf;
                            Sql.AddParameter(cmd, "@" + sField, sValue, sValue.Length);
                            break;
                        case SqlFilterMode.StartsWith:
                            // 08/29/2005 Paul.  SQL Server uses + to concatenate strings. 
                            cmd.CommandText += "   and " + sField + " like @" + sField + " + '%'" + ControlChars.CrLf;
                            sValue = EscapeSQLLike(sValue);
                            // 07/16/2006 Paul.  MySQL requires that slashes be escaped, even in the escape clause. 
                            // 09/02/2008 Paul.  PostgreSQL requires two slashes. 
                            if (IsMySQL(cmd) || IsPostgreSQL(cmd))
                            {
                                sValue = sValue.Replace("\\", "\\\\");
                                cmd.CommandText += " escape '\\\\'";
                            }
                            else
                                cmd.CommandText += " escape '\\'";
                            Sql.AddParameter(cmd, "@" + sField, sValue, sValue.Length);
                            break;
                        case SqlFilterMode.Contains:
                            // 08/29/2005 Paul.  SQL Server uses + to concatenate strings. 
                            cmd.CommandText += "   and " + sField + " like '%' + @" + sField + " + '%'" + ControlChars.CrLf;
                            sValue = EscapeSQLLike(sValue);
                            // 07/16/2006 Paul.  MySQL requires that slashes be escaped, even in the escape clause. 
                            // 09/02/2008 Paul.  PostgreSQL requires two slashes. 
                            if (IsMySQL(cmd) || IsPostgreSQL(cmd))
                            {
                                sValue = sValue.Replace("\\", "\\\\");
                                cmd.CommandText += " escape '\\\\'";
                            }
                            else
                                cmd.CommandText += " escape '\\'";
                            Sql.AddParameter(cmd, "@" + sField, sValue, sValue.Length);
                            break;
                    }
                }
            }
        }





        public static void AppendParameter(IDbCommand cmd, string[] arr, string sField)
        {
            AppendParameter(cmd, arr, sField, false);
        }

        public static void AppendParameter(IDbCommand cmd, string[] arr, string sField, bool bOrClause)
        {
            if (arr != null)
            {
                int nCount = 0;
                StringBuilder sb = new StringBuilder();
                foreach (string item in arr)
                {
                    // 09/20/2005 Paul. Allow an empty string to be a valid selection.
                    //if ( item.Length > 0 )
                    {
                        if (nCount > 0)
                            sb.Append(", ");
                        // 12/20/2005 Paul.  Need to use the correct parameter token. 
                        // 05/27/2006 Paul.  The number of parameters may exceed 10.
                        // 10/16/2006 Paul.  Use a 3-char format string to prevent ExpandParamters from performing incomplete replacements. 
                        sb.Append(CreateDbName(cmd, "@" + sField + nCount.ToString("000")));
                        //cmd.Parameters.Add("@" + sField + nCount.ToString("000"), DbType.Guid, 16).Value = item.Value;
                        // 09/20/2005 Paul.  The SQL IN clause does not allow NULL in the set.  Use an empty string instead. 
                        Sql.AddParameter(cmd, "@" + sField + nCount.ToString("000"), item, true);
                        nCount++;
                    }
                }
                if (sb.Length > 0)
                {
                    // 02/20/2006 Paul.  We sometimes need ot use the OR clause. 
                    if (bOrClause)
                        cmd.CommandText += "    or ";
                    else
                        cmd.CommandText += "   and ";
                    cmd.CommandText += sField + " in (" + sb.ToString() + ")" + ControlChars.CrLf;
                }
            }
        }

        public static void AppendParameter(DataView vw, string[] arr, string sField, bool bOrClause)
        {
            if (arr != null)
            {
                int nCount = 0;
                StringBuilder sb = new StringBuilder();
                foreach (string item in arr)
                {
                    if (nCount > 0)
                        sb.Append(", ");
                    sb.Append("\'" + item.Replace("\'", "\'\'") + "\'");
                    nCount++;
                }
                if (sb.Length > 0)
                {
                    // 02/20/2006 Paul.  We cannot set the filter in parts; it must be set fully formed. 
                    if (bOrClause)
                        vw.RowFilter += "    or " + sField + " in (" + sb.ToString() + ")" + ControlChars.CrLf;
                    else
                        vw.RowFilter += "   and " + sField + " in (" + sb.ToString() + ")" + ControlChars.CrLf;
                }
            }
        }



        public static void AppendGuids(IDbCommand cmd, string[] arr, string sField)
        {
            if (arr != null)
            {
                int nCount = 0;
                StringBuilder sb = new StringBuilder();
                foreach (string item in arr)
                {
                    if (item.Length > 0)
                    {
                        if (nCount > 0)
                            sb.Append(", ");
                        // 12/20/2005 Paul.  Need to use the correct parameter token. 
                        // 10/16/2006 Paul.  Use a 3-char format string to prevent ExpandParamters from performing incomplete replacements. 
                        sb.Append(CreateDbName(cmd, "@" + sField + nCount.ToString("000")));
                        //cmd.Parameters.Add("@" + sField + nCount.ToString("000"), DbType.Guid, 16).Value = new Guid(item.Value);
                        Sql.AddParameter(cmd, "@" + sField + nCount.ToString("000"), new Guid(item));
                        nCount++;
                    }
                }
                if (sb.Length > 0)
                {
                    cmd.CommandText += "   and " + sField + " in (" + sb.ToString() + ")" + ControlChars.CrLf;
                }
            }
        }

        // 10/20/2009 Paul.  Try to be more efficient by using a reader. 
        public static void WriteStream(IDataReader rdr, int nFieldIndex, BinaryWriter writer)
        {
            // 10/20/2009 Paul.  Read in 64K chunks. 
            const int BUFFER_LENGTH = 64 * 1024;
            long idx = 0;
            long size = 0;
            byte[] binData = new byte[BUFFER_LENGTH];
            while ((size = rdr.GetBytes(nFieldIndex, idx, binData, 0, BUFFER_LENGTH)) > 0)
            {
                writer.Write(binData, 0, (int)size);
                idx += size;
            }
        }

        public static byte[] ToByteArray(IDbDataParameter parBYTES)
        {
            byte[] binData = null;
            int size = (parBYTES == null ? 0 : parBYTES.Size);
            binData = new byte[size];
            if (size > 0)
            {
                // 10/20/2005 Paul.  Convert System.Array to a byte array. 
                GCHandle handle = GCHandle.Alloc(parBYTES.Value, GCHandleType.Pinned);
                IntPtr ptr = handle.AddrOfPinnedObject();
                Marshal.Copy(ptr, binData, 0, size);
                handle.Free();
            }
            return binData;
        }

        public static byte[] ToByteArray(System.Array arrBYTES)
        {
            byte[] binData = null;
            int size = (arrBYTES == null ? 0 : arrBYTES.Length);
            binData = new byte[size];
            if (size > 0)
            {
                // 10/20/2005 Paul.  Convert System.Array to a byte array. 
                GCHandle handle = GCHandle.Alloc(arrBYTES, GCHandleType.Pinned);
                IntPtr ptr = handle.AddrOfPinnedObject();
                Marshal.Copy(ptr, binData, 0, size);
                handle.Free();
            }
            return binData;
        }

        public static byte[] ToByteArray(object oBlob)
        {
            byte[] binData = null;
            Type tBlob = oBlob.GetType();
            if (tBlob == typeof(System.Byte[]))
            {
                binData = oBlob as byte[];
            }
            else if (tBlob == typeof(System.Array))
            {
                binData = Sql.ToByteArray(oBlob as System.Array);
            }
            else
            {
                throw (new Exception("Unsupported blob type " + oBlob.GetType().ToString()));
            }
            return binData;
        }



        public static void LimitResults(IDbCommand cmd, int nMaxRows)
        {
            if (IsMySQL(cmd) || IsPostgreSQL(cmd))
                cmd.CommandText += " limit " + nMaxRows.ToString();
            else if (IsOracle(cmd))
                cmd.CommandText = "select * from (" + cmd.CommandText + ") where rownum <= " + nMaxRows.ToString();
            else if (IsDB2(cmd))
                cmd.CommandText += " fetch first " + nMaxRows.ToString() + " rows only";
            else if (IsSQLServer(cmd))
            {
                if (cmd.CommandText.ToLower().StartsWith("select"))
                    cmd.CommandText = "select top " + nMaxRows.ToString() + cmd.CommandText.Substring(6);
            }
        }

        // 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
        public static string MetadataName(IDbCommand cmd, string sNAME)
        {
            // 09/02/2008 Paul.  Tables and field names in DB2 must be in uppercase. 
            // 09/02/2008 Paul.  Tables and field names in Oracle must be in uppercase. 
            if (IsOracle(cmd) || IsDB2(cmd))
                return sNAME.ToUpper();
            // 09/02/2008 Paul.  Tables and field names in PostgreSQL must be in uppercase. 
            else if (IsPostgreSQL(cmd))
                return sNAME.ToLower();
            // 09/02/2008 Paul.  SQL Server and MySQL are not typically case significant, 
            // but SQL Server can be configured to be case significant.  Ignore that case for now. 
            return sNAME;
        }

        // 02/08/2008 Paul.  We need to build a list of the fields used by the dynamic grid. 
        public static string FormatSelectFields(UniqueStringCollection arrSelectFields)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string sField in arrSelectFields)
            {
                if (sb.Length > 0)
                    sb.Append("     , ");
                sb.AppendLine(sField);
            }
            return sb.ToString();
        }

        public static byte[] ReadImage(Guid gID, IDbConnection con, string sCommandText)
        {
            using (MemoryStream stm = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stm))
                {
                    using (IDbCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = sCommandText;
                        cmd.CommandType = CommandType.StoredProcedure;

                        const int BUFFER_LENGTH = 4 * 1024;
                        int idx = 0;
                        int size = 0;
                        byte[] binData = new byte[BUFFER_LENGTH];
                        IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                        IDbDataParameter parFILE_OFFSET = Sql.AddParameter(cmd, "@FILE_OFFSET", idx);
                        IDbDataParameter parREAD_SIZE = Sql.AddParameter(cmd, "@READ_SIZE", size);
                        IDbDataParameter parBYTES = Sql.AddParameter(cmd, "@BYTES", binData);
                        parBYTES.Direction = ParameterDirection.InputOutput;
                        do
                        {
                            parID.Value = gID;
                            parFILE_OFFSET.Value = idx;
                            parREAD_SIZE.Value = BUFFER_LENGTH;
                            size = 0;
                            if (Sql.IsOracle(cmd) || Sql.IsDB2(cmd)) // || Sql.IsMySQL(cmd) )
                            {
                                cmd.ExecuteNonQuery();
                                binData = Sql.ToByteArray(parBYTES);
                                if (binData != null)
                                {
                                    size = binData.Length;
                                    writer.Write(binData);
                                    idx += size;
                                }
                            }
                            else
                            {
                                using (IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                                {
                                    if (rdr.Read())
                                    {
                                        binData = Sql.ToByteArray((System.Array)rdr[0]);
                                        if (binData != null)
                                        {
                                            size = binData.Length;
                                            writer.Write(binData);
                                            idx += size;
                                        }
                                    }
                                }
                            }
                        }
                        while (size == BUFFER_LENGTH);
                    }
                }
                return stm.ToArray();
            }
        }
        // 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
        // This is because SQL Server 2005 and 2008 are the only platforms that support a global transaction ID with sp_getbindtoken. 
        public static IDbTransaction BeginTransaction(IDbConnection con)
        {
            IDbTransaction trn = con.BeginTransaction();
            Guid gSPLENDID_TRANSACTION_ID = Guid.NewGuid();
            //SqlProcs.spSYSTEM_TRANSACTIONS_Create(ref gSPLENDID_TRANSACTION_ID, trn);
            return trn;
        }
    }



    public class UniqueStringCollection : StringCollection
    {
        new public int Add(string value)
        {
            if (!base.Contains(value))
                return base.Add(value);
            return -1;
        }

        new public void AddRange(string[] value)
        {
            foreach (string s in value)
            {
                if (!base.Contains(s))
                    base.Add(s);
            }
        }


    }
}
