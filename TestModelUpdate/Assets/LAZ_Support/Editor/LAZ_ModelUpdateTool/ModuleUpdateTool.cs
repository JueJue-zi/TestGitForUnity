using UnityEditor;
using UnityEngine;

public class ModuleUpdateTool : EditorWindow {
	private static EditorWindow window;

	private string logText = string.Empty;
	private string description = string.Empty;

	[MenuItem ("test/abab", false, 2)]
	public static void Test () {
		window = EditorWindow.GetWindow<ModuleUpdateTool> ("模块更新工具");
		window.minSize = new Vector2 (500, 500);
		window.maxSize = new Vector2 (1500, 1500);
		window.Show ();

		//string path = Application.dataPath + ModelUpdateConfig.LAZ_CheckTool;// @"\Assets\LAZ_Support\Editor\LAZ_CheckTool\CheckTool";
		//Debug.LogError(path);
		// var git = new CommandRunner ("git", "");
		// var status = git.Run (@"log --pretty=oneline " + path);
		// UnityEngine.Debug.LogError(status);
		// string [] text = status.Split('\n');
		// string test = text[0].Substring(0,5);
		// UnityEngine.Debug.LogError(test);
		// var sss = git.Run (@"reset --hard " + test);
		// UnityEngine.Debug.LogError (sss);
	}

	void OnGUI () {
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("LAZ_CheckTool Version: 555555");
		if (GUILayout.Button ("查看日志", GUILayout.Height (20f))) {
			logText = GetGitLog (ModelUpdateConfig.LAZ_CheckTool);
		}
		if (GUILayout.Button ("更新至某版本(请查询log后填写5位数)", GUILayout.Height (20f))) {
			string tip = GitReset(ModelUpdateConfig.LAZ_CheckTool, description);
			Debug.LogError(tip);
		}
		if (GUILayout.Button ("立即更新", GUILayout.Height (20f))) {

		}
		GUILayout.EndHorizontal ();
		GUILayout.Label ("ResetVesion", GUILayout.MaxWidth (80));
		description = EditorGUILayout.TextArea (description, GUILayout.MaxHeight (20));


		GUILayout.Space (10);
		GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
		GUILayout.BeginVertical ();
		GUILayout.Label (logText);
		GUILayout.EndVertical ();
	}

	string GetGitLog (string logPath) {
		if (string.IsNullOrEmpty (logPath)) {
			EditorUtility.DisplayDialog ("错误", "请联系制作工具相关人员并记录下操作过程", "确认", "取消");
			return string.Empty;
		}
		string path = Application.dataPath + logPath;
		var git = new CommandRunner ("git", string.Empty);
		var status = git.Run (@"log --pretty=oneline " + path);
		return status;
	}

	string GitReset (string Path,string md5Code) {
		if (string.IsNullOrEmpty (Path)) {
			EditorUtility.DisplayDialog ("错误", "请联系制作工具相关人员并记录下操作过程", "确认", "取消");
			return string.Empty;
		}
		string path = Application.dataPath + Path;
		var git = new CommandRunner ("git", string.Empty);
		var status = git.Run (@"reset --hard " + md5Code);
		return status;
	}
}