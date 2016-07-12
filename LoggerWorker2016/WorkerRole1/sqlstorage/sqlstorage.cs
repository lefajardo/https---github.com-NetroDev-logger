using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace NetroLogger_WorkerRole
{
    public static class sqlstorage
    {
        public static void generateRollupsSqlStorage()
        {

            SqlConnection dbConnection = new SqlConnection(RoleEnvironment.GetConfigurationSettingValue("AzureSqlConnectionString").ToString());

            try
            {
                dbConnection.Open();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("err on calculating rollups create conn " + ex.Message);
            }

            // move from logstaging to logmaster
            try
            {
                SqlCommand cmd = dbConnection.CreateCommand();
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spLOG_STAGING_TO_MASTER5";
                cmd.ExecuteNonQuery();
                cmd = null;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("err on staging to master " + ex.Message);
            }

            //generate hour and daily rollups
            try
            {
                SqlCommand cmd = dbConnection.CreateCommand();
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPROCESS_ROLLUPS";
                cmd.ExecuteNonQuery();
                cmd = null;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("err on process rollups sql " + ex.Message);
            }

            dbConnection.Close();
            dbConnection = null;

        }
    }
}
