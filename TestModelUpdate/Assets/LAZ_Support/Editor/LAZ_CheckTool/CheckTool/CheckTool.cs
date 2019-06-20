using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System;
using System.Data;
using System.Reflection;

public class CheckTool: EditorWindow
{
    public enum  ClcikCheckType
    {
        script,
        animator,
        shader,
        gameObject,
        missAnimator,
        effect,

    }

#region 单独检查集合
    static CheckTool PlayerBadAnimatorDeleter()
    {
        var window = GetWindow<CheckTool>();
        window.titleContent = new GUIContent("工程正确性集合工具");
        return window;
    }

    private void OnGUI()
    {
        GUILayout.Label("工程正确性(删除角色时装和武器预制体空的Animator)");
        GUILayout.Label("请选中目录或多个目标后点击下方!");
        if (GUILayout.Button("工程正确性(删除角色时装和武器预制体空的Animator) Start Check!"))
        {
            if(OpenSelectTip())
            {
            }
        }

        GUILayout.Label("\n");
        GUILayout.Label("工程正确性(GO对象引用丢失)");
        GUILayout.Label("请选中目录或多个目标后点击下方!");
        if(GUILayout.Button("工程正确性(GO对象引用丢失) Start Check!"))
        {
            if(OpenSelectTip())
            {
            }
        }

        GUILayout.Label("\n");
        GUILayout.Label("工程正确性(检查特效丢失)");
        GUILayout.Label("请选中目录或多个目标后点击下方!");
        if(GUILayout.Button("工程正确性(检查特效丢失) Start Check!"))
        {
            if(OpenSelectTip())
            {
            }
        }

        GUILayout.Label("\n");
        GUILayout.Label("工程正确性(检查脚本丢失和音效脚本丢失)");
        GUILayout.Label("请选中目录或多个目标后点击下方!");
        if(GUILayout.Button("工程正确性(检查脚本丢失和音效脚本丢失) Start Check!"))
        {
            if(OpenSelectTip())
            {
            }
        }

        GUILayout.Label("\n");
        GUILayout.Label("工程正确性(检查动画组件丢失)");
        GUILayout.Label("请选中目录或多个目标后点击下方!");
        if(GUILayout.Button("工程正确性(检查动画组件丢失) Start Check!"))
        {
            if(OpenSelectTip())
            {
            }
        }
    }

    static bool OpenSelectTip()
    {
        UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
        string path = AssetDatabase.GetAssetPath(arr[0]);
        bool select = EditorUtility.DisplayDialog("选中路径!","请确认当前选中路径\n(如选择根文件夹,可能会耗时较久请注意!) \n" + path, "确认", "取消");
        return select;
    }

#endregion

    public static void _PlayerBadAnimatorDeleter(string[] guids)
    {
        try
        {
            foreach (var guid in guids)
            {
                string objDir = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(objDir);
                GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                Animator[] animators = go.GetComponentsInChildren<Animator>(true);

                
                bool prefabDirty = false;
                foreach (var item in animators)
                {
                    if (item.runtimeAnimatorController == null)
                    {
                        prefabDirty = true;
                        GameObject.DestroyImmediate(item);
                    }
                }
                if (prefabDirty)
                {
                    PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ConnectToPrefab);
                }
                GameObject.DestroyImmediate(go);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }
        Debug.LogError("检查完毕! 数量:" + guids.Length);
        AssetDatabase.Refresh();
    }


    [MenuItem("Laz/LAZ_CheckTool/工程正确性一键检查(GO Miss|材质丢失|脚本丢失|Shader丢失|特效丢失)", false, 2)]
    public static void MeterialChecker()
    {
        GetWhiteList();

        ClearConsole();
        BeginWriteLog();
        GetWhiteList();//获取白名单列表

        ClearConsole(); //清空打印信息
        BeginWriteLog();
        
        string checkDir = @"Assets\Resources";
        string[] materials = AssetDatabase.FindAssets("t:Material", new string[] { checkDir });
        _CheckMaterialError(materials); //所有的材质检查
        
        string[] prefabs = AssetDatabase.FindAssets("t:Prefab", new string[] { checkDir });
        _CheckFindMissingReferences(prefabs);//Go对象miss检查
        _CheckEffectScriptMissing(prefabs);//特效检查
        _CheckScriptMissing(prefabs);//脚本丢失以及音效脚本
        _CheckMaterialMissing(prefabs);//prefab材质检查
        _CheckAnimationException(prefabs);//动画丢失


        EndWriteLog();
        ClearWhiteList();
        EditorUtility.DisplayDialog("检查完毕", "请通过Window/Console打开输出日志查看详细数据,也可以查看项目根目录的日志", "OK");
    }

