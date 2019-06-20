using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using UnityEditor.TreeViewExamples;
using UnityEditor.IMGUI.Controls;

namespace Indra.TextureTool
{
    enum AndroidFormat
    {
        RGB16 = 7,
        RGB24 = 3,
        Alpha8 = 1,
        RGBA32 = 4,
        RGBA16 = 13,
        ETC_RGB4 = 34,
        ETC2_RGB4 = 45,
        ETC2_RGBA8 = 47,
    }

    enum IosFormat
    {
        RGB16 = 7,
        RGB24 = 3,
        Alpha8 = 1,
        RGBA32 = 4,
        RGBA16 = 13,
        PVRTC_RGB4 = 32,
        PVRTC_RGBA4 = 33,
        //---new ASTC FORMAT
        ASTC_RGB_4x4 = 48,
        ASTC_RGB_6x6 = 50,
        ASTC_RGBA_4x4 = 54,
        ASTC_RGBA_6x6 = 56,
    }

    enum CompressQuality
    {
        NORMAL = 50,
        BEST = 100,
    }

    enum TexSize
    {
        T_32 = 32,
        T_64 = 64,
        T_128 = 128,
        T_256 = 256,
        T_512 = 512,
        T_1024 = 1024,
        T_2048 = 2048,
    }
    #region MODEL
    class TexData : TreeElement
    {
        public Texture texture;
        public TextureImporter ti;
        public TextureImporterPlatformSettings android;
        public TextureImporterPlatformSettings ios;
        public bool bModify;
        public bool mipmap;
        public bool rw;
        public Color color;

        public TexData(string name, int depth, int id) : base(name, depth, id)
        {

        }
    }
    #endregion

    #region VIEW
    class TexInfoTreeView : TreeViewWithTreeModel<TexData>
    {
        static class Styles
        {
            public static GUIStyle background = "RL Background";
            public static GUIStyle headerBackground = "RL Header";
        }

        public TexInfoTreeView(TreeViewState state, TreeModel<TexData> model) : base(state, model)
        {
            showBorder = true;
            customFoldoutYOffset = 3f;

            Reload();
        }

        public override void OnGUI(Rect rect)
        {
            // Background
            if (Event.current.type == EventType.repaint)
                DefaultStyles.backgroundOdd.Draw(rect, false, false, false, false);

            // TreeView
            base.OnGUI(rect);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TreeViewItem<TexData>)args.item;
            var contentIndent = GetContentIndent(item);

            var bgRect = args.rowRect;
            bgRect.x = contentIndent;
            bgRect.width = Mathf.Max(bgRect.width - contentIndent, 155f) - 5f;
            bgRect.yMin += 2f;
            bgRect.yMax -= 2f;
            DrawItemBackground(bgRect);

            var headerRect = bgRect;
            headerRect.xMin += 5f;
            headerRect.xMax -= 10f;
            headerRect.height = Styles.headerBackground.fixedHeight;
            HeaderGUI(headerRect, args.label, item);

            var controlsRect = headerRect;
            controlsRect.xMin += 20f;
            controlsRect.y += headerRect.height;
            ControlsGUI(controlsRect, item);

            Event evt = Event.current;
            Rect toggleRect = args.rowRect;
            toggleRect.x += GetContentIndent(args.item);
            //toggleRect.width = 400f;

            if (evt.type == EventType.MouseDown && toggleRect.Contains(evt.mousePosition))
                Selection.activeObject = AssetDatabase.LoadAssetAtPath(item.data.ti.assetPath, typeof(Texture));
        }

        void DrawItemBackground(Rect bgRect)
        {
            if (Event.current.type == EventType.repaint)
            {
                var rect = bgRect;
                rect.height = Styles.headerBackground.fixedHeight;
                Styles.headerBackground.Draw(rect, false, false, false, false);

                rect.y += rect.height;
                rect.height = bgRect.height - rect.height;
                Styles.background.Draw(rect, false, false, false, false);
            }
        }

