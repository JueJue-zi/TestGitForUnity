using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FbxSimplify
{
    public static void CheckVertexColor(string[] prefabs)
    {
        EditorUtility.DisplayProgressBar("顶点色", "收集模型数据", 0f);
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        //ist<Color> newColors = null;
        int i = 0;
        int checkNum = 0;
        foreach (var obj in prefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(obj);
            EditorUtility.DisplayProgressBar("模型顶点色检查", path, i / (float)objs.Length);
            i++;
            if (!path.ToLower().EndsWith(".fbx"))
            {
                continue;
            }
            checkNum++;
            Object[] subObjs = AssetDatabase.LoadAllAssetsAtPath(path);
            try
            {
                foreach (var o in subObjs)
                {
                    if (o is MeshFilter)
                    {
                        MeshFilter m = o as MeshFilter;
                        if (m.sharedMesh.colors != null && m.sharedMesh.colors.Length > 0)
                        {
                            Debug.LogError(string.Format("{0} {1} 含有顶点色", path, o.name));
                        }
                    }
                    if(o is SkinnedMeshRenderer)
                    {
                        SkinnedMeshRenderer m = o as SkinnedMeshRenderer;
                        if (m.sharedMesh.colors != null && m.sharedMesh.colors.Length > 0)
                        {
                            Debug.LogError(string.Format("{0} {1} 含有顶点色", path, o.name));
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                EditorUtility.ClearProgressBar();
                return;
            }

        }
        Debug.LogError("检查完毕! 数量:" + checkNum);
        if(checkNum == 0) EditorUtility.DisplayDialog ("提醒", "请检查Project视图中选中的目录或文件是否是含有模型!", "OK");
        EditorUtility.ClearProgressBar();
    }

    public static void CheckMultiUVs(string[] prefabs)
    {
        EditorUtility.DisplayProgressBar("UV检查", "收集模型数据", 0f);
        //List<Color> newColors = null;
        int i = 0;
        int checkNum = 0;
        foreach (var obj in prefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(obj);
            EditorUtility.DisplayProgressBar("UV检查", path, i / (float)prefabs.Length);
            i++;
            if (!path.ToLower().EndsWith(".fbx"))
                continue;
            checkNum++;
            Object[] subObjs = AssetDatabase.LoadAllAssetsAtPath(path);
            try
            {
                foreach (var o in subObjs)
                {
                    if (o is MeshFilter)
                    {
                        MeshFilter m = o as MeshFilter;
                        if(m.sharedMesh.uv3 != null && m.sharedMesh.uv3.Length > 0)
                        {
                            Debug.LogError("MultiUV UV3 " + m.name + " " + m.sharedMesh.uv3.Length + path);
                            continue;
                        }
                        if(m.sharedMesh.uv4 != null && m.sharedMesh.uv4.Length > 0)
                        {
                            Debug.LogError("MultiUV UV4 " + m.name + " " + m.sharedMesh.uv4.Length + path);
                            continue;
                        }
                    }
                    if(o is SkinnedMeshRenderer)
                    {
                        SkinnedMeshRenderer m = o as SkinnedMeshRenderer;
                        if (m == null)
                            continue;

                       if(m.sharedMesh.uv2 != null && m.sharedMesh.uv2.Length > 0)
                        {
                            Debug.LogError("MultiUV UV2 " + m.name + " " + m.sharedMesh.uv3.Length + path);
                            continue;
                        }

                        if(m.sharedMesh.uv3 != null && m.sharedMesh.uv3.Length > 0)
                        {
                            Debug.LogError("MultiUV UV3 " + m.name + " " + m.sharedMesh.uv3.Length + path);
                            continue;
                        }
                        if(m.sharedMesh.uv4 != null && m.sharedMesh.uv4.Length > 0)
                        {
                            Debug.LogError("MultiUV UV4 " + m.name + " " + m.sharedMesh.uv4.Length + path);
                            continue;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                EditorUtility.ClearProgressBar();
                return;
            }

        }
        Debug.LogError("检查完毕! 数量:" + checkNum);
        if(checkNum == 0) EditorUtility.DisplayDialog ("提醒", "请检查Project视图中选中的目录或文件是否是含有模型!", "OK");
        EditorUtility.ClearProgressBar();
    }
    public static void RemoveNormalAndTangent(string[] prefabs)
    {
        EditorUtility.DisplayProgressBar("模型优化", "收集模型数据", 0f);
        int i = 0;
        int checkNum = 0;
        foreach (var obj in prefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(obj);
            EditorUtility.DisplayProgressBar("模型优化", path, i / (float)prefabs.Length);
            if (!path.ToLower().EndsWith(".fbx"))
                continue;

            checkNum++;
            try
            {
                ModelImporter mi = AssetImporter.GetAtPath(path) as ModelImporter;
                mi.importTangents = ModelImporterTangents.None;
                mi.importNormals = ModelImporterNormals.None;
                mi.importMaterials = false;
                AssetDatabase.ImportAsset(path);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                EditorUtility.ClearProgressBar();
                return;
            }
        }
        Debug.LogError("移除完毕! 数量:" + checkNum);
        if(checkNum == 0) EditorUtility.DisplayDialog ("提醒", "请检查Project视图中选中的目录或文件是否是含有模型!", "OK");
        EditorUtility.ClearProgressBar();
    }

    public static void RemoveTangents(string[] prefabs)
    {
        EditorUtility.DisplayProgressBar("模型优化", "收集模型数据", 0f);
        int i = 0;
        int checkNum = 0;
        foreach (var obj in prefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(obj);
            EditorUtility.DisplayProgressBar("模型优化", path, i / (float)prefabs.Length);
            i++;
            if (!path.ToLower().EndsWith(".fbx"))
                continue;
            
            checkNum++;
            try
            {
                ModelImporter mi = AssetImporter.GetAtPath(path) as ModelImporter;
                if(mi.importTangents != ModelImporterTangents.None | mi.isReadable)
                {
                    // mi.importTangents = ModelImporterTangents.None;
                    mi.isReadable = false;
                    AssetDatabase.ImportAsset(path);
                }
                //mi.importMaterials = false;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                EditorUtility.ClearProgressBar();
                return;
            }
        }
        Debug.LogError("禁用完毕! 数量:" + checkNum);
        if(checkNum == 0) EditorUtility.DisplayDialog ("提醒", "请检查Project视图中选中的目录或文件是否是含有模型!", "OK");
        EditorUtility.ClearProgressBar();
    }
}
