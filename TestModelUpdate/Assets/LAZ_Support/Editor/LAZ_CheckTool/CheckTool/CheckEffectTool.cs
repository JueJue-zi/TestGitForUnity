using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System;

public class CheckEffectTool
{
    #region Log
    static List<string> fileLog;
    static string filePath = "工程完整性报告(特效脚本相关).txt";
    static string fileCurPath;
    static void BeginWriteLog()
    {
        //fileLog = new StringBuilder();
        if (fileLog == null) fileLog = new List<string>();
        fileCurPath = string.Format("{0}/../{1}", Application.dataPath, filePath);
    }

    static void WriteLog(string log)
    {
        fileLog.Add(log);
        //fileLog.Append(log);
        //fileLog.Append("\n");
    }

    static void EndWriteLog()
    {
        foreach (var log in fileLog)
        {
            File.WriteAllLines(fileCurPath, fileLog.ToArray());
        }
        fileLog = null;
    }
    #endregion

    public static Dictionary<string, int> pathDic;
    public static Dictionary<string, List<int>> whiteList;
    public static List<string> componentList;
    public static string[] strs;

    //[MenuItem("Laz/Tools/工程正确性检查(特效脚本相关)", false, 22)]
    public static void CheckEffectScript()
    {
        CheckTool.ClearConsole();
        BeginWriteLog();
        GetData();
        if (strs != null)
        {
            foreach (var str in strs)
            {
                _CheckEffectScript(str);
            }
        }
        ClearData();
        EndWriteLog();
        EditorUtility.DisplayDialog("检查完毕", "请通过Window/Console打开输出日志查看详细数据,也可以查看项目根目录的日志", "OK");
    }

