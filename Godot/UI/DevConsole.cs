using Godot;
using System;
using VillageProject.Core.Enums;
using VillageProject.Godot.Debugging.DevConsoleCommands;

public partial class DevConsole : Panel
{
	
	private LineEdit LineInput { get; set; }
	private TextEdit LogTextBox { get; set; }
	private List<string> CommandLog { get; set; }
	private List<string> InputHistory { get; set; }
	private IList<IDevConsoleCommand> Commands { get; set; }

	private int history_index = -1; 
	private bool up_cache = false;
	private bool down_cache = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CommandLog = new List<string>();
		InputHistory = new List<string>();
		LineInput = GetNode<LineEdit>("BoxContainer/InputLine");
		LogTextBox = GetNode<TextEdit>("BoxContainer/ScrollContainer/TextEdit");
		LineInput.TextSubmitted += MyTextSubmittedEventHandler;
		Commands = LoadCommands();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(LineInput.HasFocus())
		{
			if (Input.IsKeyPressed(Key.Up) && InputHistory.Count > 0)
			{
				if(!up_cache)
				{
					history_index = Int32.Min(CommandLog.Count - 1, history_index + 1);
					LineInput.Text = InputHistory[history_index];
					LineInput.CaretColumn = LineInput.Text.Length;
					up_cache = true;
				}
			}
			else
			{
				up_cache = false;
			}

			if (Input.IsKeyPressed(Key.Down) && InputHistory.Count > 0)
			{
				if(!down_cache)
				{
					history_index = Int32.Max(0, history_index - 1);
					LineInput.Text = InputHistory[history_index];
					LineInput.CaretColumn = LineInput.Text.Length;
					down_cache = true;
				}
			}
			else
				down_cache = false;
		}
		else if(this.Visible)
		{
			this.Visible = false;
		}
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
			{
				this.Visible = false;
			}
			if (eventKey.Pressed && 
			    (eventKey.Keycode == Key.Asciitilde || eventKey.Keycode == Key.Quoteleft))
			{
				this.Visible = !this.Visible;
				if(this.Visible)
					LineInput.GrabFocus();
			}

		}
	}

	void testCommand()
	{
		var rot = RotationFlag.East.GetRotationDirection(RotationFlag.North);
		var t = true;
	}

	void MyTextSubmittedEventHandler(string text)
	{
		InputHistory.Add(text);
		history_index = InputHistory.Count;
		Console.WriteLine("Text:" + text);
		LineInput.Clear();
		var tokens = text.Split(" ");
		var command = Commands.Where(x => x.Tag == tokens[0]).SingleOrDefault();
		var log = "";
		if (command == null)
		{
			log = $"Unkown command '{tokens[0]}'";
			testCommand();
		}
		else
		{
			var res = command.RunCommand(text);
			log = text + " - " + res.Message;
		}
		CommandLog = CommandLog.Prepend(log).ToList();
		LogTextBox.Text = string.Join("\n", CommandLog);
	}

	private List<IDevConsoleCommand> LoadCommands()
	{
		var outList = new List<IDevConsoleCommand>();
		foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly()
			         .GetTypes()
			         .Where(mytype => mytype .GetInterfaces().Contains(typeof(IDevConsoleCommand))))
		{
			var inst = Activator.CreateInstance(type);
			if(inst != null)
				outList.Add((IDevConsoleCommand)inst);
		}

		return outList;
	}
}
