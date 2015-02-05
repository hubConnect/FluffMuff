using System;
using GeniePlugin.Interfaces;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace FluffMuff
{
	public class FluffMuff : IPlugin
	{
		string _NAME = "Fluff Muff";
		string _VERSION = "1";
		string _AUTHOR = "Jon Kotowski";
		string _DESCRIPTION = "Cut down on spam and stuff.";
		private bool _enabled = true; 
		private bool Muffing = false;
		Hashtable lines = new Hashtable();
		System.Threading.Tasks.Task t;

		IHost _host; 

		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}
		public void ParentClosing()
		{
		}

		public void VariableChanged(string Variable)
		{
		}

		public string Name
		{
			get { return _NAME; }
		}

		//Required for Plugin - Called when Genie needs the plugin version (error text
		//                      or the plugins window)
		//Return Value:
		//              string: Text that is the version of the plugin
		public string Version
		{
			get { return _VERSION; }
		}

		//Required for Plugin - Called when Genie needs the plugin Author (plugins window)
		//Return Value:
		//              string: Text that is the Author of the plugin
		public string Author
		{
			get { return _AUTHOR; }
		}

		//Required for Plugin - Called when Genie needs the plugin Description (plugins window)
		//Return Value:
		//              string: Text that is the description of the plugin
		//                      This can only be up to 200 Characters long, else it will appear
		//                      "truncated"
		public string Description
		{
			get { return _DESCRIPTION; }
		}

		public void Initialize(IHost Host)
		{
			_host = Host;  
		}

		//Required for Plugin - Called when user enters text in the command box
		//Parameters:
		//              string Text:  The text the user entered in the command box
		//Return Value:
		//              string: Text that will be sent to the game
		public string ParseInput(string Text)
		{
			if (Text.StartsWith("/muff")) {
				if (Text.ToLower ().EndsWith ("on")) {

					_host.EchoText ("** Muffing enabled.");
					Muffing = true;
				} else if (Text.ToLower ().EndsWith ("off")) {

					_host.EchoText ("** Muffing disabled.");
					Muffing = false;
				} else if (Text.ToLower ().EndsWith ("reset")) {	

					lines.Clear ();
				} else if (Text.ToLower ().EndsWith ("save")) {
//
//					using (System.IO.StreamWriter file = new System.IO.StreamWriter(_host.get_Variable ("charactername") + "muff.txt"))
//					{
//						foreach (string line in lines.Keys)
//						{
//							// If the line doesn't contain the word 'Second', write the line to the file. 
//							if (!line.Contains("Second"))
//							{
//								file.WriteLine(line);
//								file.WriteLine (lines [line]);
//							}
//						}
//
//						file.Close ();
//					}
					WriteFile ();
				} else if (Text.ToLower ().EndsWith ("load")) {
					String path = _host.get_Variable ("charactername") + "muff.txt";
					ReadFile ();
				}
				else {

					_host.EchoText ("############################################################");
					_host.EchoText ("#### Commands -");
					_host.EchoText ("#### on/off -- Turn the muffing on or off.");
					_host.EchoText ("#### reset  -- Clear out your muffed lines.");
					_host.EchoText ("#### save   -- Save your muff lines to a file.");
					_host.EchoText ("#### load   -- Load previously saved lines.");
					_host.EchoText ("############################################################");
				}
			}
			return Text;
		}

		public string ParseText(string Text, string Window)
		{

			if (Window.Equals ("main")) {
				if (Muffing) {
					Muffit (Text);
				}
			}
			return Text;
		}
		private void WriteFile()
		{
			try {

				BinaryFormatter bfw = new BinaryFormatter();

				StreamWriter ws = new System.IO.StreamWriter (_host.get_Variable ("charactername") + "muff.txt");
				bfw.Serialize(ws.BaseStream, lines);

				_host.EchoText("File saved.");
			} catch (Exception e) {
				_host.EchoText ("File save error.");
			}
		}

		private async void ReadFile()
		{
			lines = new Hashtable ();
			try
			{
				StreamReader ws = new System.IO.StreamReader (_host.get_Variable ("charactername") + "muff.txt");
				BinaryFormatter bf = new BinaryFormatter();
				lines = (Hashtable)bf.Deserialize(ws.BaseStream);
				_host.EchoText("File loaded.");
			}
			catch (Exception ex)
			{
				_host.EchoText ("Read file error");
			}
		}

		async private void Muffit(string Text) 
		{

			await System.Threading.Tasks.Task.Run (() => {

				string sPattern = "^\\w+ (say|says|exclaims|asks)|^\\w+ \\w+ (say|says|exclaims|asks)";

				if(System.Text.RegularExpressions.Regex.IsMatch(Text, sPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ) {

					_host.SendText ("#echo >FluffMuff " + Text);
					return;
				}

				if (!lines.Contains (Text)) {

					lines.Add (Text, 1);
				} else {

					if ((int)lines [Text] < 15) {

						lines [Text] = (int)lines [Text] + 1;
					}
				}

				if ((int)lines [Text] < 3 ) {

					_host.SendText ("#echo >FluffMuff " + Text);
				}
			});
		}
		public void ParseXML(string XML)
		{
		}
		public void Show()
		{
			_host.EchoText ("############################################################");
			_host.EchoText ("#### To use, open a window named FluffMuff");
			_host.EchoText ("############################################################");
			_host.EchoText ("#### Commands -");
			_host.EchoText ("#### on/off -- Turn the muffing on or off.");
			_host.EchoText ("#### reset  -- Clear out your muffed lines.");
			_host.EchoText ("#### save   -- Save your muff lines to a file.");
			_host.EchoText ("#### load   -- Load previously saved lines.");
			_host.EchoText ("############################################################");
			_host.EchoText ("#### Muffing is disabled by default, so remember to turn ");
			_host.EchoText ("#### it on.");
			_host.EchoText ("############################################################");
		}

		public void OpenSettingsWindow(System.Windows.Forms.Form parent)
		{
		}


	}
}

