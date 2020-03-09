using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Controller.Stores
{
    class FluffService
    {
        FluffService()
        {
            GetConnectionStrings();
        }
        private string fileString => "mf.sqlite";

        static void GetConnectionStrings()
        {
            ConnectionStringSettingsCollection settings =
                ConfigurationManager.ConnectionStrings;

            if (settings != null)
            {
                foreach (ConnectionStringSettings cs in settings)
                {
                    Console.WriteLine(cs.Name);
                    Console.WriteLine(cs.ProviderName);
                    Console.WriteLine(cs.ConnectionString);
                }
            }
        }
        private void initDb()
        {
            if (!System.IO.File.Exists(fileString))
            {
                SQLiteConnection.CreateFile(fileString);

                using (var sqlite = new SQLiteConnection(connectionString))
                {
                    sqlite.Open();
                    string sql = $"CREATE TABLE Fluff ( data TEXT not null );";
                    SQLiteCommand command = new SQLiteCommand(sql, sqlite);
                    command.ExecuteNonQuery();
                    sqlite.Close();
                }
            }
        }

        private void insertLine(string text)
        {
            using (var sqlite = new SQLiteConnection(connectionString))
            {
                sqlite.Open();
                string sql = $"insert into Fluff (data) values ('{text}');";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite);
                command.ExecuteNonQuery();
                sqlite.Close();
            }
        }

        private bool doesDBContainLine(string text)
        {
            using (var sqlite2 = new SQLiteConnection(connectionString))
            {
                sqlite2.Open();
                string sql = $"select count(*) from Fluff where data like '%{text}%'";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                var result = (long)command.ExecuteScalar();
                sqlite2.Close();

                return result > 0;
            }
        }

    }
}
