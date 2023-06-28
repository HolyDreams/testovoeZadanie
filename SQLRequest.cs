using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace testovoeZadanie
{
    internal class SQLRequest
    {
        static string Password = "ПумПумПум";
        private static void hideDB()
        {
            var file = File.ReadAllBytes("SQLite.db").ToList();
            var passwordByte = Encoding.UTF8.GetBytes(Password);
            file.AddRange(passwordByte);
            var stream = File.Create("dbFile");
            stream.Write(file.ToArray(), 0, file.Count);
            stream.Close();

            File.Delete("SQLite.db");
        }
        private static void showDB()
        {
            var file = File.ReadAllBytes("dbFile").ToList();
            var passwordByte = Encoding.UTF8.GetBytes(Password).Count();
            file.RemoveRange(file.Count - passwordByte, passwordByte);
            var stream = File.Create("SQLite.db");
            stream.Write(file.ToArray(), 0, file.Count);
            stream.Close();
        }
        public static DataTable SQLite(string sqlRequest)
        {
            try
            {
                if (!File.Exists("dbFile"))
                    File.Copy(Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("\\bin\\")) + "\\dbFile", "dbFile");

                showDB();
                var connect = new SQLiteConnection("Data Source=SQLite.db");
                connect.Open();
                DataTable dt = new DataTable();
                var adapter = new SQLiteDataAdapter(sqlRequest, connect);
                adapter.Fill(dt);

                connect.Close();
                adapter.Dispose();

                hideDB();
                return dt;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                hideDB();
                return null;
            }
        }
    }
}