    public static void _CheckEffectScript(string path)
    {
        EditorUtility.DisplayProgressBar("检查特效脚本问题", "收集数据中...", 0);
        var strs = AssetDatabase.FindAssets("t:Prefab", new string[] { path });
        int count = 0;
        foreach (var str in strs)
        {
            string objDir = AssetDatabase.GUIDToAssetPath(str);
            EditorUtility.DisplayProgressBar("检查特效脚本问题", objDir, (float)count / strs.Length);
            try
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(objDir);
                string bug = DoCheckEffect(go);
                if (!string.IsNullOrEmpty(bug))
                {
                    Debug.LogError(string.Format("<color=red>[特效脚本]</color>{0} \n{1}", objDir, bug), go);
                    WriteLog(string.Format("[特效脚本]{0} \n{1}", objDir, bug));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
            }
            count++;
        }
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 如要在强制检测中使用，记得先调用GetData()，结束后调用ClearData()
    /// </summary>
    /// <param name="go"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string DoCheckEffect(GameObject go)
    {
        int type = -1;
        type = GetTypeByPath(go);
        if (type < 0)
        {
            return string.Empty;
            //return string.Format("{0} 不在检测范围中\n", go.name);
        }
        if (whiteList.ContainsKey(go.name))
        {
            if (whiteList[go.name].Contains(type))
            {
                return string.Empty;
            }
        }
        //拉杰尔战斗音效脚本,此处待替换

        // SkillEffectAudioCtrl skillEffectAudioCtrl = go.GetComponent<SkillEffectAudioCtrl>();
        //if (skillEffectAudioCtrl == null)
        //{
        //    str.Append(string.Format("{0} 缺少脚本 SkillEffectAudioCtrl\n", go.name));
        //}
        // if (skillEffectAudioCtrl == null || !skillEffectAudioCtrl.enabled)
        // {
        //     return string.Empty;
        // }
        if (!CheckComponents(go))
        {
            return string.Format("{0} 音效回收有问题，请策划检查EffectTimeOut脚本、子弹特效脚本和SkillEffectAudioCtrl脚本\n", go.name);
        }
        return string.Empty;

        //StringBuilder str = new StringBuilder();
        //EffectTimeOut effectTimeOut = go.GetComponent<EffectTimeOut>();
        //if (effectTimeOut == null || !effectTimeOut.enabled)
        //{
        //    str.Append(string.Format("{0} 音效回收有问题，请策划检查EffectTimeOut脚本、子弹特效脚本和SkillEffectAudioCtrl脚本\n", go.name));
        //}
        //return str.ToString();
    }

    private static bool CheckComponents(GameObject go)
    {
        if (componentList == null || componentList.Count == 0) return true;
        foreach(var component in componentList)
        {
            var temp = go.GetComponent(component);
            if (temp != null)
            {
                MonoBehaviour mono = (MonoBehaviour)temp;
                if (mono != null && mono.enabled) return true;
            }
        }
        return false;
        #region old
        //if (go.GetComponent<BattleRemoteAttackAreaColliderEx_TimerRotateBullet>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleRemoteAttackAreaColliderEx_ParallelBullet>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleRemoteAttackAreaColliderEx>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleMeeleAttackAreaColliderEx_TrackBullet>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleRemoteAttackAreaColliderEx_SineCurve>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleMeleeAttackAreaColliderEx_SineCurve_Missile>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleRemoteAttackAreaColliderEx_CustomCurve>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleMeleeAttackAreaColliderEx_CustomCurve_Missile>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleRemoteAttackAreaColliderEx_Curve>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleRemoteAttackAreaColliderEx_Boomerang>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleMeleeAttackAreaColliderEx_Throw>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleMeleeAttackAreaColliderEx_Throw_Missile>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleRemoteAttackAreaColliderEx_DropCtrl>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleMeleeAttackAreaColliderEx_Timer>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleMeleeAttackAreaColliderEx_Trap>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleRemoteAttackAreaColliderEx_Rotate>())
        //{
        //    return false;
        //}
        //if (go.GetComponent<BattleMeleeAttackAreaColliderEx_Trap_Rotate>())
        //{
        //    return false;
        //}
        //return true;
        #endregion
    }

    public static int GetTypeByPath(GameObject go)
    {
        //if (pathDic.ContainsKey(path))
        //{
        //    return pathDic[path];
        //}
        if(pathDic == null) return -1;
        string path = AssetDatabase.GetAssetPath(go);
        foreach (var item in pathDic)
        {
            if (path.Contains(item.Key))
            {
                var strs = path.Split(new string[1] { item.Key }, StringSplitOptions.RemoveEmptyEntries);
                if (strs[strs.Length - 1].ToCharArray()[0] == '/')
                {
                    return item.Value;
                }
            }
        }
        return -1;
    }

    private static void GetEffectPath()
    {
        pathDic = null;
        if (pathDic == null) pathDic = new Dictionary<string, int>();
        strs = null;
        //string pathsPath = @"Assets\Editor\CheckEffectTool\CheckEffectScriptPaths.txt";
        string pathsPath = @"CheckToolFiles\CheckEffectScriptPaths.txt";
        if (File.Exists(pathsPath))
        {
            strs = File.ReadAllLines(pathsPath);
            for (int iCount = 0; iCount < strs.Length; iCount++)
            {
                strs[iCount].Trim();
                strs[iCount] = strs[iCount].Replace(@"\", "/");
                strs[iCount] = strs[iCount].Replace("//", "/");
                var tempStrs = strs[iCount].Split(',');
                strs[iCount] = tempStrs[0];
                int type = int.Parse(tempStrs[1]);
                tempStrs = strs[iCount].Split(new string[1] { "Assets/" }, StringSplitOptions.RemoveEmptyEntries);
                strs[iCount] = "Assets/" + tempStrs[tempStrs.Length - 1];
                pathDic.Add(strs[iCount], type);
            }
        }
        else
        {
            Debug.LogError("不存在 " + pathsPath + " 该配置检查路径的文件");
            strs = new string[3];
            strs[0] = @"Assets\Resources\Prefabs\lPhone\Creatures\EnemyVehicle\BossSkillEffect";
            strs[1] = @"Assets\Resources\Prefabs\lPhone\Creatures\EnemyVehicle\BossSkillEffect_FastEffect";
            strs[2] = @"Assets\Resources\Prefabs\lPhone\Creatures\EnemyVehicle\EnemyBodyEffect";
            pathDic = new Dictionary<string, int>();
            pathDic.Add(strs[0], 0);
            pathDic.Add(strs[1], 1);
            pathDic.Add(strs[2], 2);
        }
    }

    public static void GetData()
    {
        GetEffectPath();
        GetWhiteList();
        GetNoCheckComponent();
    }
    public static void ClearData()
    {
        strs = null;
        pathDic = null;
        whiteList = null;
        componentList = null;
    }
    private static void GetWhiteList()
    {
        whiteList = null;
        if (whiteList == null) whiteList = new Dictionary<string, List<int>>();
        //string pathsPath = @"Assets\Editor\CheckEffectTool\CheckEffectScriptWhiteList.txt";
        string pathsPath = @"CheckToolFiles\CheckEffectScriptWhiteList.txt";
        if (File.Exists(pathsPath))
        {
            var strs = File.ReadAllLines(pathsPath);
            for (int iCount = 0; iCount < strs.Length; iCount++)
            {
                strs[iCount].Trim();
                var tempStrs = strs[iCount].Split(',');
                whiteList.Add(tempStrs[0], new List<int>());
                for (int jCount = 1; jCount < tempStrs.Length; jCount++)
                {
                    if (!whiteList.ContainsKey(tempStrs[0]))
                    {
                        whiteList.Add(tempStrs[0], new List<int>());
                    }
                    whiteList[tempStrs[0]].Add(int.Parse(tempStrs[jCount]));
                    //Debug.Log(tempStrs[0] + ": " + tempStrs[jCount]);
                }
            }
        }
    }
    
    private static void GetNoCheckComponent()
    {
        componentList = null;
        componentList = new List<string>();
        //string pathsPath = @"Assets\Editor\CheckEffectTool\CheckEffectScriptComponents.txt";
        string pathsPath = @"CheckToolFiles\CheckEffectScriptComponents.txt";
        if (File.Exists(pathsPath))
        {
            var strs = File.ReadAllLines(pathsPath);
            for (int iCount = 0; iCount < strs.Length; iCount++)
            {
                strs[iCount].Trim();
                componentList.Add(strs[iCount]);
            }
        }
    }
}
