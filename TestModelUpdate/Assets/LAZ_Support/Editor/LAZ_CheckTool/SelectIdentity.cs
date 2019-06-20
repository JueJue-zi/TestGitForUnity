using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectIdentity : EditorWindow {

	public enum ID_TYPE {
		ARTIST = 0, //美术
		DESIGNER = 1, //策划
		PRORGAMMER = 2, //技术
		QA = 3, //测试
		TA = 4, //TA
	}

	public static ID_TYPE nowTypeId = ID_TYPE.DESIGNER; //默认为策划
	private string idTip = "";
	
	private static EditorWindow window;

	[MenuItem ("Laz/LAZ_CheckTool/身份工具集合(筛选出对应身份的工具)",false,1)]
	public static void DoID () {
		window = EditorWindow.GetWindow<SelectIdentity> ("检查工具");
		window.minSize = new Vector2 (235, 420);
		window.maxSize = new Vector2 (240, 425);
		window.Show ();
	}

	void OnGUI () {
		Init();
	}

	void Init()
	{
		GUILayout.Label("当前身份:" + IdName(nowTypeId));
		GUILayout.BeginVertical ();
		if (GUILayout.Button ("我是美术", GUILayout.Height (35f))) {
			idTip = "你已经确认自己是一名美术人员，请点击确认。";
			nowTypeId = ID_TYPE.ARTIST;
		}
		if (GUILayout.Button ("我是策划", GUILayout.Height (35f))) {
			idTip = "你已经确认自己是一名策划，请点击确认。";
			nowTypeId = ID_TYPE.DESIGNER;
		}
		if (GUILayout.Button ("我是程序", GUILayout.Height (35f))) {
			idTip = "你已经确认自己是一名程序，请点击确认。";
			nowTypeId = ID_TYPE.PRORGAMMER;
		}
		if (GUILayout.Button ("我是测试", GUILayout.Height (35f))) {
			idTip = "你已经确认自己是一名测试，请点击确认。";
			nowTypeId = ID_TYPE.QA;
		}
		if (GUILayout.Button ("我是TA", GUILayout.Height (35f))) {
			idTip = "你已经确认自己是一名TA，请点击确认。";
			nowTypeId = ID_TYPE.TA;
		}
		GUILayout.EndVertical ();
		GUILayout.Space (10);
		GUILayout.BeginVertical ();
		if (!string.IsNullOrEmpty (idTip)) {
			GUILayout.Label (idTip);
		}
		if(GUILayout.Button("确认", GUILayout.Height(35f)))
		{
			if(window != null)window.Close();
			IntegralTool.InitIdTool(nowTypeId);
		}
		GUILayout.Label("选择身份影响资源变动时检查规则!");
		GUILayout.EndVertical ();
	}

	private string IdName(ID_TYPE id_Type)
	{
		string name = string.Empty;
		switch (id_Type)
		{
			case ID_TYPE.ARTIST:
				name = "美术";
				break;
			case ID_TYPE.DESIGNER:
				name = "策划";
				break;
			case ID_TYPE.PRORGAMMER:
				name = "程序";
				break;
			case ID_TYPE.QA:
				name = "测试";
				break;
			case ID_TYPE.TA:
				name = "TA";
				break;
		}
		return name;
	}
	
}