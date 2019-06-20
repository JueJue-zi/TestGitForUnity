using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OptimizeToolWindow : EditorWindow {

		private static EditorWindow window;

		[MenuItem ("Laz/LAZ_OptimizeTool", false, 1)]
		public static void OptimizeTool () {
			window = EditorWindow.GetWindow<OptimizeToolWindow> ("优化工具");
			window.minSize = new Vector2 (500, 420);
			window.maxSize = new Vector2 (1500, 1000);
			window.Show ();
		}

		void OnGUI () {
			Init ();
		}

		string modelDvPath = string.Empty;
		string modelNormalAndTangentPath = string.Empty;

		static Rect modelDvRect; //预设脚本对象
		static Rect modelNormalAndTangentRect; //预设脚本以及音效对象

		void Init () {
			if (string.IsNullOrEmpty (modelDvPath)) modelDvPath = @"Assets\Resources";
			if (string.IsNullOrEmpty (modelNormalAndTangentPath)) modelNormalAndTangentPath = @"Assets\Resources";
			if (GUILayout.Button ("贴图工具", GUILayout.Height (35f))) {
				window.Close ();
				Indra.TextureTool.TextureListWindow.OpenTexWindow ();
			}
			if (GUILayout.Button ("刷新模型顶部到底部的顶点色", GUILayout.Height (35))) {
				CharacterGlintVertexColorTool.Open ();
			}

			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));

			GUILayout.Label ("移除模型法线和切线信息");
			GUILayout.BeginHorizontal ();
			GUI.Label (GUILayoutUtility.GetRect (0, 20f, GUILayout.Width (100)), "当前路径:");
			modelDvRect = GUILayoutUtility.GetRect (0, 22f, GUILayout.ExpandWidth (true));
			GUI.TextField (modelDvRect, modelDvPath);
			if (GUILayout.Button ("选择路径", GUILayout.Width (100), GUILayout.Height (30))) {
				modelDvPath = EditorUtility.OpenFolderPanel ("选择需要检查的文件路径", string.Empty, string.Empty);
			}
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("移除模型法线和切线信息")) {
				if (!string.IsNullOrEmpty (modelDvPath)) {
					string cPath = ConversionPath (modelDvPath);
					if (string.IsNullOrEmpty (cPath)) return;
					string[] prefabs = AssetDatabase.FindAssets ("t:model", new string[] { cPath });
					FbxSimplify.RemoveNormalAndTangent (prefabs);
				} else {
					EditorUtility.DisplayDialog ("提醒", "路径为空!请选择你需要检查的路径!", "OK");
				}
			}

			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));

			GUILayout.Label ("禁用模型读写");
			GUILayout.BeginHorizontal ();
			GUI.Label (GUILayoutUtility.GetRect (0, 20f, GUILayout.Width (100)), "当前路径:");
			modelNormalAndTangentRect = GUILayoutUtility.GetRect (0, 22f, GUILayout.ExpandWidth (true));
			GUI.TextField (modelNormalAndTangentRect, modelNormalAndTangentPath);
			if (GUILayout.Button ("选择路径", GUILayout.Width (100), GUILayout.Height (30))) {
				modelNormalAndTangentPath = EditorUtility.OpenFolderPanel ("选择需要检查的文件路径", string.Empty, string.Empty);
			}
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("禁用模型读写")) {
				if (!string.IsNullOrEmpty (modelNormalAndTangentPath)) {
					string cPath = ConversionPath (modelNormalAndTangentPath);
					if (string.IsNullOrEmpty (cPath)) return;
					string[] prefabs = AssetDatabase.FindAssets ("t:model", new string[] { cPath });
					FbxSimplify.RemoveTangents (prefabs);
				} else {
					EditorUtility.DisplayDialog ("提醒", "路径为空!请选择你需要检查的路径!", "OK");
				}
			}
		}

		static string ConversionPath (string str) {
				if (string.IsNullOrEmpty (str)) return string.Empty;
				string Str = string.Empty;
				int i = str.IndexOf ("Assets", 0);
				if (i == -1) {
					bool abc = EditorUtility.DisplayDialog ("提醒", "请选择Assets下的文件夹!", "", "Cancel");
					if (abc)
						return string.Empty;
					else
						return string.Empty;
				}
				Str = str.Substring (i);
				Str.Replace ("/", @"\");
				return Str;
	}
}