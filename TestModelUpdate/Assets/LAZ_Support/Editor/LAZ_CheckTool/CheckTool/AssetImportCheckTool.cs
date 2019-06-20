using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Profiling;

public class AssetImportCheckTool : AssetPostprocessor
{
    const string _title = "5q2j5Zyo5qOA5p+l5a+85YWl6aKE5Yi25L2T";
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
#if UsingSyscAB
        CheckAllAssets(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
#endif

        var titleByte = System.Convert.FromBase64String(_title);
        var title = System.Text.Encoding.UTF8.GetString(titleByte);
        int current = 1;
        int total = importedAssets.Length;
        //bool anyFuckingError = false;

        CheckTool.GetWhiteList();

        //待Fgui 特效部分确认后修改检查方式
        //CheckEffectTool.GetData()
        foreach (string str in importedAssets)
        {
            if (str.ToLower().Contains(".fbx")) continue;
            if (str.Contains("Assets/StreamingAssets")) continue;
            EditorUtility.DisplayProgressBar(title, str, (float)current / total);
            var obj = AssetDatabase.LoadAssetAtPath<Object>(str);
            try
            {
                CheckTool.CheckAssetsSize(obj);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(string.Format("{0} \n {1}", obj.name, ex));
            }

            if (obj is GameObject)
            {
                if (obj == null) Debug.LogError("Loading Asset Failed!!!!!!! \n Path: " + str); 
                var go = obj as GameObject;

                NowIdentiyCheck(str,go);
                //anyFuckingError |= (effectListTimeCtrlMissing | checkEffectOpenMissing | nameError);
            }
            current++;
        }
        EditorUtility.ClearProgressBar();
        //foreach (string str in deletedAssets)
        //{
        //    Debug.Log("Deleted Asset: " + str);
        //}

        for (int i = 0; i < movedAssets.Length; i++)
        {
            Debug.LogWarning("从位置: " + movedFromAssetPaths[i] + "\n移动到: " + movedAssets[i]+ " <color=red>[您移动了文件，请确保您没有手滑]</color>");
        }
        //if (anyFuckingError)
        //{
        //    EditorUtility.DisplayDialog("警告", "有资源错误，请检查Console日志！", "确定");
        //}

        CheckEffectTool.ClearData();
        CheckTool.ClearWhiteList();

    }

    #region  检查方法拆分

