using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class IntegralTool : EditorWindow {

		static SelectIdentity.ID_TYPE nowSelectType;
		public static void InitIdTool (SelectIdentity.ID_TYPE select_ID) {
			nowSelectType = select_ID;
			window = EditorWindow.GetWindow<IntegralTool> ("身份工具");
			window.minSize = new Vector2 (500, 420);
			window.maxSize = new Vector2 (1500, 1000);
			InitPath();
			window.Show ();

		}

		void OnGUI () {
			GUILayout.Label (@"默认路径为: Assets\Resources");
			GUILayout.Label("工具中能修改的路径必须选择Assets下目录的文件或文件夹!");
			switch (nowSelectType) {
				case SelectIdentity.ID_TYPE.ARTIST:
					InitAritstTool ();
					break;
				case SelectIdentity.ID_TYPE.DESIGNER:
					InitDesigner ();
					break;
				case SelectIdentity.ID_TYPE.PRORGAMMER:
					InitProrgammer ();
					break;
				case SelectIdentity.ID_TYPE.QA:
					InitQA ();
					break;
				case SelectIdentity.ID_TYPE.TA:
					InitTA ();
					break;
				default:
					EditorUtility.DisplayDialog ("错误", "请联系制作工具相关人员并记录下操作过程", "确认", "取消");
					break;
			}
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("修改身份", GUILayout.Width (100), GUILayout.Height (30))) {
				window.Close ();
				SelectIdentity.DoID ();
			}
			GUILayout.EndHorizontal ();
			//UpdatePath();
		}

		static EditorWindow window;
		static string goPath = string.Empty;
		static string animatorPath = string.Empty;
		static string effectPath = string.Empty;
		static string prefabMaterialpath = string.Empty;
		static string scriptMissingPath = string.Empty;
		static string prefabAnimatorPath = string.Empty;
		static string assetPath = string.Empty;
		static string modelPath = string.Empty;
		
		static Rect goRect;//Go检查对象
		static Rect playerAnimator;//动画检查对象
		static Rect effectRect;//特效检查对象
		static Rect prefabMaterialRect;//预设材质对象
		static Rect scriptMissingRect;//预设脚本对象
		static Rect prefabMissingRect;//预设脚本以及音效对象
		static Rect assetRect;//资源对象
		static Rect modelRect; //模型

		static void InitPath()
		{
			if (string.IsNullOrEmpty (assetPath)) assetPath = @"Assets\Resources";
			if (string.IsNullOrEmpty (goPath)) goPath = @"Assets\Resources";
			if (string.IsNullOrEmpty (animatorPath)) animatorPath = @"Assets\Resources";
			if (string.IsNullOrEmpty (effectPath)) effectPath = @"Assets\Resources";
			if (string.IsNullOrEmpty (prefabMaterialpath)) prefabMaterialpath = @"Assets\Resources";
			if (string.IsNullOrEmpty (scriptMissingPath)) scriptMissingPath = @"Assets\Resources";
			if (string.IsNullOrEmpty (prefabAnimatorPath)) prefabAnimatorPath = @"Assets\Resources";
			if (string.IsNullOrEmpty (modelPath)) modelPath = @"Assets\Resources";
		}

		static void UpdatePath()
		{
			Event evt = Event.current;
			switch (evt.type) {
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (goRect.Contains (evt.mousePosition)) {
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						if (evt.type == EventType.DragPerform) {
							DragAndDrop.AcceptDrag ();
							foreach (var item in DragAndDrop.objectReferences) {
								goPath = AssetDatabase.GetAssetPath (item);
								break;
							}
						}
					}
					if (playerAnimator.Contains (evt.mousePosition)) {
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						if (evt.type == EventType.DragPerform) {
							DragAndDrop.AcceptDrag ();
							foreach (var item in DragAndDrop.objectReferences) {
								animatorPath = AssetDatabase.GetAssetPath (item);
								break;
							}
						}
					}
					if (effectRect.Contains (evt.mousePosition)) {
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						if (evt.type == EventType.DragPerform) {
							DragAndDrop.AcceptDrag ();
							foreach (var item in DragAndDrop.objectReferences) {
								effectPath = AssetDatabase.GetAssetPath (item);
								break;
							}
						}
					}
					if (prefabMaterialRect.Contains (evt.mousePosition)) {
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						if (evt.type == EventType.DragPerform) {
							DragAndDrop.AcceptDrag ();
							foreach (var item in DragAndDrop.objectReferences) {
								prefabMaterialpath = AssetDatabase.GetAssetPath (item);
								break;
							}
						}
					}
					if (scriptMissingRect.Contains (evt.mousePosition)) {
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						if (evt.type == EventType.DragPerform) {
							DragAndDrop.AcceptDrag ();
							foreach (var item in DragAndDrop.objectReferences) {
								scriptMissingPath = AssetDatabase.GetAssetPath (item);
								break;
							}
						}
					}
					if (prefabMissingRect.Contains (evt.mousePosition)) {
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						if (evt.type == EventType.DragPerform) {
							DragAndDrop.AcceptDrag ();
							foreach (var item in DragAndDrop.objectReferences) {
								prefabAnimatorPath = AssetDatabase.GetAssetPath (item);
								break;
							}
						}
					}
					if (assetRect.Contains (evt.mousePosition)) {
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						if (evt.type == EventType.DragPerform) {
							DragAndDrop.AcceptDrag ();
							foreach (var item in DragAndDrop.objectReferences) {
								assetPath = AssetDatabase.GetAssetPath (item);
								break;
							}
						}
					}
					break;
			}
		}

		//美术
		static void InitAritstTool () {
			if (GUILayout.Button ("贴图工具", GUILayout.Height (35f))) {
				window.Close ();
				Indra.TextureTool.TextureListWindow.OpenTexWindow ();
			}
			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
			GUILayout.Label ("请在Project界面选择文件或目录!");
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("刷新模型顶部到底部的顶点色", GUILayout.Width (200), GUILayout.Height (30))) {
				CharacterGlintVertexColorTool.Open ();
			}
			GUILayout.EndHorizontal ();
			GUILayout.Label ("禁用模型读写");
			GUILayout.BeginHorizontal ();
			GUI.Label (GUILayoutUtility.GetRect (0, 20f, GUILayout.Width (100)), "当前路径:");
			modelRect = GUILayoutUtility.GetRect (0, 22f, GUILayout.ExpandWidth (true));
			GUI.TextField (modelRect, modelPath);
			if (GUILayout.Button ("选择路径", GUILayout.Width (100), GUILayout.Height (30))) {
				modelPath = EditorUtility.OpenFolderPanel ("选择需要检查的文件路径", string.Empty, string.Empty);
			}
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("禁用模型读写")) {
				if (!string.IsNullOrEmpty (modelPath)) {
					string cPath = ConversionPath (modelPath);
					if (string.IsNullOrEmpty (cPath)) return;
					string[] prefabs = AssetDatabase.FindAssets ("t:model", new string[] { cPath });
					FbxSimplify.CheckVertexColor (prefabs);
					FbxSimplify.CheckMultiUVs (prefabs);
				} else {
					EditorUtility.DisplayDialog ("提醒", "路径为空!请选择你需要检查的路径!", "OK");
				}
			}

			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
			GUILayout.Label ("根据路径提供检测资源大小");
			GUILayout.BeginHorizontal ();
			GUI.Label (GUILayoutUtility.GetRect (0, 20f, GUILayout.Width (100)), "当前路径:");
			assetRect = GUILayoutUtility.GetRect (0, 22f, GUILayout.ExpandWidth (true));
			GUI.TextField (assetRect, assetPath);
			if (GUILayout.Button ("选择路径", GUILayout.Width (100), GUILayout.Height (30))) {
				assetPath = EditorUtility.OpenFolderPanel ("选择需要检查的文件路径", string.Empty, string.Empty);
			}
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("检测资源大小")) {
				if (!string.IsNullOrEmpty (assetPath)) {
					string cPath = ConversionPath (assetPath);
					if(string.IsNullOrEmpty(cPath))return;
					string[] prefabs = AssetDatabase.FindAssets ("t:Texture", new string[] { cPath });
					CheckTool._CheckAssetsSize(prefabs);
				} else {
					EditorUtility.DisplayDialog ("提醒", "路径为空!请选择你需要检查的路径!", "OK");
				}
			}
		}

		//策划
		static void InitDesigner () {
			if (GUILayout.Button ("一键检测工程正确性")) {
				CheckTool.MeterialChecker ();
			}
		}

		//程序
		static void InitProrgammer () {

			GUIStyle aa= new GUIStyle();
			aa.normal.background = null; //这是设置背景填充的
			aa.normal.textColor=new Color(1,0,0);   //设置字体颜色的
			aa.fontSize = 10;
			GUILayout.Label ("工程正确性(GO对象引用丢失)",aa);
			if (GUILayout.Button ("一键检测工程正确性")) {
				CheckTool.MeterialChecker ();
			}

			#region  Go对象检查工具
			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
			GUIStyle bb=new GUIStyle();
			bb.normal.background = null; //这是设置背景填充的
			bb.normal.textColor=new Color(1,0,0);   //设置字体颜色的
			bb.fontSize = 10;
			GUILayout.Label ("工程正确性(GO对象引用丢失)",bb);
			GUILayout.BeginHorizontal ();
			GUI.Label (GUILayoutUtility.GetRect (0, 20f, GUILayout.Width (100)), "当前路径:");
			goRect = GUILayoutUtility.GetRect (0, 22f, GUILayout.ExpandWidth (true));
			 GUI.TextField (goRect, goPath);
			if (GUILayout.Button ("选择路径", GUILayout.Width (100), GUILayout.Height (30))) {
				goPath = EditorUtility.OpenFolderPanel ("选择需要检查的文件路径", string.Empty, string.Empty);
			}
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("GO对象引用丢失")) {
				if (!string.IsNullOrEmpty (goPath)) {
					string cPath = ConversionPath (goPath);
					if(string.IsNullOrEmpty(cPath))return;
					string[] prefabs = AssetDatabase.FindAssets ("t:Prefab", new string[] { cPath });
					CheckTool._CheckFindMissingReferences (prefabs);
				} else {
					EditorUtility.DisplayDialog ("提醒", "路径为空!请选择你需要检查的路径!", "OK");
				}
			}
			#endregion

			#region 工程正确性(删除角色时装和武器预制体空的Animator)
			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
			GUIStyle cc= new GUIStyle();
			cc.normal.background = null; //这是设置背景填充的
			cc.normal.textColor=new Color(1,0,0);   //设置字体颜色的
			cc.fontSize = 10;
			GUILayout.Label ("删除角色时装和武器预制体空的Animator",cc);
			GUILayout.BeginHorizontal ();
			GUI.Label (GUILayoutUtility.GetRect (0, 20f, GUILayout.Width (100)), "当前路径:");
			playerAnimator = GUILayoutUtility.GetRect (0, 22f, GUILayout.ExpandWidth (true));
			GUI.TextField (playerAnimator, animatorPath);
			if (GUILayout.Button ("选择路径", GUILayout.Width (100), GUILayout.Height (30))) {
				animatorPath = EditorUtility.OpenFolderPanel ("选择需要检查的文件路径", string.Empty, string.Empty);
			}

			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("工程正确性(删除角色时装和武器预制体空的Animator)")) {
				if (!string.IsNullOrEmpty (animatorPath)) {
					string cPath = ConversionPath (animatorPath);
					if(string.IsNullOrEmpty(cPath))return;
					string[] prefabs = AssetDatabase.FindAssets ("t:Prefab", new string[] { cPath });
					CheckTool._PlayerBadAnimatorDeleter (prefabs);
				} else {
					EditorUtility.DisplayDialog ("提醒", "路径为空!请选择你需要检查的路径!", "OK");
				}
			}
			#endregion

			#region  特效检查
			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
			GUIStyle dd= new GUIStyle();
			dd.normal.background = null; //这是设置背景填充的
			dd.normal.textColor=new Color(1,0,0);   //设置字体颜色的
			dd.fontSize = 10;
			GUILayout.Label ("工程正确性(检查特效丢失)",dd);
			GUILayout.BeginHorizontal ();
			GUI.Label (GUILayoutUtility.GetRect (0, 20f, GUILayout.Width (100)), "当前路径:");
			effectRect = GUILayoutUtility.GetRect (0, 22f, GUILayout.ExpandWidth (true));
			GUI.TextField (effectRect, effectPath);
			if (GUILayout.Button ("选择路径", GUILayout.Width (100), GUILayout.Height (30))) {
				effectPath = EditorUtility.OpenFolderPanel ("选择需要检查的文件路径", string.Empty, string.Empty);
			}

			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("检查特效丢失")) {
				if (!string.IsNullOrEmpty (effectPath)) {
					string cPath = ConversionPath (effectPath);
					if(string.IsNullOrEmpty(cPath))return;
					string[] prefabs = AssetDatabase.FindAssets ("t:Prefab", new string[] { cPath });
					CheckTool._CheckEffectScriptMissing (prefabs);
				} else {
					EditorUtility.DisplayDialog ("提醒", "路径为空!请选择你需要检查的路径!", "OK");
				}
			}
			#endregion

			#region  prefab材质检查
			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
			GUIStyle ee= new GUIStyle();
			ee.normal.background = null; //这是设置背景填充的
			ee.normal.textColor=new Color(1,0,0);   //设置字体颜色的
			ee.fontSize = 10;
			GUILayout.Label ("工程正确性(prefab材质)", ee);
			GUILayout.BeginHorizontal ();
			GUI.Label (GUILayoutUtility.GetRect (0, 20f, GUILayout.Width (100)), "当前路径:");
			prefabMaterialRect = GUILayoutUtility.GetRect (0, 22f, GUILayout.ExpandWidth (true));
			GUI.TextField (prefabMaterialRect, prefabMaterialpath);
			if (GUILayout.Button ("选择路径", GUILayout.Width (100), GUILayout.Height (30))) {
				prefabMaterialpath = EditorUtility.OpenFolderPanel ("选择需要检查的文件路径", string.Empty, string.Empty);
			}

			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("prefab材质")) {
				if (!string.IsNullOrEmpty (prefabMaterialpath)) {
					string cPath = ConversionPath (prefabMaterialpath);
					if(string.IsNullOrEmpty(cPath))return;
					string[] prefabs = AssetDatabase.FindAssets ("t:Prefab", new string[] { cPath });
					CheckTool._CheckFindMissingReferences (prefabs);
				} else {
					EditorUtility.DisplayDialog ("提醒", "路径为空!请选择你需要检查的路径!", "OK");
				}
			}
			#endregion

			#region  脚本丢失以及音效脚本
			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
			GUIStyle ff= new GUIStyle();
			ff.normal.background = null; //这是设置背景填充的
			ff.normal.textColor=new Color(1,0,0);   //设置字体颜色的
			ff.fontSize = 10;
			GUILayout.Label ("工程正确性(脚本丢失以及音效脚本)", ff);
			GUILayout.BeginHorizontal ();
			GUI.Label (GUILayoutUtility.GetRect (0, 20f, GUILayout.Width (100)), "当前路径:");
			scriptMissingRect = GUILayoutUtility.GetRect (0, 22f, GUILayout.ExpandWidth (true));
			GUI.TextField (scriptMissingRect, scriptMissingPath);
			if (GUILayout.Button ("选择路径", GUILayout.Width (100), GUILayout.Height (30))) {
				scriptMissingPath = EditorUtility.OpenFolderPanel ("选择需要检查的文件路径", string.Empty, string.Empty);
			}

			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("脚本丢失以及音效脚本")) {
				if (!string.IsNullOrEmpty (scriptMissingPath)) {
					string cPath = ConversionPath (scriptMissingPath);
					if(string.IsNullOrEmpty(cPath))return;
					string[] prefabs = AssetDatabase.FindAssets ("t:Prefab", new string[] { cPath });
					CheckTool._CheckScriptMissing (prefabs);
				} else {
					EditorUtility.DisplayDialog ("提醒", "路径为空!请选择你需要检查的路径!", "OK");
				}
			}
			#endregion

			#region 动画丢失
			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
			GUIStyle gg= new GUIStyle();
			gg.normal.background = null; //这是设置背景填充的
			gg.normal.textColor=new Color(1,0,0);   //设置字体颜色的
			gg.fontSize = 10;
			GUILayout.Label ("工程正确性(动画丢失)", gg);
			GUILayout.BeginHorizontal ();
			GUI.Label (GUILayoutUtility.GetRect (0, 20f, GUILayout.Width (100)), "当前路径:");
			prefabMissingRect = GUILayoutUtility.GetRect (0, 22f, GUILayout.ExpandWidth (true));
			GUI.TextField (prefabMissingRect, prefabAnimatorPath);
			if (GUILayout.Button ("选择路径", GUILayout.Width (100), GUILayout.Height (30))) {
				prefabAnimatorPath = EditorUtility.OpenFolderPanel ("选择需要检查的文件路径", string.Empty, string.Empty);
			}

			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("动画丢失)")) {
				if (!string.IsNullOrEmpty (prefabAnimatorPath)) {
					string cPath = ConversionPath (prefabAnimatorPath);
					if(string.IsNullOrEmpty(cPath))return;
					string[] prefabs = AssetDatabase.FindAssets ("t:Prefab", new string[] { cPath });
					CheckTool._CheckAnimationException (prefabs);
				} else {
					EditorUtility.DisplayDialog ("提醒", "路径为空!请选择你需要检查的路径!", "OK");
				}
			}

			GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
			GUIStyle hh= new GUIStyle();
			hh.normal.background = null; //这是设置背景填充的
			hh.normal.textColor=new Color(1,0,0);   //设置字体颜色的
			hh.fontSize = 10;
			GUILayout.Label ("根据路径提供检测资源大小", hh);
			GUILayout.BeginHorizontal ();
			GUI.Label (GUILayoutUtility.GetRect (0, 20f, GUILayout.Width (100)), "当前路径:");
			assetRect = GUILayoutUtility.GetRect (0, 22f, GUILayout.ExpandWidth (true));
			GUI.TextField (assetRect, assetPath);
			if (GUILayout.Button ("选择路径", GUILayout.Width (100), GUILayout.Height (30))) {
				assetPath = EditorUtility.OpenFolderPanel ("选择需要检查的文件路径", string.Empty, string.Empty);
			}
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("检测资源大小")) {
				if (!string.IsNullOrEmpty (assetPath)) {
					string cPath = ConversionPath (assetPath);
					if(string.IsNullOrEmpty(cPath))return;
					string[] prefabs = AssetDatabase.FindAssets ("t:Texture", new string[] { cPath });
					CheckTool._CheckAssetsSize(prefabs);
				} else {
					EditorUtility.DisplayDialog ("提醒", "路径为空!请选择你需要检查的路径!", "OK");
				}
			}
			#endregion
		}

		//测试
		static void InitQA () {
			if (GUILayout.Button ("一键检测工程正确性")) {
				CheckTool.MeterialChecker ();
			}
		}

		//TA
		static void InitTA () {
			InitProrgammer();
		}

		static string ConversionPath (string str) {
				if (string.IsNullOrEmpty (str)) return string.Empty;
				string Str = string.Empty;
				int i = str.IndexOf ("Assets", 0);
				if(i == -1)
				{
					bool abc = EditorUtility.DisplayDialog ("提醒", "请选择Assets下的文件夹!", "","Cancel");
					if(abc)
						return string.Empty;
					else
						return string.Empty;
				}
				Str = str.Substring (i);
				Str.Replace ("/", @"\");
				return Str;
	}
}