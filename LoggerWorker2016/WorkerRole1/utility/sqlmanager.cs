using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Collections.Generic;

namespace NetroLogger_WorkerRole.utility
{
    public class SqlManager
    {
        #region Fields

        private string _connectionString = "";

        #endregion

        #region Constructor
        
        public SqlManager(string connectionString)
        {            
            _connectionString = connectionString;
        } 

        #endregion

        #region Properties
        
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        #endregion

        #region Public methods
        /*
        public static string getConnectionString(ServiceToParse service, AppConfig _appConfig)
        {
            string connectionString = "";

            switch (service)
            {
                case ServiceToParse.QuickTime:
                    connectionString = "data source=" + _appConfig.ServerIP
                    + ";initial catalog=" + _appConfig.Database
                    + ";password=" + _appConfig.Password
                    + ";persist security info=True;user id=" + _appConfig.UserName + ";";
                    break;

                case ServiceToParse.Wowza:
                    connectionString = "data source=" + _appConfig.ServerIPWowNonLog
                    + ";initial catalog=" + _appConfig.DatabaseWowNonLog
                    + ";password=" + _appConfig.PasswordWowNonLog
                    + ";persist security info=True;user id=" + _appConfig.UserNameWowNonLog + ";";
                    break;
                case ServiceToParse.WinMedia:
                    connectionString = "data source=" + _appConfig.ServerIPWmNonLog
                    + ";initial catalog=" + _appConfig.DatabaseWmNonLog
                    + ";password=" + _appConfig.PasswordWmNonLog
                    + ";persist security info=True;user id=" + _appConfig.UserNameWmNonLog + ";";
                    break;
                case ServiceToParse.ICEcast:
                    connectionString = "data source=" + _appConfig.ServerIPIceNonLog
                        + ";initial catalog=" + _appConfig.DatabaseIceNonLog
                        + ";password=" + _appConfig.PasswordIceNonLog
                        + ";persist security info=True;user id=" + _appConfig.UserNameIceNonLog + ";";
                    break;
                case ServiceToParse.MP3:
                    connectionString = "data source=" + _appConfig.ServerIPNonLog
                        + ";initial catalog=" + _appConfig.DatabaseNonLog
                        + ";password=" + _appConfig.PasswordNonLog
                        + ";persist security info=True;user id=" + _appConfig.UserNameNonLog + ";";
                    break;
                case ServiceToParse.SHOUTcast:
                    connectionString = "data source=" + _appConfig.ServerIPShoutNonLog
                        + ";initial catalog=" + _appConfig.DatabaseShoutNonLog
                        + ";password=" + _appConfig.PasswordShoutNonLog
                        + ";persist security info=True;user id=" + _appConfig.UserNameShoutNonLog + ";";
                    break;
            }

            return connectionString;
        }
        */
        public bool isCentralDBOnline() {
            SqlConnection dbConnection = new SqlConnection(_connectionString);
            try
            {

                dbConnection.Open();
                dbConnection.Close();
                return true;

            }
            catch
            {
             
                return false;

            }
        }

        public bool DropTable(string tablename) {
            SqlConnection dbConnection = new SqlConnection(_connectionString);

            SqlCommand cmd = dbConnection.CreateCommand();
            cmd.CommandText = "Drop Table " + tablename ;
            cmd.CommandType = CommandType.Text ;


            try {

                dbConnection.Open();

                cmd.ExecuteNonQuery();

                dbConnection.Close();
                return true;

            }
            catch {
                dbConnection.Close();
                return false;

            }
        }

        public int ExecuteStoredProcedureEscalar(string storedProcedure, List<SqlParameter> sqlParameters)
        {
            int result;

            SqlConnection dbConnection = new SqlConnection(_connectionString);

            SqlCommand cmd = dbConnection.CreateCommand();
            cmd.CommandText = storedProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;

            foreach (SqlParameter parameter in sqlParameters)
            {
                cmd.Parameters.Add(parameter);
            }

            
            dbConnection.Open();

            result = Convert.ToInt32(cmd.ExecuteScalar());

            dbConnection.Close();
            return result;
        }


        public DataTable  ExecuteStoredProcedureTable(string storedProcedure, List<SqlParameter> sqlParameters)
        {
            DataTable  result;

            SqlConnection dbConnection = new SqlConnection(_connectionString);

            SqlCommand cmd = dbConnection.CreateCommand();
            cmd.CommandText = storedProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;

            foreach (SqlParameter parameter in sqlParameters)
            {
                cmd.Parameters.Add(parameter);
            }


            dbConnection.Open();

            result = convertDataReaderToDataSet( cmd.ExecuteReader( ) ).Tables[0];


            dbConnection.Close();
            return result;
        }

