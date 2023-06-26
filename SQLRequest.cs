using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace testovoeZadanie
{
    internal class SQLRequest
    {
        public static DataTable SQLite(string sqlRequest)
        {
            try
            {
                using (var connect = new SqliteConnection("Data Source=SQLite.db"))
                {
                    connect.Open();

                    DataTable dt = new DataTable();
                    SqliteCommand cmd = new SqliteCommand(sqlRequest, connect);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            dt.Load(reader);
                        }
                    }

                    connect.Close();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
