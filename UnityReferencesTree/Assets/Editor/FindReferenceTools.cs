//Author:wangjiaying
//Date:2016.11.16
//Function:
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 用于处理寻找物体引用
/// 并显示出来
/// </summary>
public class FindReferenceTools : EditorWindow
{

    [MenuItem("Assets/Find References")]
    public static void FindRefernce()
    {
        Object o = Selection.activeObject;
        if (o == null)
        {
            EditorUtility.DisplayDialog("提示", "错误：请至少选择一个物体！", "OK");
            return;
        }

        _mode = ShowMode.ReferenceMode;
        _target = o;
        BeginFindReference();

        OpenWindow();
    }

    [MenuItem("Tools/Open Refernces Find Window")]
    public static void OpenWindow()
    {
        FindReferenceTools win = EditorWindow.GetWindow<FindReferenceTools>();
        win.position = new Rect(win.position.x, win.position.y, 700, 600);
        win.Show();
    }

    private static Object _target;
    private static string[] _referenceList;
    private static RefFolderItem _refFolderItem;
    private static RefItem _refItem;

    private static bool recursive = false;
    private static ShowMode _mode = ShowMode.FolderMode;
    private static LayoutDisplay _displayMode = LayoutDisplay.NestMode;

    private GUIStyle titleStyle = null;
    private GUIStyle textStyle = null;

    private Vector2 _scroll;

    void OnGUI()
    {
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle();
            titleStyle.normal.textColor = Color.green;
            titleStyle.fontSize = 25;
        }
        if (textStyle == null)
        {
            textStyle = new GUIStyle();
            textStyle.normal.textColor = Color.yellow;
            textStyle.fontSize = 16;
        }

        GUILayout.Space(15);
        EditorGUILayout.LabelField("----> 引用查询器", titleStyle);
        GUILayout.Space(30);

        _target = EditorGUILayout.ObjectField("当前查询物体：", _target, typeof(Object), false);