        public static DataSet convertDataReaderToDataSet(SqlDataReader reader)
        {
            DataSet dataSet = new DataSet();
            do
            {
                // Create new data table

                DataTable schemaTable = reader.GetSchemaTable();
                DataTable dataTable = new DataTable();

                if (schemaTable != null)
                {
                    // A query returning records was executed

                    for (int i = 0; i < schemaTable.Rows.Count; i++)
                    {
                        DataRow dataRow = schemaTable.Rows[i];
                        // Create a column name that is unique in the data table
                        string columnName = (string)dataRow["ColumnName"]; //+ "<C" + i + "/>";
                        // Add the column definition to the data table
                        DataColumn column = new DataColumn(columnName, (Type)dataRow["DataType"]);
                        dataTable.Columns.Add(column);
                    }

                    dataSet.Tables.Add(dataTable);

                    // Fill the data table we just created

                    while (reader.Read())
                    {
                        DataRow dataRow = dataTable.NewRow();

                        for (int i = 0; i < reader.FieldCount; i++)
                            dataRow[i] = reader.GetValue(i);

                        dataTable.Rows.Add(dataRow);
                    }
                }
                else
                {
                    // No records were returned

                    DataColumn column = new DataColumn("RowsAffected");
                    dataTable.Columns.Add(column);
                    dataSet.Tables.Add(dataTable);
                    DataRow dataRow = dataTable.NewRow();
                    dataRow[0] = reader.RecordsAffected;
                    dataTable.Rows.Add(dataRow);
                }
            }
            while (reader.NextResult());
            return dataSet;
        }



        public bool ExecuteStoredProcedure(string storedProcedure, List<SqlParameter> sqlParameters)
        {
            SqlConnection dbConnection = new SqlConnection(_connectionString);

            
            SqlCommand cmd = dbConnection.CreateCommand();
            cmd.CommandText = storedProcedure ;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            foreach ( SqlParameter parameter in sqlParameters ){
                cmd.Parameters.Add( parameter );
            }

            dbConnection.Open();

            if (cmd.ExecuteNonQuery() < 1)
            {
                dbConnection.Close();
                return false;
            }

            dbConnection.Close();
            return true;
        }


        public bool ExecuteNonQuery(string query)
        {
            SqlConnection dbConnection = new SqlConnection(_connectionString);

            SqlCommand cmd = dbConnection.CreateCommand();
            cmd.CommandText = query;
            cmd.CommandTimeout = 0;
            dbConnection.Open();

            if (cmd.ExecuteNonQuery() < 1)
            {
                dbConnection.Close();
                return false;
            }

            dbConnection.Close();
            return true;
        }

        public static bool ExecuteNonQuery(string query, SqlTransaction trn, SqlConnection dbConnection)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbConnection;
            cmd.Transaction = trn;
            cmd.CommandText = query;
            cmd.CommandTimeout = 0;
            dbConnection.Open();

            if (cmd.ExecuteNonQuery() < 1)
            {
                
                return false;
            }

            
            return true;
        }
        
        public DataTable LoadTable(string query, string connect)
        {
            DataTable dTable = new DataTable();
            if (query == "")
            {
                return dTable;
            }
            SqlConnection dbConnection  = new SqlConnection(connect);
            SqlDataAdapter dAdapter     = new SqlDataAdapter(query, dbConnection);

                dbConnection.Open();
                dAdapter.Fill(dTable);
                dbConnection.Close();

            return dTable;
        }

        public DataTable LoadTable(string query)
        {
            return LoadTable(query, _connectionString);
        }

        public void SaveTable(string query, DataTable dTable, string connect)
        {
            SqlConnection dbConnection  = new SqlConnection(connect);
            SqlDataAdapter dAdapter     = new SqlDataAdapter(query, dbConnection);
            SqlCommandBuilder cBuilder  = new SqlCommandBuilder(dAdapter);

            dbConnection.Open();
            dAdapter.Update(dTable);
            dbConnection.Close();
        }

        public void SaveTable(string query, DataTable dTable)
        {
            SaveTable(query, dTable, _connectionString);
        }

        

        #endregion
    }
}