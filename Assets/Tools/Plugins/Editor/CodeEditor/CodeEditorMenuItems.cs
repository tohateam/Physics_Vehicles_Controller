using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CodeEditorMenuItems : EditorWindow {

    /// <summary>
    /// Adds the 'Open in Code Editor' Item to the Assets Context Menu
    /// </summary>
    [MenuItem("Assets/Open in Code Editor")]
    private static void OpenInCodeEditor ()
    {
        if (Selection.activeObject.GetType() == typeof(TextAsset))
        {
            TextAsset asset = Selection.activeObject as TextAsset;
            CodeWindow.CreateWindow(Application.dataPath + AssetDatabase.GetAssetPath(asset).Substring(6));
        }
        if (Selection.activeObject.GetType() == typeof(MonoScript))
        {
            MonoScript asset = Selection.activeObject as MonoScript;
            CodeWindow.CreateWindow(Application.dataPath + AssetDatabase.GetAssetPath(asset).Substring(6));
        }
    }

    /// <summary>
    /// Enables/Disables the 'Open in Code Editor' Item to the Assets Context Menu created above
    /// </summary>
    /// <returns>Can the selected file be opened by the Code Editor</returns>
    [UnityEditor.MenuItem("Assets/Open in Code Editor", true)]
    private static bool OpenInCodeEditorValidation ()
    {
        bool isTrue = false;
        if (Selection.activeObject != null)
        {
            if (Selection.activeObject.GetType() != null)
            {
                if (Selection.activeObject.GetType() == typeof(TextAsset))
                {
                    isTrue = true;
                }
                if (Selection.activeObject.GetType() == typeof(MonoScript))
                {
                    isTrue = true;
                }
            }
        }
        return isTrue;
    }

    /// <summary>
    /// Adds the ability to create text files in the Project view
    /// </summary>
    [UnityEditor.MenuItem("Assets/Create/Text File", false, 82)]
    public static void CreateTextFile()
    {
        string path = "";
        Object obj = Selection.activeObject;
        if (obj == null) path = GetSelectedPathOrFallback();
        else path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
        if (path.Length > 5)
        {
            if (Directory.Exists(path))
            {
                if (path[path.Length - 1] != '/')
                {
                    path = path + '/';
                }
                int fileNum = 0;
                while (!FileNumValid(fileNum, path))
                {
                    fileNum++;
                }
                string fileName = "New Text File.txt";
                if (fileNum > 0)
                {
                    fileName = "New Text File " + fileNum + ".txt";
                }
                StreamWriter writer = new StreamWriter(path + fileName);
                writer.Write("");
                writer.Close();
                AssetDatabase.Refresh();
            } else
            {
                if (File.Exists(path))
                {
                    string dir = Path.GetDirectoryName(path) + "\\";
                    int fileNum = 0;
                    while (!FileNumValid(fileNum, dir))
                    {
                        fileNum++;
                    }
                    string fileName = "New Text File.txt";
                    if (fileNum > 0)
                    {
                        fileName = "New Text File " + fileNum + ".txt";
                    }
                    StreamWriter writer = new StreamWriter(dir + fileName);
                    writer.Write("");
                    writer.Close();
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    /// <summary>
    /// Can we create a text file with number num in directory dir
    /// </summary>
    /// <param name="num">The text file number. Part of the name e.g. in 'New Text File 4' num is 4</param>
    /// <param name="dir">The proposed directory to create the file</param>
    /// <returns>Can we create text file there</returns>
    public static bool FileNumValid (int num, string dir)
    {
        string fileName = "New Text File.txt";
        if (num > 0)
        {
            fileName = "New Text File " + num + ".txt";
        }
        return !(File.Exists(dir + fileName));
    }

    /// <summary>
    /// Method to get the selected directory in the Project View
    /// </summary>
    /// <returns>Selected Directory in the Project View</returns>
    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
}

/// <summary>
/// When you select a GameObject in the Heirachy, there is a 'Open in Code Editor' button at the bottom of every Monobehaviour based component.
/// This editor class adds that functionality.
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true)]
public class CodeEditorInspectorOne : Editor
{
    public override void OnInspectorGUI()
    {
        try { 
        base.OnInspectorGUI();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open in Code Editor"))
        {
                MonoBehaviour myTarget = (MonoBehaviour)target;
                MonoScript script = MonoScript.FromMonoBehaviour(myTarget);
                CodeWindow.CreateWindow(Application.dataPath + AssetDatabase.GetAssetPath(script).Substring(6));
            }
        GUILayout.EndHorizontal();
    } catch
        {
            base.OnInspectorGUI();
        }
    }
}

/// <summary>
/// When you select a Script file in the Project view, there is a 'Open in Code Editor' button at the bottom of the import settings in the Inspector.
/// This editor class adds that functionality.
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(MonoScript), true)]
public class CodeEditorInspectorTwo : Editor
{
    public override void OnInspectorGUI()
    {
        try { 
            base.OnInspectorGUI();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open in Code Editor", GUILayout.Height(30)))
            {
                MonoScript script = (MonoScript)target;
                CodeWindow.CreateWindow(Application.dataPath + AssetDatabase.GetAssetPath(script).Substring(6));
            }
            GUILayout.EndHorizontal();
        }
        catch 
        {
            base.OnInspectorGUI();
        }
    }
}

/// <summary>
/// When you select a TextAsset in the Project view (normally a .txt file), there is a 'Open in Code Editor' button at the bottom of the import settings in the Inspector.
/// This editor class adds that functionality.
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(TextAsset), true)]
public class CodeEditorInspectorThree : Editor
{
    public override void OnInspectorGUI()
    {
        try
        {
            EditorGUI.EndDisabledGroup();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open in Code Editor", GUILayout.Height(30)))
            {
                TextAsset script = (TextAsset)target;
                CodeWindow.CreateWindow(Application.dataPath + AssetDatabase.GetAssetPath(script).Substring(6));
            }
            GUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(true);
        } catch
        {
            base.OnInspectorGUI();
        }
    }
}

    