        void HeaderGUI(Rect headerRect, string lable, TreeViewItem<TexData> item)
        {
            headerRect.y += 1;
            Rect toggleRect = headerRect;
            toggleRect.width = 16;
            item.data.bModify = EditorGUI.Toggle(toggleRect, item.data.bModify);
            Rect labelRect = headerRect;
            labelRect.xMin = toggleRect.width + 30f;
            Color color = GUI.backgroundColor;
            GUI.backgroundColor = item.data.color;
            if (GUI.Button(labelRect, item.data.ti.assetPath))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath(item.data.ti.assetPath, typeof(Texture));
            }
            GUI.backgroundColor = color;
        }

        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            return 100f;
        }

        void ControlsGUI(Rect controlsRect, TreeViewItem<TexData> item)
        {
            var rect = controlsRect;
            float xmin = rect.xMin;
            rect.y += 3f;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = 50f;
            GUI.Label(rect, "MipMap");
            rect.xMin += 50f;
            rect.width = 16f;
            item.data.mipmap = EditorGUI.Toggle(rect, item.data.mipmap);
            rect.xMin += 30f;
            rect.width = 120f;
            GUI.Label(rect, "Read/Write Enabled");
            rect.xMin += 120f;
            rect.width = 16;
            item.data.rw = EditorGUI.Toggle(rect, item.data.rw);
            rect.xMin += 120;
            rect.width = 120;
            GUI.Label(rect, string.Format("{0}x{1}", item.data.texture.width, item.data.texture.height));

            rect.xMin = xmin;

            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            GUI.Label(rect, "Android:");
            rect.xMin += 70f;
            rect.width = 80f;
            TexSize ts = (TexSize)EditorGUI.EnumPopup(rect, (TexSize)item.data.android.maxTextureSize);
            item.data.android.maxTextureSize = (int)ts;
            rect.xMin += (80f + 20f);
            rect.width = 90f;
            item.data.android.format = (TextureImporterFormat)EditorGUI.EnumPopup(rect, (AndroidFormat)item.data.android.format);
            rect.xMin += (90f + 20f);
            rect.width = 80f;
            CompressQuality cq = (CompressQuality)EditorGUI.EnumPopup(rect, (CompressQuality)item.data.android.compressionQuality);
            item.data.android.compressionQuality = (int)cq;

            rect.xMin = xmin;
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            GUI.Label(rect, "IOS:");
            rect.xMin += 70f;
            rect.width = 80f;
            TexSize ts1 = (TexSize)EditorGUI.EnumPopup(rect, (TexSize)item.data.ios.maxTextureSize);
            item.data.ios.maxTextureSize = (int)ts1;
            rect.xMin += (80f + 20f);
            rect.width = 90f;
            item.data.ios.format = (TextureImporterFormat)EditorGUI.EnumPopup(rect, (IosFormat)item.data.ios.format);
            rect.xMin += (90f + 20f);
            rect.width = 80f;
            CompressQuality cq1 = (CompressQuality)EditorGUI.EnumPopup(rect, (CompressQuality)item.data.ios.compressionQuality);
            item.data.ios.compressionQuality = (int)cq1;
        }

    }
    #endregion


    public class TextureListWindow : EditorWindow
    {
        public static TextureListWindow OpenTexWindow()
        {
            var window = GetWindow<TextureListWindow>();
            window.titleContent = new GUIContent("纹理转换");
            window.Focus();
            return window;
        }
        #region 定义

        #endregion

        private List<TexData> _config;
        private List<TexData> _filterConfig;
        private List<string> _filterTexs;
        private string texDir; //纹理的目录
        private string matDir; //用材质球过滤不需要改动的纹理
        private Vector2 scrollPos;
        private bool bMipMap, bRw;
        private AndroidFormat antFmt;
        private IosFormat iosFmt;
        private CompressQuality antQua, iosQua;
        private bool bModifyAndroid = true, bModifyIOS = true;
        private bool bTexOkFilter;

        //尺寸过滤器
        private bool size64, size128, size256, size512, size1024, sizeother;

        [SerializeField] TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading
        TexInfoTreeView m_TreeView;

        void Init()
        {
            _config = new List<TexData>();
            _filterConfig = new List<TexData>();
            _filterTexs = new List<string>();
            texDir = "";
            matDir = "";
            TexData td = new TexData("ROOT", -1, 0);
            _config.Add(td);
            bTexOkFilter = true;

            size64 = size128 = size256 = size512 = size1024 = sizeother = true;

            //multi operation
            bMipMap = bRw = false;
            //antFmt = AndroidFormat.ETC2_RGBA8;
            //iosFmt = IosFormat.PVRTC_RGBA4;

            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();

            var treeModel = new TreeModel<TexData>(_config);
            m_TreeView = new TexInfoTreeView(m_TreeViewState, treeModel);
        }

        [Obsolete]
        private void OnGUI()
        {
            if (_config == null)
                Init();
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            bTexOkFilter = GUILayout.Toggle(bTexOkFilter, "过滤安卓ETC2和IOS PVRTC格式的纹理");

            size64 = GUILayout.Toggle(size64, "显示64x64");
            size128 = GUILayout.Toggle(size128, "显示128x128");
            size256 = GUILayout.Toggle(size256, "显示256x256");
            size512 = GUILayout.Toggle(size512, "显示512x512");
            size1024 = GUILayout.Toggle(size1024, "显示1024x1024");
            sizeother = GUILayout.Toggle(sizeother, "显示其它尺寸");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.Label(GUILayoutUtility.GetRect(0, 20f, GUILayout.Width(100)), "纹理目录:");
            Rect texDirRect = GUILayoutUtility.GetRect(0, 22f, GUILayout.ExpandWidth(true));
            texDir = GUI.TextField(texDirRect, texDir);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUI.Label(GUILayoutUtility.GetRect(0, 20f, GUILayout.Width(100)), "材质球过滤目录:");
            Rect matDirRect = GUILayoutUtility.GetRect(0, 22f, GUILayout.ExpandWidth(true));
            matDir = GUI.TextField(matDirRect, matDir);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("列举纹理"))
            {
                GetTexList();
                //AppendFilterToTail();
                Debug.Log(_config.Count);
                m_TreeView.treeModel.SetData(_config);
                m_TreeView.Reload();
            }
            if (GUILayout.Button("转换"))
            {
                DoConvert();
                //ConvertDebug();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            DrawMultiOpContent();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.BeginHorizontal();
            bModifyAndroid = GUILayout.Toggle(bModifyAndroid, "修改Android", GUILayout.Width(100));
            bModifyIOS = GUILayout.Toggle(bModifyIOS, "修改IOS", GUILayout.Width(80));
            GUILayout.Box("!勾选才会修改，不勾选那么对应平台的设置不会生效");
            GUILayout.EndHorizontal();

            Rect tv = new Rect(10, 190, position.width - 20, position.height - 200);
            m_TreeView.OnGUI(tv);
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (texDirRect.Contains(evt.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            foreach (var item in DragAndDrop.objectReferences)
                            {
                                texDir = AssetDatabase.GetAssetPath(item);
                                break;
                            }
                        }
                    }
                    if (matDirRect.Contains(evt.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            foreach (var item in DragAndDrop.objectReferences)
                            {
                                matDir = AssetDatabase.GetAssetPath(item);
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        [Obsolete]
        private void DoConvert()
        {
            int convertCount = 0;
            foreach (var item in _config)
            {
                if (item.bModify && item.texture != null)
                    convertCount++;
            }
            int ok = 0;
            for (int i = 0; i < _config.Count; i++)
            {
                TexData td = _config[i];
                if (td.bModify && td.texture != null)
                {
                    EditorUtility.DisplayProgressBar("纹理转换", td.ti.assetPath, ok++ / (float)convertCount);
                    td.bModify = false;
                    td.color = Color.gray;
                    td.ti.mipmapEnabled = td.mipmap;
                    td.ti.isReadable = td.rw;
                    AndroidFormat androidFmt = (AndroidFormat)td.android.format;
                    IosFormat iosFmt = (IosFormat)td.ios.format;
                    //智能格式转换 - Normal不要alpha unity说没alpha 就不设置alpha 
                    if (td.ti.textureType == TextureImporterType.NormalMap | !TextureHasAlpha(td.ti))
                    {
                        if (androidFmt == AndroidFormat.RGBA32)
                            androidFmt = AndroidFormat.RGB24;
                        else if (androidFmt == AndroidFormat.RGBA16)
                            androidFmt = AndroidFormat.RGB16;
                        else if (androidFmt == AndroidFormat.ETC2_RGBA8)
                            androidFmt = AndroidFormat.ETC2_RGB4;
                        if (iosFmt == IosFormat.RGBA32)
                            iosFmt = IosFormat.RGB24;
                        else if (iosFmt == IosFormat.RGBA16)
                            iosFmt = IosFormat.RGB16;
                        else if (iosFmt == IosFormat.PVRTC_RGBA4)
                            iosFmt = IosFormat.PVRTC_RGB4;
                        else if (iosFmt == IosFormat.ASTC_RGBA_4x4)
                            iosFmt = IosFormat.ASTC_RGB_4x4;
                        else if (iosFmt == IosFormat.ASTC_RGBA_6x6)
                            iosFmt = IosFormat.ASTC_RGB_6x6;
                    }
                    if(bModifyAndroid)
                        td.ti.SetPlatformTextureSettings("Android", td.android.maxTextureSize, (TextureImporterFormat)androidFmt, td.android.compressionQuality, false);
                    if(bModifyIOS)
                        td.ti.SetPlatformTextureSettings("iPhone", td.ios.maxTextureSize, (TextureImporterFormat)iosFmt, td.ios.compressionQuality, false);
                    AssetDatabase.ImportAsset(td.ti.assetPath);
                    EditorUtility.ClearProgressBar();
                }
            }
        }
        private bool TextureHasAlpha(TextureImporter ti)
        {
            string path = ti.assetPath;
            TextureFormat tf = TextureFormat.RGB24;
            if (ti.textureShape == TextureImporterShape.TextureCube)
            {
                Cubemap cm = AssetDatabase.LoadAssetAtPath<Cubemap>(path);
                if (cm != null)
                {
                    tf = cm.format;
                }
            }
            else
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex != null)
                {
                    tf = tex.format;
                }
            }
            return IsFormatWithAlpha(tf);
        }

        private bool IsFormatWithAlpha(TextureFormat tm)
        {
            bool alpha = false;
            switch (tm)
            {
                case TextureFormat.ARGB4444:
                case TextureFormat.RGBA32:
                case TextureFormat.ARGB32:
                case TextureFormat.RGBA4444:
                case TextureFormat.BGRA32:
                case TextureFormat.RGBAHalf:
                case TextureFormat.RGBAFloat:
                case TextureFormat.DXT5Crunched:
                case TextureFormat.PVRTC_RGBA2:
                case TextureFormat.PVRTC_RGBA4:
                case TextureFormat.ATC_RGBA8:
                case TextureFormat.ETC2_RGBA1:
                case TextureFormat.ETC2_RGBA8:
                case TextureFormat.ASTC_RGBA_4x4:
                case TextureFormat.ASTC_RGBA_5x5:
                case TextureFormat.ASTC_RGBA_6x6:
                case TextureFormat.ASTC_RGBA_8x8:
                case TextureFormat.ASTC_RGBA_10x10:
                case TextureFormat.ASTC_RGBA_12x12:
                    alpha = true;
                    break;

            }
            return alpha;
        }
        #region 调试
        private void ConvertDebug()
        {
            for (int i = 0; i < _config.Count; i++)
            {
                TexData td = _config[i];
                if (td.bModify)
                {
                    td.bModify = false;
                    td.color = Color.gray;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(td.mipmap);
                    sb.Append(" ");
                    sb.Append(td.rw);
                    sb.Append(" ");
                    sb.Append("Android:");
                    sb.Append(" ");
                    sb.Append(td.android.maxTextureSize);
                    sb.Append(" ");
                    sb.Append(td.android.format);
                    sb.Append(" ");
                    sb.Append(td.android.compressionQuality);
                    sb.Append(" ");
                    sb.Append("Ios:");
                    sb.Append(" ");
                    sb.Append(td.ios.maxTextureSize);
                    sb.Append(" ");
                    sb.Append(td.ios.format);
                    sb.Append(" ");
                    sb.Append(td.ios.compressionQuality);
                    sb.Append(" ");
                    Debug.Log(sb.ToString());
                }
            }
        }
        #endregion

        [Obsolete]
        void DrawMultiOpContent()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            bMipMap = GUILayout.Toggle(bMipMap, "MipMap", GUILayout.Width(80));
            bRw = GUILayout.Toggle(bRw, "R/W Enabled", GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ANDROID:", GUILayout.Width(70));
            antFmt = (AndroidFormat)EditorGUILayout.EnumPopup(antFmt, GUILayout.Width(90));
            antQua = (CompressQuality)EditorGUILayout.EnumPopup(antQua, GUILayout.Width(90));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("IOS:", GUILayout.Width(70));
            iosFmt = (IosFormat)EditorGUILayout.EnumPopup(iosFmt, GUILayout.Width(90));
            iosQua = (CompressQuality)EditorGUILayout.EnumPopup(iosQua, GUILayout.Width(90));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            if (GUILayout.Button("全选", GUILayout.Width(200), GUILayout.Height(50)))
            {

                foreach (var item in _config)
                {
                    item.bModify = true;
                }
            }
            if (GUILayout.Button("批量转换选择纹理", GUILayout.Width(200), GUILayout.Height(50)))
            {
                if ((int)antFmt == 0 | (int)antQua == 0 | (int)iosFmt == 0 | (int)iosQua == 0)
                {
                    EditorUtility.DisplayDialog("警告", "没有设置批量转换的目标格式", "OK");
                }
                else
                {
                    foreach (var item in _config)
                    {
                        if (item.bModify && item.texture != null)
                        {
                            item.mipmap = bMipMap;
                            item.rw = bRw;
                            item.android.format = (TextureImporterFormat)antFmt;
                            item.android.compressionQuality = (int)antQua;
                            item.ios.format = (TextureImporterFormat)iosFmt;
                            item.ios.compressionQuality = (int)iosQua;
                        }
                    }
                    DoConvert();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        void DrawItem(List<TexData> texDatas, Color itemBg)
        {
            for (int i = 0; i < texDatas.Count; i++)
            {
                GUILayout.BeginHorizontal();
                Color oldc = GUI.backgroundColor;
                TexData td = texDatas[i];
                td.bModify = GUILayout.Toggle(td.bModify, " ", GUILayout.Width(20));
                td.mipmap = GUILayout.Toggle(td.mipmap, "MipMap", GUILayout.Width(80));
                td.rw = GUILayout.Toggle(td.rw, "R/W Enabled", GUILayout.Width(100));
                GUILayout.Label(string.Format("{0}x{1}", td.texture.width, td.texture.height), GUILayout.Width(70));

                GUILayout.Label("ANDROID:", GUILayout.Width(70));
                TexSize ts = (TexSize)EditorGUILayout.EnumPopup((TexSize)td.android.maxTextureSize, GUILayout.Width(80));
                td.android.maxTextureSize = (int)ts;
                td.android.format = (TextureImporterFormat)EditorGUILayout.EnumPopup((AndroidFormat)td.android.format, GUILayout.Width(90));
                CompressQuality cq = (CompressQuality)EditorGUILayout.EnumPopup((CompressQuality)td.android.compressionQuality, GUILayout.Width(60));
                td.android.compressionQuality = (int)cq;

                GUILayout.Label("IOS:", GUILayout.Width(30));
                TexSize ts1 = (TexSize)EditorGUILayout.EnumPopup((TexSize)td.ios.maxTextureSize, GUILayout.Width(80));
                td.ios.maxTextureSize = (int)ts1;
                td.ios.format = (TextureImporterFormat)EditorGUILayout.EnumPopup((IosFormat)td.ios.format, GUILayout.Width(90));
                CompressQuality cq1 = (CompressQuality)EditorGUILayout.EnumPopup((CompressQuality)td.ios.compressionQuality, GUILayout.Width(60));
                td.ios.compressionQuality = (int)cq1;


                var align = GUI.skin.button.alignment;
                GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                GUI.backgroundColor = itemBg;
                if (GUILayout.Button(td.ti.assetPath))
                {
                    Selection.activeObject = td.texture;
                }
                GUI.backgroundColor = oldc;
                GUI.skin.button.alignment = align;
                GUILayout.EndHorizontal();
            }
        }

        void AppendFilterToTail()
        {
            int existsCount = _config.Count;
            foreach (var item in _filterConfig)
            {
                item.id += existsCount;
            }
            _config.AddRange(_filterConfig);
        }

        //获取除cubemap以外的所有纹理
        //默认过滤安卓etc2 和ios pvrtc的贴图
        void GetTexList()
        {
            _config.Clear();
            _filterConfig.Clear();
            _filterTexs.Clear();
            TexData _td_temp = new TexData("ROOT", -1, 0);
            _config.Add(_td_temp);
            if (!string.IsNullOrEmpty(matDir))
            {
                string[] ids = AssetDatabase.FindAssets("t:Material", new string[] { matDir });
                int index = 0;
                foreach (var id in ids)
                {
                    string matPath = AssetDatabase.GUIDToAssetPath(id);
                    Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
                    int propCount = ShaderUtil.GetPropertyCount(mat.shader);
                    for (int i = 0; i < propCount; i++)
                    {
                        if (ShaderUtil.GetPropertyType(mat.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                        {
                            string propertyName = ShaderUtil.GetPropertyName(mat.shader, i);
                            Texture tex = mat.GetTexture(propertyName);
                            if (tex == null)
                                continue;
                            string _tex = AssetDatabase.GetAssetPath(tex.GetInstanceID());
                            string ext = System.IO.Path.GetExtension(_tex).ToLower();
                            if (ext.Equals(".cubemap"))
                                continue;

                            TexData td;
                            td = new TexData("ROOT", 0, index++);
                            td.bModify = false;
                            td.texture = tex;
                            td.ti = AssetImporter.GetAtPath(_tex) as TextureImporter;
                            td.android = td.ti.GetPlatformTextureSettings("Android");
                            td.ios = td.ti.GetPlatformTextureSettings("iPhone");
                            td.mipmap = td.ti.mipmapEnabled;
                            td.rw = td.ti.isReadable;
                            td.color = Color.red;
                            _filterConfig.Add(td);
                            _filterTexs.Add(_tex);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(texDir))
            {
                string[] ids = AssetDatabase.FindAssets("t:Texture", new string[] { texDir });
                int index = 1;
                foreach (var id in ids)
                {
                    string texPath = AssetDatabase.GUIDToAssetPath(id);
                    if (_filterTexs.Contains(texPath))
                        continue;
                    string ext = System.IO.Path.GetExtension(texPath).ToLower();
                    if (ext.Equals(".cubemap") | ext.Equals(".rendertexture"))
                        continue;
                    TexData td;
                    td = new TexData("ROOT", 0, index++);

                    td.bModify = false;
                    td.texture = AssetDatabase.LoadAssetAtPath(texPath, typeof(Texture)) as Texture;
                    td.ti = AssetImporter.GetAtPath(texPath) as TextureImporter;
                    td.android = td.ti.GetPlatformTextureSettings("Android");
                    td.ios = td.ti.GetPlatformTextureSettings("iPhone");
                    bool bPass = false;

                    //如果mipmap开启 或者rw开启 一定要显示出来（默认规则）
                    bool bWastedMem = false;
                    if (td.ti.mipmapEnabled | td.ti.isReadable) bWastedMem = true;

                    if ((td.android.format == TextureImporterFormat.ETC2_RGB4 | td.android.format == TextureImporterFormat.ETC2_RGBA8)
                        && (td.ios.format == TextureImporterFormat.PVRTC_RGB4 | td.ios.format == TextureImporterFormat.PVRTC_RGBA4))
                        bPass = true;
                    if (bPass && bTexOkFilter && !bWastedMem)
                    {
                        index--;
                        continue;
                    }

                    //尺寸过滤
                    if (!FilterSize(td.texture) && !bWastedMem)
                    {
                        index--;
                        continue;
                    }

                    td.mipmap = td.ti.mipmapEnabled;
                    td.rw = td.ti.isReadable;
                    td.color = new Color(169 / 255f, 212 / 255f, 115 / 255f, 1f);
                    if (td.ti != null)
                        _config.Add(td);
                    else
                        Debug.LogError(texPath);
                }
            }
        }

        bool FilterSize(Texture tex)
        {
            int w = tex.width, h = tex.height;
            if (w == 64 && h == 64)
            {
                return size64;
            }
            else if (w == 128 && h == 128)
            {
                return size128;
            }
            else if (w == 256 && h == 256)
            {
                return size256;
            }
            else if (w == 512 && h == 512)
            {
                return size512;
            }
            else if (w == 1024 && h == 1024)
            {
                return size1024;
            }
            else
            {
                return sizeother;
            }
        }

    }
}
