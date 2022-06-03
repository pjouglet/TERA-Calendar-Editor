using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TERAResources;

namespace TERACalendar
{
    internal static class SQLManager
    {
        private static SqlConnection _connection;
        private static SqlConnection GetSqlConnection()
        {
            if(_connection is null)
            {
                _connection = new SqlConnection($"Server={Configuration.DbUrl};Database=PlanetDB_2800;UID={Configuration.DbUser};PWD={Configuration.DbPassword}");
                _connection.Open();
            }
            return _connection;
        }

        public static void Close()
        {
            _connection?.Close();
        }

        public static void AddItem(int itemId, int amount, DateTime start)
        {
            CreateAttendanceEventIfNotExist(start);

            SqlConnection connection = GetSqlConnection();

            SqlParameter dateParameter = new SqlParameter();
            dateParameter.ParameterName = "@compensationDate";
            dateParameter.Value = start;
            dateParameter.SqlDbType = SqlDbType.DateTime;

            SqlParameter itemParameter = new SqlParameter();
            itemParameter.ParameterName = "@itemTemplateId";
            itemParameter.Value = itemId;
            itemParameter.SqlDbType = SqlDbType.Int;

            SqlParameter amountParameter = new SqlParameter();
            amountParameter.ParameterName = "@amount";
            amountParameter.Value = amount;
            amountParameter.SqlDbType = SqlDbType.Int;

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "dbo.spAddAttendanceEventCompensation";
            cmd.Parameters.Add(dateParameter);
            cmd.Parameters.Add(itemParameter);
            cmd.Parameters.Add(amountParameter);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }

        public static void UpdateItem(int itemId, int amount, DateTime start)
        {
            SqlConnection connection = GetSqlConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = $"UPDATE dbo.AttendanceEventCompensations SET itemTemplateId={itemId}, amount={amount} WHERE compensationDate='{start.ToString("yyyy-MM-dd HH:mm:ss:fff")}'";
            cmd.ExecuteNonQuery();
        }

        public static void DeleteItem(DateTime start)
        {
            SqlConnection connection = GetSqlConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM dbo.AttendanceEventCompensations WHERE compensationDate='{start.ToString("yyyy-MM-dd HH:mm:ss:fff")}'";
            cmd.ExecuteNonQuery();
        }

        private static void CreateAttendanceEventIfNotExist(DateTime start)
        {
            DateTime startMonth = new DateTime(start.Year, start.Month, 1, 0, 0, 0);
            DateTime endMonth = new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month), 23, 59, 59);
            SqlConnection connection = GetSqlConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT identityNo FROM dbo.AttendanceEventInfo WHERE startTime='{startMonth.ToString("yyyy-MM-dd HH:mm:ss:fff")}' AND endTime='{endMonth.ToString("yyyy-MM-dd HH:mm:ss:fff")}'";

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Close();
                return;
            }

            reader.Close();

            SqlParameter startParameter = new SqlParameter();
            startParameter.ParameterName = "@startTime";
            startParameter.Value = startMonth;
            startParameter.SqlDbType = SqlDbType.DateTime;

            SqlParameter endParameter = new SqlParameter();
            endParameter.ParameterName = "@endTime";
            endParameter.Value = endMonth;
            endParameter.SqlDbType = SqlDbType.DateTime;

            cmd = connection.CreateCommand();
            cmd.CommandText = "dbo.spAddAttendanceEventInfo";
            cmd.Parameters.Add(startParameter);
            cmd.Parameters.Add(endParameter);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }

        public static SqlDataReader GetitemsFromDB()
        {
            List<TeraItem> items = new List<TeraItem>();
            SqlConnection connection = GetSqlConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM dbo.AttendanceEventCompensations";

            return cmd.ExecuteReader();
        }
    }
}
