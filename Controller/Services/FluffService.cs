using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluffMuff.Utils;
using GeniePlugin.Interfaces;

namespace FluffMuff.Stores
{
    class FluffService
    {

        private Task executionTask;
        private WhitelistStore whitelistStore;

        private List<string> fluffToProcess;

        private IHost _host;

        public bool debug = false;
        private readonly SQLiteConnection _connection;
 
        public FluffService(IHost host, SQLiteConnection connection)
        {
            _connection = connection;
            _host = host;

            whitelistStore = new WhitelistStore();
            fluffToProcess = new List<string>();

            InitTask();
        }
        
        private string fileString => "mf.sqlite";


        

        private bool WhitelistContainsItem(string item)
        {
            return whitelistStore.GetWhitelist().Any((s =>
            {
                var reg = new Regex(s);
                var match = reg.Match(item);

                return match.Success;
            } ) );
        }

        public void QueueItem(string text)
        {
            fluffToProcess.Add(text);
        }

        private void InsertLineIntoFluff(string text)
        {
            _connection.Open();

            string sql = $"insert into Fluff (data) values ('{text}');";
            SQLiteCommand command = new SQLiteCommand(sql, _connection);
            command.ExecuteNonQuery();

            _connection.Close();
        }

        private void insertLineForEcho(string text)
        {
            _connection.Open();
            string sql = $"insert into Echo (data) values ('{text}');";
            SQLiteCommand command = new SQLiteCommand(sql, _connection);
            command.ExecuteNonQuery();
            _connection.Close();
        }

        public void InitTask()
        {
            executionTask = new Task(() =>
            {
                while (true)
                {
                    Thread.Sleep(250);
                    ProcessFluff();
                }
            });
        }


        void ProcessFluff()
        {
            var itemsToProcess = fluffToProcess.ToList();
            fluffToProcess = new List<string>();

            if (debug)
                _host.EchoText("[DEBUG] Tick");

            itemsToProcess.ForEach(item =>
            {
                if ( !WhitelistContainsItem(item))
                {
                    var sanitizedLine = item.Sanitize();

                    if (sanitizedLine == "\r\n") return;
                    
                    if (!DoesDbContainLine(sanitizedLine))
                    {
                        if (debug)
                        {
                            _host.EchoText($"[DEBUG] {item}");
                        }

                        _host.SendText($"#echo >FluffMuff {item}");
                        //insertLineForEcho(item);
                        InsertLineIntoFluff(sanitizedLine);
                    }
                }
            });
        }

        public void Run()
        {

            executionTask.Start();
        }

    private bool DoesDbContainLine(string text)
        {
            _connection.Open();

            string sql = $"select count(*) from Fluff where data like '%{text}%'";
            SQLiteCommand command = new SQLiteCommand(sql, _connection);
            var result = (long)command.ExecuteScalar();

            _connection.Close();

            return result > 0;
        }


    }
}