    //身份区分检查方式
    static void NowIdentiyCheck(string str,GameObject go)
    {
        SelectIdentity.ID_TYPE nowSelectId = SelectIdentity.nowTypeId;
        switch (nowSelectId)
        {
            case SelectIdentity.ID_TYPE.ARTIST:
                TryCheckPrefab(go);//Go对象引用丢失
                TryCheckScript(str,go);//检查丢失脚本
                TryCheckEffect(str,go);//检查丢失特效
                TryCheckMissAnimation(str,go);//检查丢失动画
                TryCheckMatinfo(str,go);//检查丢失材质
                TryCheckTimeEffect(go);//检查时间特效
                TryCheckEffectScript(go);//检查特效脚本
                TryCheckName(str,go);//检查名字
                break;
            case SelectIdentity.ID_TYPE.DESIGNER:
                TryCheckPrefab(go);//Go对象引用丢失
                TryCheckScript(str,go);//检查丢失脚本
                TryCheckEffect(str,go);//检查丢失特效
                TryCheckMissAnimation(str,go);//检查丢失动画
                TryCheckMatinfo(str,go);//检查丢失材质
                TryCheckTimeEffect(go);//检查时间特效
                TryCheckEffectScript(go);//检查特效脚本
                TryCheckName(str,go);//检查名字
                break;
            case SelectIdentity.ID_TYPE.PRORGAMMER:
                TryCheckPrefab(go);//Go对象引用丢失
                TryCheckScript(str,go);//检查丢失脚本
                TryCheckEffect(str,go);//检查丢失特效
                TryCheckMissAnimation(str,go);//检查丢失动画
                TryCheckName(str,go);//检查名字
                break;
            case SelectIdentity.ID_TYPE.QA:
                TryCheckPrefab(go);//Go对象引用丢失
                TryCheckScript(str,go);//检查丢失脚本
                TryCheckEffect(str,go);//检查丢失特效
                TryCheckMissAnimation(str,go);//检查丢失动画
                TryCheckMatinfo(str,go);//检查丢失材质
                TryCheckTimeEffect(go);//检查时间特效
                TryCheckEffectScript(go);//检查特效脚本
                TryCheckName(str,go);//检查名字
                break;
            case SelectIdentity.ID_TYPE.TA:
                TryCheckPrefab(go);//Go对象引用丢失
                TryCheckScript(str,go);//检查丢失脚本
                TryCheckEffect(str,go);//检查丢失特效
                TryCheckMissAnimation(str,go);//检查丢失动画
                TryCheckMatinfo(str,go);//检查丢失材质
                TryCheckTimeEffect(go);//检查时间特效
                TryCheckEffectScript(go);//检查特效脚本
                TryCheckName(str,go);//检查名字
                break;
            default:
                TryCheckPrefab(go);//Go对象引用丢失
                TryCheckScript(str,go);//检查丢失脚本
                TryCheckEffect(str,go);//检查丢失特效
                TryCheckMissAnimation(str,go);//检查丢失动画
                TryCheckMatinfo(str,go);//检查丢失材质
                TryCheckTimeEffect(go);//检查时间特效
                TryCheckEffectScript(go);//检查特效脚本
                TryCheckName(str,go);//检查名字
                break;
        }
    }
    static void TryCheckName(string str,GameObject go)
    {
        try
        {
            _CheckPrefabNameNotMatchObjectName(str, go);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    static void TryCheckEffectScript(GameObject go)
    {
        try
        {
             CheckTool._CheckEffectOpenMissing(go);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    static void TryCheckTimeEffect(GameObject go)
    {
        try
        {
            CheckTool._CheckEffectListTimeCtrlMissing(go);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    static void TryCheckMatinfo(string str,GameObject go)
    {
        try
        {
            var missingMatInfo = CheckTool.GetMatMissingInfo(go);
            if (!string.IsNullOrEmpty(missingMatInfo))
            {
                Debug.LogError(string.Format("<color=red>[材质丢失]</color> {0}\n{1}", str, missingMatInfo), go);
            }

        }
                    catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
    }
    static void TryCheckMissAnimation(string str,GameObject go)
    {
        try
        {
            var missingAnimation = CheckTool.GetAnimationBug(go);
            if (!string.IsNullOrEmpty(missingAnimation))
            {
                if (!CheckTool.isWarningLog)
                {
                    Debug.LogError(string.Format("<color=red>[动画组件]</color> {0}\n{1}", str, missingAnimation), go);
                }
                else
                {
                    Debug.LogWarning(string.Format("<color=red>[动画组件]</color> {0}\n{1}", str, missingAnimation), go);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
        }
    }
    static void TryCheckPrefab(GameObject go)
    {
        try
        {
            CheckTool._CheckGoMissingReferences(go);
        }
        catch (System.Exception ex)
        {
             Debug.LogError(ex);
        }
    }

    static void TryCheckEffect(string str,GameObject go)
    {
        try
        {
            var effectError = CheckEffectTool.DoCheckEffect(go);
            if (!string.IsNullOrEmpty(effectError))
            {
                Debug.LogError(string.Format("<color=red>[特效脚本]</color>{0} \n{1}", str, effectError), go);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    static void TryCheckScript(string str,GameObject go)
    {
        try
        {
            var missingScripts = CheckTool.doCheckScriptMissing(go);
            if (!string.IsNullOrEmpty(missingScripts))
            {
                 Debug.LogError(string.Format("<color=red>[脚本丢失]</color>{0} \n{1}", str, missingScripts), go);
            }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
    }

    static bool _CheckPrefabNameNotMatchObjectName(string path, Object obj)
    {
        if (!path.Contains(obj.name))
        {
            Debug.LogError(string.Format("<color=red>[名字不统一]</color> {0}\n<color=red>{2}</color>  <color=brown>{1}</color>", 
                path, obj.name,System.IO.Path.GetFileNameWithoutExtension(path)), obj);
            return true;
        }
        return false;
    }

    #endregion

#if UsingSyscAB
    const string AssetFolderPathLower = "assets/resources/";
    static void CheckAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        List<string> vimportedAssets = new List<string>(importedAssets);
        List<string> vdeletedAssets = new List<string>(deletedAssets);
        List<string> vmovedAssets = new List<string>(movedAssets);
        List<string> vmovedFromAssetPaths = new List<string>(movedFromAssetPaths);


        ForRun(vimportedAssets);
        ForRun(vdeletedAssets);
        ForRun(vmovedAssets);
        ForRun(vmovedFromAssetPaths);
    }

    static bool IsInResource(string vPath)
    {
        //Check in Resources
        var tempData = vPath;
        tempData = tempData.ToLower();
        string[] vListData = tempData.Split('/');
        if (vListData.Length <= 1) return false;
        if (vListData.Length >= 2)
            if (vListData[1] == "resources") return true;

        return false;
    }

    static void SetIntoList(string[] vStrData, ref List<string> vLIstData)
    {
        for (int iCount = 0; iCount < vStrData.Length; iCount++)
        {
            vLIstData.Add(vStrData[iCount]);
        }
    }

    static string RemoveABOldData(string assetPath)
    {
        var filePath = assetPath.ToLower().Replace('\\', '/');
        var ext = Path.GetExtension(filePath);

        if (ext.IndexOf(".unity") < 0)
        {
            assetPath = GetHashCodeAssetbundleName(filePath);
        }
        else
        {
            filePath = filePath.Replace(AssetFolderPathLower, System.String.Empty);
            assetPath = filePath.Substring(0, filePath.Length - ext.Length);
        }

        Debug.LogError("assetPath:::::" + assetPath + "   filePath:" + filePath);
        return assetPath;
    }


    private static string AdvanceAllocation(string vPath)
    {
        var vFullPath = Path.GetDirectoryName(vPath);
        uint hashCode = (uint)vFullPath.GetHashCode();
        var vReturndata = hashCode.ToString("x2");
        return vReturndata;
    }

    static string GetHashCodeAssetbundleName(string resPath)
    {
        resPath = resPath.Replace("\\", "/");
        return AdvanceAllocation(resPath);
    }

    static void ForRun(List<string> vTmpStringData)
    {
        for (int iCount = 0; iCount < vTmpStringData.Count; iCount++)
        {
            if (IsInResource(vTmpStringData[iCount]))
                RemoveChangeABData(RemoveABOldData(vTmpStringData[iCount]));
        }
    }


    static void RemoveChangeABData(string vABData)
    {
        //Combine String
        var vItemPath = Application.dataPath + "/StreamingAssets/AB/AssetBundles/android/" + vABData;
        Debug.LogError("vItemPath:::::" + vItemPath);
        var vItemPathManifest = vItemPath + ".manifest";

        File.Delete(vItemPath);
        File.Delete(vItemPathManifest);
        AssetDatabase.Refresh();

    }
#endif
}
