using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ModuleUpdateTool : EditorWindow {
	private EditorWindow window;

	[MenuItem ("test/abab", false, 2)]
	public static void Test () {
		// window = EditorWindow.GetWindow<ModuleUpdateTool> ("模块更新工具");
		// window.minSize = new Vector2 (500, 500);
		// window.maxSize = new Vector2 (1500, 1500);
		// window.Show ();

		string path = System.Environment.CurrentDirectory + @"\Assets\LAZ_Support\Editor\LAZ_CheckTool\CheckTool";
		var git = new CommandRunner ("git", path);
		var status = git.Run (@"log --pretty=oneline " + path);
		string [] text = status.Split('\n');
		string test = text[0].Substring(0,5);
		UnityEngine.Debug.LogError(test);
		var sss = git.Run (@"reset --hard " + test);
		UnityEngine.Debug.LogError (sss);
	}

}

public class CommandRunner {
	public string ExecutablePath { get; }
	public string WorkingDirectory { get; }

	public CommandRunner (string executablePath, string workingDirectory = null) {
		ExecutablePath = executablePath;
		WorkingDirectory = workingDirectory ?? Path.GetDirectoryName (executablePath);
	}

	public string Run (string arguments) {
		var info = new ProcessStartInfo (ExecutablePath, arguments) {
			CreateNoWindow = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				WorkingDirectory = WorkingDirectory,
		};

		var process = new Process {
			StartInfo = info,
		};
		process.Start ();
		return process.StandardOutput.ReadToEnd ();
	}
}