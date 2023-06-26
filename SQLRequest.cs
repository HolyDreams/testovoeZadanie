using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
                if (!File.Exists("SQLite.db"))
                {
                    File.Copy(Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("\\bin\\")) + "\\SQLite.db", "SQLite.db");
                }
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
