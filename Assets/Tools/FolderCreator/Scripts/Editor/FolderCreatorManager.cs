using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HawkQ {
    public class FolderCreatorManager : BaseEditorWindow {
        private static FolderCreatorManager _window;
        private static FolderList folderList;

        private ReorderableList m_FolderList;
        private SerializedObject m_FolderSerializedObject;
        private string basePath;
        private Vector2 scrollPos;

        public static FolderCreatorManager window {
            get {
                if (_window == null)
                    _window = GetWindow<FolderCreatorManager> (false, "Folder Creator", false);
                return _window;
            }
        }

        [MenuItem ("Tools/HawkQ/Folder Creator")]
        static void OpenQ_InventoryMainEditorWindow () {
            _window = GetWindow<FolderCreatorManager> (false, "Folder Creator", true);
        }

        private void OnEnable () {
            //if (EditorPrefs.HasKey("FolderListPath"))
            //{
            //    string objectPath = EditorPrefs.GetString("FolderListPath");
            //    folderList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(FolderList)) as FolderList;

            //    InitializeReorderableList();
            //}
            if (EditorPrefs.HasKey ("Folder Base Path")) {
                basePath = EditorPrefs.GetString ("Folder Base Path");
            } else {
                basePath = Application.dataPath;
            }

            if (folderList != null) {
                InitializeReorderableList ();
            }
        }

        private void OnGUI () {
            scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

            GUILayout.Space (10);

            GUILayout.Space (5);
            GUILayout.Label ("Folder Creator", normalCenterStyle);
            GUILayout.Space (5);

            if (folderList == null) {
                //if (GUILayout.Button("Choose a Folder List", GUILayout.MinHeight(50f)))
                //{
                //    OpenFolderCreator();
                //}
                GUILayout.Label ("Please Select A Folder List Assset");
            }

            EditorGUI.BeginChangeCheck ();
            folderList = EditorGUILayout.ObjectField ("Folder List", folderList, typeof (FolderList), false) as FolderList;
            if (EditorGUI.EndChangeCheck ()) {
                if (folderList != null)
                    InitializeReorderableList ();
                else
                    ClearReorderableList ();
            }

            if (folderList != null) {
                DrawMainWindow ();
            }

            EditorGUILayout.EndScrollView ();
            if (GUI.changed && folderList != null) {
                EditorUtility.SetDirty (folderList);
            }
        }

        void OpenFolderCreator () {
            string absPath = EditorUtility.OpenFilePanel ("Select a Folder List", "", "");
            string relPath = absPath;

            if (absPath.StartsWith (Application.dataPath)) {
                relPath = absPath.Substring (Application.dataPath.Length - "Assets".Length);
            }

            folderList = AssetDatabase.LoadAssetAtPath (relPath, typeof (FolderList)) as FolderList;
            EditorPrefs.SetString ("FolderListPath", relPath);

            InitializeReorderableList ();
        }

        void DrawMainWindow () {

            GUILayout.BeginHorizontal ();
            GUILayout.Label ("Base Path:");
            GUILayout.Space (5);
            EditorGUILayout.LabelField (basePath);
            GUILayout.EndHorizontal ();

            if (GUILayout.Button ("Change Base Path")) {
                string tempPath = EditorUtility.OpenFolderPanel ("Select a Folder List", basePath, "");
                if (tempPath.Length != 0) {
                    basePath = tempPath;
                    EditorPrefs.SetString ("Folder Base Path", basePath);
                }
            }

            GUILayout.Space (10);
            m_FolderSerializedObject.ApplyModifiedProperties ();
            m_FolderSerializedObject.Update ();
            m_FolderList.DoLayoutList ();
            m_FolderSerializedObject.ApplyModifiedProperties ();

            if (GUILayout.Button ("Create")) {
                CreateFolder ();
            }

            GUILayout.Space (10);
        }

        void CreateFolder () {
            string m_dataPath = basePath;

            if (m_dataPath.StartsWith (Application.dataPath)) {
                m_dataPath = m_dataPath.Substring (Application.dataPath.Length - "Assets".Length);
            }
            string folderName;
            foreach (var folder in folderList.folderList) {
                folderName = folder.folderName;

                if (folderName.Length == 0) {
                    Debug.Log ("The folder name can't be null");
                    return;
                }

                if (folderName.Contains ("/")) {
                    string innerPath = null;
                    string realName = folderName;
                    int flag = folderName.LastIndexOf ("/");

                    innerPath = folderName.Substring (0, flag);
                    realName = folderName.Substring (flag + 1);

                    if (!Directory.Exists (m_dataPath + "/" + folderName)) {
                        Debug.Log ("Success Create " + folderName + " Folder");
                        AssetDatabase.CreateFolder (m_dataPath + "/" + innerPath, realName);
                    } else
                        Debug.Log (folderName + " Folder Already Exists");
                } else {
                    if (!Directory.Exists (m_dataPath + "/" + folderName)) {
                        Debug.Log ("Success Create " + folderName + " Folder");
                        AssetDatabase.CreateFolder (m_dataPath, folderName);
                    } else
                        Debug.Log (folderName + " Folder Already Exists");
                }
            }

            AssetDatabase.Refresh ();
        }

        void InitializeReorderableList () {
            m_FolderSerializedObject = new SerializedObject (folderList);
            FolderCreateReorderableList ();
            FolderSetupReoirderableListHeaderDrawer ();
            FolderSetupReorderableListElementDrawer ();
            FolderSetupReorderableListOnAddDropdownCallback ();
        }

        void ClearReorderableList () {
            m_FolderList = null;
            m_FolderSerializedObject = null;

        }

        void FolderCreateReorderableList () {
            m_FolderList = new ReorderableList (
                m_FolderSerializedObject,
                m_FolderSerializedObject.FindProperty ("folderList"),
                true, true, true, true);
            m_FolderList.elementHeight = 50;
        }

        void FolderSetupReoirderableListHeaderDrawer () {
            m_FolderList.drawHeaderCallback =
                (Rect rect) => {
                    EditorGUI.LabelField (new Rect (rect.x, rect.y, rect.width - 60, rect.height), "Folder Name");

                    EditorGUI.LabelField (new Rect (rect.x + rect.width / 2, rect.y, rect.width - 60, rect.height), folderList.folderList.Count.ToString () + " Folders");
                };
        }

        void FolderSetupReorderableListElementDrawer () {

            m_FolderList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) => {
                    var element = m_FolderList.serializedProperty.GetArrayElementAtIndex (index);
                    rect.y += 2;

                    float delayWidth = 60;
                    float nameWidth = rect.width - delayWidth;

                    EditorGUI.LabelField (new Rect (rect.x, rect.y, nameWidth / 2, 20), "Folder " + (++index).ToString ());

                    EditorGUI.PropertyField (
                        new Rect (rect.x, rect.y + 20, rect.width, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative ("folderName"), GUIContent.none);
                };
        }

        void FolderSetupReorderableListOnAddDropdownCallback () {
            m_FolderList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) => {
                    FolderOnReorderableListAddDropdownClick ();
                };
        }
        void FolderOnReorderableListAddDropdownClick () {
            int index = m_FolderList.serializedProperty.arraySize;
            m_FolderList.serializedProperty.arraySize++;
            m_FolderList.index = index;

            SerializedProperty element = m_FolderList.serializedProperty.GetArrayElementAtIndex (index);
            element.FindPropertyRelative ("folderName").stringValue = null;
            m_FolderSerializedObject.ApplyModifiedProperties ();
        }
    }
}