    //static StringBuilder fileLog;
    static List<string> fileLog;
    static string filePath = "工程完整性报告.txt";

    static int maxTexTureSize = 4194304;

    static void BeginWriteLog()
    {
        //fileLog = new StringBuilder();
        if (fileLog == null) fileLog = new List<string>(); 
        //string curFilePath = string.Format("{0}/../{1}", Application.dataPath, filePath);
    }

    static void WriteLog(string log)
    {
        fileLog.Add(log);
        //fileLog.Append(log);
        //fileLog.Append("\n");
    }

    static void EndWriteLog()
    {
        System.IO.File.WriteAllLines(filePath, fileLog.ToArray());
        fileLog = null;
        //fileLog.Length = 0;
    }

    public static void ClearConsole()
    {
        var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
        if (logEntries != null)
        {
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
            GUIUtility.keyboardControl = 0;
        }
    }

    static void _CheckMaterialError(string[] guids)
    {
        EditorUtility.DisplayProgressBar("材质检查", "收集数据中...", 0);
        int i = 0;;
        foreach (var guid in guids)
        {
            string matdir = AssetDatabase.GUIDToAssetPath(guid);
            EditorUtility.DisplayProgressBar("材质检查", matdir, (float)i / guids.Length);
            Material mat = AssetDatabase.LoadAssetAtPath(matdir, typeof(Material)) as Material;
            if (mat.shader.name.Equals("Hidden/InternalErrorShader"))
            {
                Debug.LogError("<color=red>[shader丢失]</color>Error Material " + mat.name, mat);
                WriteLog("[shader丢失]Error Material " + mat.name);
            }
            i++;
        }
        EditorUtility.ClearProgressBar();
    }

    //+++++++++++++++++++++++++++++++++++检查Go对象丢失+++++++++++++++++++++++++++++++++++++++
    public static void _CheckFindMissingReferences(string[] guids)
    {
        EditorUtility.DisplayProgressBar("GO对象内Miss", "检查GO对象丢失", 0);
        List<GameObject> objects = new List<GameObject>();
        //int i = 0;
        EditorUtility.DisplayProgressBar("GO对象内Miss", "收集数据中...", 0);
        foreach (var guid in guids)
        {
            string objDir = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(objDir);
            if (objects == null) objects = new List<GameObject>();
            objects.Add(go);
        }

        int mCurrentIndex = 0;
        foreach (var go in objects)
        {
            EditorUtility.DisplayProgressBar("GO对象内Miss", "检查GO对象丢失", mCurrentIndex*1.0f/ objects.Count);
            mCurrentIndex++;
            _CheckGoMissingReferences(go);
        }
        EditorUtility.ClearProgressBar();
        Debug.LogError("检查完毕! 数量:" + guids.Length);
    }

    public static void _CheckGoMissingReferences(GameObject go)
    {
        var components = go.GetComponents<Component>();
        foreach (var c in components)
        {
            if (c == null)
            {
                Debug.LogError("Missing script found on: " + FullObjectPath(go));
                    //Debug.LogError(string.Format("<color=red>[Go引用Miss]</color> {0}\n", FullObjectPath(go)), go);
            }
            else
            {
                SerializedObject so = new SerializedObject(c);
                var sp = so.GetIterator();

                while (sp.NextVisible(true))
                {
                    if (sp.propertyType != SerializedPropertyType.ObjectReference)
                    {
                        continue;
                    }

                    if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
                    {
                            //ShowError(FullObjectPath(go), sp.name);
                        ShowError(go, sp.name);
                    }
                }
            }
        }
    }

    private static void ShowError(string objectName, string propertyName)
    {
        Debug.LogError("Missing reference found in: " + objectName + ", Property : " + propertyName);
        WriteLog("Go内丢失对象: Go名：" + objectName + ", 属性字段 : " + propertyName);
    }

