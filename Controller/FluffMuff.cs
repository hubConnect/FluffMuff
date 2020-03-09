using GeniePlugin.Interfaces;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Text.RegularExpressions;
using FluffMuff.Services;
using FluffMuff.Stores;

namespace FluffMuff
{
    public class FluffMuff : IPlugin
	{

		public string Name => "FluffMuff";

        public string Version => "1.0";

        public string Description => "Stuff";

        public string Author => "Harc It's Slarc";

        private string fileString => "mf.sqlite";
        private string connectionString => $"Data source={fileString};Version=3;";

        public bool Enabled { get; set; }

        public static string pattern = @"[@'>]";  
        IHost _host;

        private FluffService _fluffService;
        private EchoService _echoService;

        private SQLiteConnection _connection;

		public void Initialize(IHost host) {
            _connection = new SQLiteConnection(connectionString);
			_host = host;
			Enabled = true;
            InitDb();

            _fluffService = new FluffService(_host,_connection );
            _fluffService.Run();

//            _echoService = new EchoService(_host, _connection);
//            _echoService.Run();
        }

		public void Show() {
		}

        private void InitDb() {
            if (!System.IO.File.Exists(fileString)) {
                SQLiteConnection.CreateFile(fileString);
                _connection.Open();
                string sql = $"CREATE TABLE Fluff ( id INTEGER PRIMARY KEY, data TEXT not null ); CREATE TABLE Echo ( id INTEGER PRIMARY KEY, data TEXT not null ); ";
                SQLiteCommand command = new SQLiteCommand(sql, _connection);
                command.ExecuteNonQuery();
                _connection.Close();
                
            }
        }

        private void SetDebug(bool status)
        {
            _fluffService.debug = status;
        }

        public void VariableChanged( string variable ) {
		}

        public string ParseText(string input, string window) {
            if (input == "" || window != "main" || input == " ") return input;
            
            _fluffService.QueueItem(input);

            return input;
		}

		public string ParseInput( string input ) {
            if (input.StartsWith("/debug on"))
                SetDebug(true);
            else if (input.StartsWith("/debug off"))
                 SetDebug(false);

			return input;
		}

		public void ParseXML( string xml ) {
		}

		public void ParentClosing() {
		}
	}
}

