using System;
using System.Data;

namespace NetroMedia.SharedFunctions
{
	public class DBField
	{
		#region Fields

		private string _fieldName;
		private DataRow  _row;

		#endregion

		#region Constructor

		public DBField(DataRow row, string fieldName)
		{
			_row = row;
			_fieldName = fieldName;
		}

		#endregion

		#region Public methods

		public  object  Value
		{
			get 
			{
				return _row[_fieldName];
			}
			set
			{
				_row[_fieldName] = value;
			}
		}

		new public string ToString()
		{
			if ((Value == null) || (Value == DBNull.Value))
				return "";
			else
				return Value.ToString();
		}

		public int ToInt()
		{
			if ((Value == null) || (Value == DBNull.Value))
				return 0;
			else
				return (int) Value;
		}

		public DateTime ToDateTime()
		{
			if ((Value == null) || (Value == DBNull.Value))
				return DateTime.MinValue;
			else
				return (DateTime) Value;
		}

		public bool ToBool()
		{
			if ((Value == null) || (Value == DBNull.Value))
				return false;
			else
				return Convert.ToBoolean(Value);
		}

        public byte[] ToBytes()
        {
            if ((Value == null) || (Value == DBNull.Value))
                return new byte[0];
            else
                return (byte[])Value;
        }

		#endregion
	}
}