        if (_target)
        {
            recursive = EditorGUILayout.ToggleLeft("递归查找", recursive);
            _mode = (ShowMode)EditorGUILayout.EnumPopup("查找模式", (System.Enum)_mode);
            _displayMode = (LayoutDisplay)EditorGUILayout.EnumPopup("显示模式", (System.Enum)_displayMode);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("->");
            if (GUILayout.Button("重新寻找引用", GUILayout.Height(25)))
                BeginFindReference();
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        EditorGUILayout.BeginVertical(GUI.skin.box);
        //if (_referenceList != null)
        //    for (int i = 0; i < _referenceList.Length; i++)
        //    {
        //        EditorGUILayout.TextField(_referenceList[i]);
        //    }
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        switch (_mode)
        {
            case ShowMode.FolderMode:
                if (_refFolderItem != null)
                    ShowRefFolderItems(_refFolderItem);
                break;
            case ShowMode.ReferenceMode:
                if (_refItem != null)
                {
                    switch (_displayMode)
                    {
                        case LayoutDisplay.NestMode:
                            ShowRefItemsNest(_refItem);
                            break;
                        case LayoutDisplay.DirectMode:
                            ShowRefItemsDirect(_refItem);
                            break;
                    }
                }
                break;
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    private void ShowRefFolderItems(RefFolderItem items, int layer = 0)
    {
        EditorGUILayout.LabelField(new string('-', layer * 2) + items._name, textStyle);
        GUILayout.Space(5);

        if (items._next.Count < 1) return;

        for (int i = 0; i < items._next.Count; i++)
        {
            ShowRefFolderItems(items._next[i], layer + 1);
        }
    }

    private void ShowRefItemsNest(RefItem items, int layer = 0)
    {
        //string space = new string(' ', layer * 5);
        //items._fold = EditorGUILayout.Foldout(items._fold, name, textStyle);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(" ", GUILayout.Width(layer * 30));

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.BeginVertical();
        //EditorGUILayout.LabelField(new string(' ', layer * 5) + items._object.name, textStyle);
        //EditorGUILayout.LabelField(new string(' ', layer * 5 + (textStyle.fontSize - GUI.skin.font.fontSize)) + "(" + items._path + ")");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent(AssetPreview.GetMiniThumbnail(items.GetObject)), GUILayout.Height(22), GUILayout.Width(22));
        EditorGUILayout.LabelField(items.Name, textStyle);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("(" + items.Path + ")");

        GUILayout.Space(5);

        if (items._refList.Count > 0)
        {
            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Label(" ", GUILayout.Width(layer * 45));
            items._isFold = !EditorGUILayout.Foldout(!items._isFold, name);
            //EditorGUILayout.EndHorizontal();

            if (!items._isFold)
            {
                //EditorGUILayout.BeginVertical(GUI.skin.box);
                for (int i = 0; i < items._refList.Count; i++)
                {
                    ShowRefItemsNest(items._refList[i], layer + 1);
                }
                //EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.EndVertical();
        if (GUILayout.Button("定位"))
            EditorGUIUtility.PingObject(items.GetObject);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(15);
    }


    private void ShowRefItemsDirect(RefItem items, int layer = 0)
    {
        //string space = new string(' ', layer * 5);
        //items._fold = EditorGUILayout.Foldout(items._fold, name, textStyle);
        EditorGUILayout.BeginHorizontal();
        //GUILayout.Label(" ", GUILayout.Width(layer * 30));

        EditorGUILayout.BeginHorizontal(/*GUI.skin.box*/);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(new string(' ', layer * 5) + items.Name, textStyle);
        EditorGUILayout.LabelField(new string(' ', layer * 5 + (textStyle.fontSize - GUI.skin.font.fontSize)) + "(" + items.Path + ")");

        EditorGUILayout.EndVertical();
        if (GUILayout.Button("定位"))
            EditorGUIUtility.PingObject(items.GetObject);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (items._refList.Count > 0)
        {
            items._isFold = !EditorGUILayout.Foldout(!items._isFold, name);

            if (!items._isFold)
            {
                for (int i = 0; i < items._refList.Count; i++)
                {
                    ShowRefItemsDirect(items._refList[i], layer + 1);
                }
            }
        }


        GUILayout.Space(15);
    }


    private static void BeginFindReference()
    {
        if (_target == null)
            return;

        switch (_mode)
        {
            case ShowMode.FolderMode:
                _referenceList = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(_target), recursive);

                _refFolderItem = new RefFolderItem();

                for (int i = 0; i < _referenceList.Length; i++)
                {
                    _refFolderItem.AddRefItem(_referenceList[i]);
                }
                break;
            case ShowMode.ReferenceMode:
                _refItem = new RefItem();
                _refItem.InitRef(_target);
                break;
        }

    }

    public class RefFolderItem
    {
        public string _name;
        public RefFolderItem _parent;
        public List<RefFolderItem> _next = new List<RefFolderItem>();

        public void AddRefItem(string path)
        {
            Regex reg = new Regex(@"(\S+?)/");
            if (reg.IsMatch(path))
            {
                string name = reg.Match(path).Groups[1].Value;
                if (string.IsNullOrEmpty(_name))
                    _name = name;

                string full = reg.Match(path).Value;
                string nextPath = path.Replace(full, "");
                string nextName = reg.Match(nextPath).Groups[1].Value;

                RefFolderItem item = _next.Find((i) => i._name == nextName);
                if (item == null)
                {
                    item = new RefFolderItem();
                    item._parent = this;
                    _next.Add(item);
                }

                item.AddRefItem(nextPath);
            }
            else _name = path;
        }

        public bool IsFile()
        {
            return _next.Count < 1;
        }

        public string GetFullPath()
        {
            return "";
        }

    }

    public class RefItem
    {
        /// <summary>
        /// 是否折叠
        /// </summary>
        public bool _isFold;

        private Object _object;
        private string _path;
        private string _size;

        public List<RefItem> _refList = new List<RefItem>();

        public void InitRef(Object o)
        {
            _object = o;
            _path = AssetDatabase.GetAssetPath(o);
            //计算硬盘占用大小
            FileInfo info = new FileInfo(_path);
            if (info.Exists)
            {
                float size = info.Length;
                if (size < 1024)
                    _size = size.ToString() + "B";
                else if (size < 1024 * 1024)
                    _size = System.Math.Round((size / 1024), 2).ToString() + "KB";
                else _size = System.Math.Round((size / 1024 / 1024), 2).ToString() + "M";
            }
            string[] paths = AssetDatabase.GetDependencies(_path, false);
            for (int i = 0; i < paths.Length; i++)
            {
                RefItem item = new RefItem();
                item.InitRef(AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object)));
                _refList.Add(item);
            }
        }

        public string Name
        {
            get
            {
                if (_object)
                    return _object.name + "(" + _size + ")";
                return "Null";
            }
        }

        public string Path { get { return _path; } }

        public Object GetObject { get { return _object; } }
    }

    public enum ShowMode
    {
        FolderMode,
        ReferenceMode,
    }

    public enum LayoutDisplay
    {
        NestMode,
        DirectMode,
    }

}
