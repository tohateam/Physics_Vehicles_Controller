/*
BASIC CODE EDITOR - CodeWindow.cs
BY SOLUTION STUDIOS

This class defines the editor window and draws all of its contents.
We use Unity's EditorUtility class for Open and Save File Dialogues as System.Windows.Forms will not work on Mac.
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The Class Representing the Basic Code Editor's Editor Window
/// </summary>
public class CodeWindow : EditorWindow {

    // The path of the file which is open
    [SerializeField]
    string pathOpened = "";

    // Whether we have opened a file or not
    [SerializeField]
    bool fileLoaded = false;

    // The file text we are viewing or editing
    [SerializeField]
    string fileText = "";

    // The window's scroll view position
    [SerializeField]
    Vector2 scrollRect = new Vector2 (0, 0);

    // Text Area Settings
    [SerializeField]
    bool wrapText = false;
    [SerializeField]
    bool compileAfterSaving = true;
    [SerializeField]
    bool lineNums = true;

    // Reference to font asset in for the text area
    [SerializeField]
    Font font;

    /// <summary>
    /// Method to create the window used in Window Menu Item
    /// </summary>
    [UnityEditor.MenuItem ("Window/Open Code Editor")]
    public static void CreateWindow () {
        // Create the window
        CodeWindow window = CreateInstance<CodeWindow> ();

        // Set the window title
        window.SetTitle ("Code Editor");

        // Set the window's minimum size and display it
        window.minSize = new Vector2 (250, 250);
        window.Show ();
    }

    /// <summary>
    /// Method to create window used in file based menu items
    /// </summary>
    /// <param name="filePath">The path of the file to open</param>
    public static void CreateWindow (string filePath) {
        // Create the window
        CodeWindow window = CreateInstance<CodeWindow> ();

        // Set the window title
        window.SetTitle ("Code Editor");
        window.OpenFile (filePath);

        // Set the window's minimum size and display it
        window.minSize = new Vector2 (250, 250);
        window.Show ();
    }

    /// <summary>
    /// Opens a file at a path. Used by method above
    /// </summary>
    /// <param name="path">The path of the file</param>
    public void OpenFile (string path) {
        try {
            // Try to open the file
            StreamReader fileReader = new StreamReader (path);
            fileText = fileReader.ReadToEnd ();
            fileReader.Close ();

            // Remeber the path and set the text area text
            pathOpened = path;
            fileLoaded = true;
        } catch { }
    }

    /// <summary>
    /// Draws the elements within the window
    /// </summary>
    private void OnGUI () {
        // Load the text area font from the 'Editor Default Resources' folder
        if (!font) {
            font = (Font) EditorGUIUtility.Load ("Code Editor/Inconsolata-Regular.ttf");
        }

        // If we have opened a file, set the window's title to the file name
        if (fileLoaded) {
            if (!string.IsNullOrEmpty (pathOpened)) {
                SetTitle (System.IO.Path.GetFileName (pathOpened));
            } else {
                SetTitle ("Code Editor");
            }
        }

        GUILayout.BeginHorizontal ();

        // Draw the buttons across the top of the window
        if (GUILayout.Button ("New")) {
            GUI.FocusControl (null); // << Loose focus of the text area so the text can be changed properly
            fileLoaded = true;

            // We are editing a blank file which has not been saved anywhere so keep these blank:
            pathOpened = "";
            fileText = "";
        }
        if (GUILayout.Button ("Open")) {
            // Show Dialogue to open file
            string path = EditorUtility.OpenFilePanel ("Open Text File", UnityEngine.Application.dataPath, "");
            try {
                // Try to open the file
                StreamReader fileReader = new StreamReader (path);
                fileText = fileReader.ReadToEnd ();
                fileReader.Close ();

                GUI.FocusControl (null); // << Loose focus of the text area so the text can be changed properly

                // Remeber the path and set the text area text
                pathOpened = path;
                fileLoaded = true;
            } catch { }
        }
        if (GUILayout.Button ("Save")) {
            if (pathOpened == "") {
                // If it is a new file we don't know the path so we need to display a dialogue to get one (same as save as)
                SaveAsStuff (false);
            } else {
                // We have the path so save the file there
                StreamWriter fileWriter = new StreamWriter (pathOpened, false);
                fileWriter.Write (fileText);
                fileWriter.Close ();
            }
            // Recompile scripts if we should
            if (compileAfterSaving) {
                AssetDatabase.Refresh ();
            }
        }
        if (GUILayout.Button ("Save As")) {
            // Open the file dialogue to do a save as (within SaveAsStuff function)
            SaveAsStuff (false);
            if (compileAfterSaving) {
                AssetDatabase.Refresh ();
            }
        }
        if (GUILayout.Button ("Close")) {
            // Close the window. CloseRequest() shows 'Save Changes?' dialogue if required
            CloseRequest ();
            Close ();
        }

        // Draw the settings to be changed across the top of the window
        GUILayout.FlexibleSpace ();
        GUILayout.Label ("Wrap Text:");
        wrapText = EditorGUILayout.Toggle (wrapText);
        GUILayout.Label ("Compile On Save:");
        compileAfterSaving = EditorGUILayout.Toggle (compileAfterSaving);
        GUILayout.Label ("Line Nums:");
        lineNums = EditorGUILayout.Toggle (lineNums);

        // Draw button to open another Code Editor Window
        GUILayout.FlexibleSpace ();
        if (GUILayout.Button ("+")) {
            CodeWindow window = CreateInstance<CodeWindow> ();
            window.Show ();
        }
        GUILayout.EndHorizontal ();

        // We want the text area within a scroll view. (Always showing vertical scrollbar makes calculating line heights for wrapped text easier)
        scrollRect = GUILayout.BeginScrollView (scrollRect, false, true);

        if (fileLoaded) {
            // Get the default editor style and change it. Replace font, change font size and enable word wrap (if requried)
            GUIStyle editorStyle = new GUIStyle (EditorStyles.textArea);
            editorStyle.font = font;
            editorStyle.fontSize = 16;
            if (wrapText) {
                editorStyle.wordWrap = true;
            } else {
                editorStyle.wordWrap = false;
            }

            // Make the text area take up the space of the rest of the window
            editorStyle.stretchHeight = true;
            editorStyle.stretchWidth = true;

            GUILayout.BeginHorizontal ();

            // Draw line numbers if required
            if (lineNums) {
                GUILayout.BeginVertical (GUILayout.Width (30));

                // Get the editor label style and adjust it for line numbers. We don't want any margins. Set font and font size
                GUIStyle lineNum = new GUIStyle (EditorStyles.label);
                lineNum.margin = new RectOffset (0, 0, 0, 0);
                lineNum.border = new RectOffset (0, 0, 0, 0);
                lineNum.padding = new RectOffset (0, 0, 0, 0);
                lineNum.overflow = new RectOffset (0, 0, 0, 0);
                lineNum.font = font;
                lineNum.fontSize = 16;

                // This lines it up with the text area lines as the text area has padding
                GUILayout.Space (3);

                // Split up the text into lines and iterate through them
                string[] lines = fileText.Split (new string[1] { "\n" }, System.StringSplitOptions.None);
                for (int i = 0; i < lines.Length; i++) {
                    // Here we use the lineNum text style to calculate the height of each line in the text area. So we temporarily enable word wrap if required
                    if (wrapText) {
                        lineNum.wordWrap = true;
                    }
                    GUILayout.BeginHorizontal ();
                    GUILayout.FlexibleSpace ();

                    // Calculate the height of each line within the text area so we know where to position the line numbers. This is required in case word wrap is enabled and a single line in the file takes up multiple lines in the text area.
                    float size = lineNum.lineHeight;
                    if (i < lines.Length) {
                        GUIContent content = new GUIContent (lines[i]);
                        size = lineNum.CalcHeight (content, position.width - 58.0f);
                    }

                    // Diable word wrap for displaying the line numbers
                    lineNum.wordWrap = false;

                    // Display the line number. The height property makes sure the next line number is drawn in the correct place
                    GUILayout.Label ((i + 1) + ":", lineNum, GUILayout.Height (Mathf.Max (size, editorStyle.lineHeight)));
                    GUILayout.EndHorizontal ();
                }
                GUILayout.EndVertical ();
            }

            // Draw the text area for editing the file's text
            GUI.SetNextControlName ("MainTextArea"); // << This is needed so changing the editor window focus works
            fileText = EditorGUILayout.TextArea (fileText, editorStyle);

            GUILayout.EndHorizontal ();
        }
        GUILayout.EndScrollView ();
        Repaint (); // << Redraw the window immediatley instead of waiting for it to be focused on
    }

    /// <summary>
    /// Set the Window Title
    /// </summary>
    /// <param name="title">The new window title</param>
    public void SetTitle (string title) {
        CodeWindow window = this as CodeWindow;

        // The API changed in Unity 5.3 so previous versions use old API
#if UNITY_4_6
        window.title = "Code Editor";
#elif UNITY_5_0
        window.title = "Code Editor";
#elif UNITY_5_1
        window.title = "Code Editor";
#elif UNITY_5_2
        window.title = "Code Editor";
#else
        window.titleContent = new GUIContent ("Code Editor");
#endif
    }

    /// <summary>
    /// Call before closing the window to display 'Save Changes?' Dialogue
    /// </summary>
    public void CloseRequest () {
        if (fileLoaded) {
            if (!string.IsNullOrEmpty (pathOpened)) {
                // Check if any file contents have changed
                StreamReader fileReader = new StreamReader (pathOpened);
                string contents = fileReader.ReadToEnd ();
                fileReader.Close ();
                if (contents == fileText) {
                    // Stop here as the file contents have not changed so we don't need to save again
                    return;
                }
            }

            // Display 'Save Changes?' Dialogue
            if (EditorUtility.DisplayDialog ("Save Changes?", "Do you want to save your changes? If not then all unsaved changes will be lost.", "Yes", "No")) {
                if (pathOpened == "") {
                    // If we don't have a path do a 'Save As'. Tell it the window is closing.
                    SaveAsStuff (true);
                } else {
                    // Save the file to the path we have
                    StreamWriter fileWriter = new StreamWriter (pathOpened, false);
                    fileWriter.Write (fileText);
                    fileWriter.Close ();
                }

                // Tell Unity to recompile scripts if required
                if (compileAfterSaving) {
                    AssetDatabase.Refresh ();
                }
            }
        }
    }

    /// <summary>
    /// Called just before the window is closed
    /// </summary>
    void OnDestroy () {
        // Display the 'Save Changes?' Dialogue if required before closing
        CloseRequest ();
    }

    /// <summary>
    /// Displays dialogue to choose path to save the file
    /// </summary>
    /// <param name="isClosing">Is this called from 'Save Changes?' dialogue on closing. If yes, it will go in a cycle to find a valid path.</param>
    void SaveAsStuff (bool isClosing) {
        // Show the save file dialogue
        string path = EditorUtility.SaveFilePanel ("Save Text File", UnityEngine.Application.dataPath, "New Text Document.txt", "");
        try {
            // Try to save the file and remember the path
            pathOpened = path;
            StreamWriter fileWriter = new StreamWriter (pathOpened, false);
            fileWriter.Write (fileText);
            fileWriter.Close ();
        } catch {
            // If there was a problem saving ask if the user wants to choose another path if the window is closing
            // If the window is not closing we don't need this dialogue as they can just press the 'Save As' button again
            if (isClosing) {
                if (EditorUtility.DisplayDialog ("Invalid Path", "The file could not be saved as the path is not valid. Try again?", "Yes", "No")) {
                    SaveAsStuff (true);
                }
            }
        }
    }
}