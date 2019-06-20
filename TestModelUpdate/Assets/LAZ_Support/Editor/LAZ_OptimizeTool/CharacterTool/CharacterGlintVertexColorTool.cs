using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class CharacterGlintVertexColorTool : EditorWindow {
    public static void Open() {
        EditorWindow.GetWindow<CharacterGlintVertexColorTool>().Show();
    }

    private ReorderableList m_List;
    private List<GameObject> m_DataList;
    void OnEnable() {
        m_DataList = new List<GameObject>();
        m_List = new ReorderableList(m_DataList, typeof(GameObject), false, false, true, true);
		m_List.headerHeight = 3;
		m_List.elementHeight = EditorGUIUtility.singleLineHeight + 4;
		m_List.drawElementCallback = DrawDataElement;
        m_List.onAddCallback = DoNotCreate;
        heightMax = 10;
        heightMin = 0;
    }

    void DoNotCreate(ReorderableList list) {
        m_DataList.Add(null);
    }

    void OnDisable() {
        m_DataList.Clear();
        m_DataList = null;
    }

    private float heightMax;
    private float heightMin;
    private bool autoHeightArea = true;

    private GameObject root;

    void OnGUI() {
        if (GUILayout.Button("绘制顶点色")) {
            Do();
        }
        autoHeightArea = EditorGUILayout.Toggle("自动计算高度范围", autoHeightArea);
        if (!autoHeightArea) {
            EditorGUILayout.MinMaxSlider("高度范围:" + heightMin + "~" + heightMax, ref heightMin, ref heightMax, 0, 30);
        }
        EditorGUILayout.HelpBox("绘制成功以后会在fbx同目录生成新的mesh文件，记住要把它们也签入！", MessageType.Info);
        
        m_List.DoLayoutList();
    }

    void DrawDataElement(Rect rect, int index, bool selected, bool focused) {
        rect.y += 2;
        rect.x += 5;
        rect.width -= 10;
        rect.height -= 4;
		m_DataList[index] = EditorGUI.ObjectField(rect, m_DataList[index], typeof(GameObject), false) as GameObject;
	}

    void Do() {
        root = new GameObject();
        var goList = new GameObject[m_DataList.Count];
        var goIndex = 0;
        // var mat = new Material(Shader.Find("Hidden/EditorToolShader/EditorCharacterVertexColorShader"));
        foreach(var prefab in m_DataList) {
            var go = GameObject.Instantiate(prefab);
            go.transform.SetParent(root.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            // var gomrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            // foreach(var mr in gomrs) {
            //     mr.sharedMaterial = mat;
            // }
            goList[goIndex++] = go;
        }
        if (autoHeightArea) {
            CalculateHeightArea();
        }


        var mrs = root.GetComponentsInChildren<SkinnedMeshRenderer>();
        var area = heightMax - heightMin;
        foreach(var mr in mrs) {
            var mesh = mr.sharedMesh;
            var path = AssetDatabase.GetAssetPath(mesh);
            var pathExt = path.Substring(path.Length - 4);
            if (pathExt.ToUpper().Equals(".FBX")) {
                var newMesh = GameObject.Instantiate(mesh) as Mesh;
                mr.sharedMesh = newMesh;
                
                var vertArray = newMesh.vertices;
                var newColor = new Color[vertArray.Length];
                for(var i = 0; i < vertArray.Length; i++) {
                    var vert = vertArray[i];
                    var worldPos = mr.transform.localToWorldMatrix * new Vector4(vert.x, vert.y, vert.z, 1);
                    newColor[i] = Color.Lerp(Color.black, Color.red, (worldPos.y - heightMin) / area);
                }
                newMesh.colors = newColor;

                var newMeshPath = path.Substring(0, path.Length - 4) + "_" + mesh.name + ".asset";
                AssetDatabase.CreateAsset(newMesh, newMeshPath);
            }
        }
        
        for(var i = 0; i < m_DataList.Count; i++) {
            m_DataList[i] = PrefabUtility.ReplacePrefab(goList[i], m_DataList[i]);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }

    void CalculateHeightArea() {
        heightMin = float.MaxValue;
        heightMax = float.MinValue;
        var mrs = root.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(var mr in mrs) {
            var bounds = mr.bounds;
            heightMin = Mathf.Min(bounds.min.y, heightMin);
            heightMax = Mathf.Max(bounds.max.y, heightMax);
        }
    }

}