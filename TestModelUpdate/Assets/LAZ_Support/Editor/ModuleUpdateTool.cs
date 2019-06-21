using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class ModuleUpdateTool : EditorWindow {
	private static EditorWindow window;

	[MenuItem ("test/abab", false, 2)]
	public static void Test () {
		// window = EditorWindow.GetWindow<ModuleUpdateTool> ("模块更新工具");
		// window.minSize = new Vector2 (500, 500);
		// window.maxSize = new Vector2 (1500, 1500);
		// window.Show ();

		string path = System.Environment.CurrentDirectory ;//+ @"\Assets\LAZ_Support\Editor\LAZ_CheckTool\CheckTool";

		var status = Run ("st", path);
		UnityEngine.Debug.LogError (status);
	}

	public static string Run (string arguments, string WorkingDirectory) {
		var info = new ProcessStartInfo ("git", arguments) {
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