    private static void ShowError(GameObject vGo, string propertyName)
    {
        Debug.LogError(string.Format("<color=red>[Go引用Miss]</color> {0}\n", FullObjectPath(vGo)), vGo);
        WriteLog("Go内丢失对象: Go名：" + FullObjectPath(vGo) + ", 属性字段 : " + propertyName);
    }
    
    private static string FullObjectPath(GameObject go)
    {
        return go.transform.parent == null ? go.name : FullObjectPath(go.transform.parent.gameObject) + "/" + go.name;
    }

    //+++++++++++++++++++++++++++++++++++特效上的脚本丢失了其它特效对象+++++++++++++++++++++++++++++++++++++++

    public static void _CheckEffectScriptMissing(string[] guids)
    {
        EditorUtility.DisplayProgressBar("特效检查", "收集数据中...", 0);
        int i = 0;
        foreach (var guid in guids)
        {
            string objDir = AssetDatabase.GUIDToAssetPath(guid);
            EditorUtility.DisplayProgressBar("特效检查", objDir, (float)i / guids.Length);

            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(objDir);
            _CheckEffectOpenMissing(go);
            _CheckEffectListTimeCtrlMissing(go);
            i++;
        }
        EditorUtility.ClearProgressBar();
        Debug.LogError("检查完毕! 数量:" + guids.Length);
    }

    public static bool _CheckEffectOpenMissing(GameObject go)
    {
        // FGUI 可能会采用新的特效脚本管理,暂时屏蔽此处检测

        // EffectOpen[] eo = go.transform.GetComponentsInChildren<EffectOpen>(true);
        bool result = false;
        // foreach (var e in eo)
        // {
        //     if (e.effect == null)
        //     {
        //         Debug.LogError(string.Format("<color=red>[特效]</color>{0} -> {1} EffectOpen丢特效了", go.name, e.name), go);
        //         WriteLog(string.Format("[特效]{0} -> {1} EffectOpen丢特效了", go.name, e.name));
        //         result = true;
        //     }
        // }

        return result;
    }

    public static bool _CheckEffectListTimeCtrlMissing(GameObject go)
    {
        // FGUI 可能会采用新的特效脚本管理,暂时屏蔽此处检测

        // EffectListTimeCtrl[] eltc = go.transform.GetComponentsInChildren<EffectListTimeCtrl>(true);
         bool result = false;
        // foreach (var e in eltc)
        // {
        //     if (e.ListEffect.Count != e.ListEffecStartTime.Count | _ListNullValue(e.ListEffect))
        //     {
        //         Debug.LogError(string.Format("<color=red>[特效]</color>{0} -> {1} EffectListItemCtrl丢特效了", go.name, e.name), go);
        //         WriteLog(string.Format("[特效]{0} -> {1} EffectListItemCtrl丢特效了", go.name, e.name));
        //         result = true;
        //     }
        // }
        
        return result;
    }

    public static bool _ListNullValue(List<GameObject> gos)
    {
        foreach (var item in gos)
        {
            if (item == null)
                return true;
        }
        return false;
    }
    //---------------------------------------------------------------------------------------

