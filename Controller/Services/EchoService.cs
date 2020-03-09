using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using GeniePlugin.Interfaces;

namespace FluffMuff.Services
{
    class EchoService
    {
        private Task executionTask;
        private readonly IHost _host;
        private readonly SQLiteConnection _connection;

        public EchoService(IHost host, SQLiteConnection connection)
        {
            _connection = connection;
            _host = host;
            InitTask();
        }

        public void InitTask()
        {
            executionTask = new Task(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    ProcessEchoes();
                }
            });
        }

        static string GetConnectionString()
        {
            return "Data Source=mf.sqlite;Version=3;";
        }

        public void Run()
        {

            executionTask.Start();
        }

        private void ProcessEchoes()
        {
                _connection.Open();

                string sql = $"select * from Echo order by id asc";
                SQLiteCommand command = new SQLiteCommand(sql, _connection);
                var result = command.ExecuteReader();
                
                while(result.Read())
                {
                    var id = result.GetInt16(0);
                    var data = result.GetString(1);

                    _host.SendText($"#Echo >FluffMuff {data}");

                    using(var deleteCommand = new SQLiteCommand($"delete from Echo where id = {id}", _connection))
                    {
                        deleteCommand.ExecuteNonQuery();
                    }

                }

                _connection.Close();
        }

    }
}
