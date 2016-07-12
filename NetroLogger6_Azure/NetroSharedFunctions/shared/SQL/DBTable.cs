using System;
using System.Data;

namespace NetroMedia.SharedFunctions
{
    /// <summary>
    /// Summary description for DBTable.
    /// </summary>
    public class DBTable
    {
        #region Fields

        private string _query = "";
        private DataTable _table;
        private string _connectionString;

        #endregion

        #region Constructor

        public DBTable(string query, string connectionString)
        {
            _connectionString = connectionString;

            _table = new SqlManager(connectionString).LoadTable(query);
            _query = query;
        }

        public DBTable()
        {
            _table = new DataTable();
            _query = "";
        }


        #endregion

        #region Public methods

        public void Update()
        {
            new SqlManager(_connectionString).SaveTable(_query, _table);
        }

        public DataRow NewRow()
        {
            DataRow myRow = _table.NewRow();
            _table.Rows.Add(myRow);

            return myRow;
        }

        #endregion

        #region Properties

        public DataTable Table
        {
            get { return _table; }
        }

        public int RowCount
        {
            get { return _table.Rows.Count; }
        }

        public DBField this[int rowNum, string fieldName]
        {
            get { return new DBField(this._table.Rows[rowNum], fieldName); }
        }

        public DBField this[string fieldName]
        {
            get
            {
                return new DBField(this._table.Rows[0], fieldName);

            }
        }

        #endregion
    }
}