    //+++++++++++++++++++++检查脚本材质丢失的情况++++++++++++++++++++++++++++++++++++++++++++++++
    public static void _CheckMaterialMissing(string[] guids)
    {
        EditorUtility.DisplayProgressBar("检查材质丢失错误", "收集数据中...", 0);
        int i = 0;
        foreach (var guid in guids)
        {
            try
            {

                string objDir = AssetDatabase.GUIDToAssetPath(guid);
                EditorUtility.DisplayProgressBar("检查材质丢失错误", objDir, (float)i / guids.Length);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(objDir);
                string info = GetMatMissingInfo(go);
                if (!string.IsNullOrEmpty(info))
                {
                    Debug.LogError(string.Format("<color=red>[材质丢失]</color> {0}\n{1}", objDir, info), go);
                    WriteLog(string.Format("[材质丢失] {0}\n{1}", objDir, info));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
            }
            i++;
        }
        EditorUtility.ClearProgressBar();
        Debug.LogError("检查完毕! 数量:" + guids.Length);
    }

    public static string GetMatMissingInfo(GameObject go)
    {
        StringBuilder sb = new StringBuilder();
        if (CheckInWhiteList(go, CheckType.LostMaterial)) return sb.ToString();
        MeshRenderer[] mrs = go.GetComponentsInChildren<MeshRenderer>(true);
        SkinnedMeshRenderer[] smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        ParticleSystem[] pss = go.GetComponentsInChildren<ParticleSystem>(true);
        LineRenderer[] lrs = go.GetComponentsInChildren<LineRenderer>(true);
        foreach (var item in mrs)
        {
            foreach (var m in item.sharedMaterials)
            {
                if (m == null)
                {
                    sb.Append(string.Format("{0} 丢材质\n", item.name));
                    break;
                }
            }
        }
        foreach (var item in smrs)
        {
            foreach (var m in item.sharedMaterials)
            {
                if (m == null)
                {
                    sb.Append(string.Format("{0} 丢材质\n", item.name));
                    break;
                }
            }
        }
        foreach (var item in pss)
        {
            var psr = item.GetComponent<ParticleSystemRenderer>();
            if (psr != null)
            {
                if (psr.sharedMaterial == null)
                    sb.Append(string.Format("{0} 丢材质\n", item.name));
            }
        }
        foreach (var item in lrs)
        {
            foreach (var m in item.sharedMaterials)
            {
                if (m == null)
                {
                    sb.Append(string.Format("{0} 丢材质\n", item.name));
                    break;
                }
            }
        }
        return sb.ToString();
    }
    //--------------------------------------------------------------------------------------------

    //++++++++++++++++++++检查脚本丢失的情况++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public static void _CheckScriptMissing(string[] guids)
    {
        EditorUtility.DisplayProgressBar("检查脚本丢失问题", "收集数据中...", 0);
        int i = 0;
        foreach (var guid in guids)
        {
            string objDir = AssetDatabase.GUIDToAssetPath(guid);
            EditorUtility.DisplayProgressBar("检查脚本丢失问题", objDir, (float)i / guids.Length);
            try
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(objDir);
                string bug = doCheckScriptMissing(go);
                if (!string.IsNullOrEmpty(bug))
                {
                    Debug.LogError(string.Format("<color=red>[脚本丢失]</color>{0} \n{1}", objDir, bug), go);
                    WriteLog(string.Format("[脚本丢失]{0} \n{1}", objDir, bug));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
            }
            i++;
        }
        Debug.LogError("检查完毕! 数量:" + guids.Length);
        EditorUtility.ClearProgressBar();
    }

    public static string doCheckScriptMissing(GameObject go)
    {
        StringBuilder sb = new StringBuilder();
        Transform[] trans = go.GetComponentsInChildren<Transform>(true);
        foreach (var tran in trans)
        {
            var components = tran.GetComponents<Component>();
            foreach (var com in components)
            {
                if (com == null)
                {
                    sb.Append(string.Format("{0} 丢失了脚本\n", tran.name));
                    break;
                }
            }
        }
        return sb.ToString();
    }
    //--------------------------------------------------------------------------------------------

    //++++++++++++++++++++++++++++++++++动画丢失++++++++++++++++++++++++++++++++++++++++++++++++++
    public static void _CheckAnimationException(string[] guids)
    {
        EditorUtility.DisplayProgressBar("检查动画组件问题", "收集数据中...", 0);
        int i = 0;
        foreach (var guid in guids)
        {
            string objDir = AssetDatabase.GUIDToAssetPath(guid);
            EditorUtility.DisplayProgressBar("检查动画组件问题", objDir, (float)i / guids.Length);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(objDir);
            try
            {
                string info = GetAnimationBug(go);
                if (!string.IsNullOrEmpty(info))
                {
                    if (!isWarningLog)
                    {
                        Debug.LogError(string.Format("<color=red>[动画组件]</color> {0}\n{1}", objDir, info), go);
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("<color=red>[动画组件]</color> {0}\n{1}", objDir, info), go);
                    }
                    WriteLog(string.Format("[动画组件] {0}\n{1}", objDir, info));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(string.Format("{0} \n {1}", go.name, e));
            }
            i++;
        }
        EditorUtility.ClearProgressBar();
        Debug.LogError("检查完毕! 数量:" + guids.Length);
    }

    public static bool isWarningLog = false;

    /// <summary>
    /// 查找组件上丢失Controller、丢失Animation的情况
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static string GetAnimationBug(GameObject go)
    {
        isWarningLog = false;
        StringBuilder sb = new StringBuilder();
        Animator[] animators = go.GetComponentsInChildren<Animator>(true);
        Animation[] animations = go.GetComponentsInChildren<Animation>(true);
        foreach (var controller in animators)
        {
            if (controller.runtimeAnimatorController == null)
            {
                if (CheckInWhiteList(go, CheckType.LostController)) continue;
                sb.Append(string.Format("{0} Animator丢失控制器\n", controller.name));
                continue;
            }
            AnimatorController ac = controller.runtimeAnimatorController as AnimatorController;
            if (ac == null) // AnimatorOverrideController 暂时忽略
            {
                continue;
            }
            AnimatorControllerLayer[] layers = ac.layers;
            foreach (var layer in layers)
            {
                var stateMachine = layer.stateMachine;
                if (stateMachine == null)
                    continue;
                ChildAnimatorState[] states = stateMachine.states;
                isWarningLog = CheckInWhiteList(go, CheckType.LostMotion) && string.IsNullOrEmpty(sb.ToString());
                foreach (var item in states)
                {
                    if (CheckInWhiteList(go, CheckType.LostMotion, item.state.name)) continue;
                    if (item.state.motion == null)
                    {
                        sb.Append(string.Format(" {0} 上挂载的Animator组件的State: {1} 丢失动画\n", controller.name, item.state.name));
                        // sb.Append(string.Format("<color=blue>{0}</color>上挂载的Animator组件的State:<color=blue>{1}</color>丢失动画\n", controller.name, item.state.name));
                    }
                }
            }
        }
        //if (GetComponentDoAction<TextDamage>(go, sb, GetAnimationBug_CheckDefaultClipWhiteList))
        //{
        //    return sb.ToString();
        //}
        foreach (var anim in animations)
        {
            if (CheckInWhiteList(go, CheckType.LostClip)) break;
            AnimationClip[] clips = AnimationUtility.GetAnimationClips(anim.gameObject);
            if (clips.Length != anim.GetClipCount())
            {
                sb.Append(string.Format("{0} Animation组件丢失了动画", anim.name));
                isWarningLog = false;
                break;
            }
        }

        foreach (var anim in animations)
        {
            if (CheckInWhiteList(go, CheckType.LostDefaultClip)) break;
            //AnimationClip[] clips = AnimationUtility.GetAnimationClips(anim.gameObject);
            if (anim.clip == null)
            {
                sb.Append(string.Format("{0} Animation组件丢失了默认动画", anim.name));
                isWarningLog = false;
                break;
            }
        }
        return sb.ToString();
    }

    //-------------------------------------------------------------------------------------------

    #region 用于过滤挂载特定脚本的资源
    private static string GetAnimationBug_CheckDefaultClipWhiteList(object obj)
    {
        GameObject go = obj as GameObject;
        Animation[] animations = go.GetComponentsInChildren<Animation>(true);
        foreach (var anim in animations)
        {
            AnimationClip[] clips = AnimationUtility.GetAnimationClips(anim.gameObject);
            if (clips.Length != anim.GetClipCount())
            {
                return string.Format("{0} Animation组件丢失了动画", anim.name);
            }
        }
        return string.Empty;
    }

    private static bool GetComponentDoAction<T>(GameObject go, StringBuilder sb, Func<object, string> func)
    {
        if (go.GetComponentInChildren<T>() != null)
        {
            sb.Append(func(go));
            return true;
        }
        return false;
    }

    public static void CheckAssetsSize(object obj)
    {
        Texture target = obj as Texture;
        if(target == null) return;
        var type = System.Reflection.Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
        MethodInfo methodInfo = type.GetMethod ("GetStorageMemorySize", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
        int targetSize = (int)methodInfo.Invoke(null,new object[]{target});
        if(targetSize >= maxTexTureSize)
        {
            Debug.LogError (target.name + "硬盘占用：" +EditorUtility.FormatBytes((int)methodInfo.Invoke(null,new object[]{target})) 
            + "大于"+ EditorUtility.FormatBytes(maxTexTureSize)+"请优化");
            //EditorUtility.DisplayDialog
            //Debug.Log(target.name + "硬盘占用："+EditorUtility.FormatBytes((int)methodInfo.Invoke(null,new object[]{target})));
        }
    }

    public static void _CheckAssetsSize(string[] guids)
    {
        EditorUtility.DisplayProgressBar("检查资源大小", "检查资源大小", 0);
         int i = 0;
        foreach (var guid in guids)
        {
            string objDir = AssetDatabase.GUIDToAssetPath(guid);
            EditorUtility.DisplayProgressBar("检查检查资源大小问题", objDir, (float)i / guids.Length);
            var go = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(objDir);
            try
            {
                 CheckAssetsSize(go);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(string.Format("{0} \n {1}", go.name, e));
            }
            i++;
        }
        EditorUtility.ClearProgressBar();
        Debug.LogError("检查完毕! 数量:" + i);
    }

    #endregion

    #region 白名单

    public enum CheckType
    {
        None = 0,
        LostController = 1,//丢失控制器
        LostMotion = 2,//丢失State的动画
        LostClip = 3,//丢失动画
        LostDefaultClip = 4,//丢失默认动画
        LostMaterial = 5,//丢失材质
    }

    private static int EnumCount = 5;

    private static bool CheckInWhiteList(GameObject go, CheckType type, string state = "")
    {
        if (whiteList == null) return false; 
        string path = AssetDatabase.GetAssetPath(go);
        var strs = path.Split('.');
        path = strs[0];
        int index = (int)type - 1;
        if (whiteList.Count <= index) return false;
        foreach(var item in whiteList[index])
        {
            if (path.Contains(item.Key))
            {
                if (type != CheckType.LostMotion || state == "") return true;
                else
                {
                    if (whiteList[index][item.Key] != null && whiteList[index][item.Key].Contains(state)) return true;
                    continue;
                }
            }
        }
        //if (whiteList[index].ContainsKey(path))
        //{
        //    if (type != AnimationCheckType.LostMotion || state == "") return true;
        //    else
        //    {
        //        if (whiteList[index][path] != null && whiteList[index][path].Contains(state)) return true;
        //        return false;
        //    }
        //}
        return false;
    }

    private static List<Dictionary<string, List<string>>> whiteList;
    public static void GetWhiteList()
    {
        whiteList = null;
        whiteList = new List<Dictionary<string, List<string>>>();
        for(int iCount = 0; iCount < EnumCount; iCount++)
        {
            whiteList.Add(new Dictionary<string, List<string>>());
        }
        string pathsPath = @"CheckToolFiles\WhiteList.xlsx";
        DataSet data = ExcelTool.GetData(pathsPath);
        if (data == null) return;
        if (data.Tables == null) return;
        try
        {
            var Rows = data.Tables[0].Rows;

            for (int iCount = 1; iCount < Rows.Count; iCount++)
            {
                bool isWhiteList = Rows[iCount][3].ToString().Trim().Contains("1") ? true : false;
                if (isWhiteList)
                {
                    string path = Rows[iCount][0].ToString().Trim();
                    path = path.Replace(@"\", "/");
                    var strs = path.Split(new string[1] { "Assets/" }, StringSplitOptions.RemoveEmptyEntries);
                    path = "Assets/" + strs[strs.Length - 1];
                    string name = Rows[iCount][1].ToString().Trim();
                    path += "/" + name;
                    path = path.Replace("//", "/");//以防重复添加斜杠

                    var indexStrs = Rows[iCount][2].ToString().Trim().Split(',');
                    for (int jCount = 0; jCount < indexStrs.Length; jCount++)
                    {
                        int index = 0;
                        int.TryParse(indexStrs[jCount], out index);
                        if (index > 0) index -= 1;//枚举类型从1开始，而whiteList从0开始
                        else continue;
                        if (!whiteList[index].ContainsKey(path))
                        {
                            whiteList[index].Add(path, null);
                            //Debug.Log(path + " " + (index + 1));
                        }
                        if (index == (int)CheckType.LostMotion - 1)
                        {
                            if (whiteList[index][path] == null) whiteList[index][path] = new List<string>();
                            var stateStrs = Rows[iCount][4].ToString().Trim().Split(',');
                            foreach (var stateStr in stateStrs)
                            {
                                string state = stateStr.Trim();
                                if (!whiteList[index][path].Contains(state))
                                {
                                    whiteList[index][path].Add(state);
                                }
                                //Debug.Log(stateStr);
                            }
                        }

                    }
                }
            }
        }
        catch
        {
            Debug.LogError("请查看根目录 " + pathsPath + " 是否存在且为最新版本。");
        }
    }

    public static void ClearWhiteList()
    {
        whiteList = null;
    }
    #endregion
}
