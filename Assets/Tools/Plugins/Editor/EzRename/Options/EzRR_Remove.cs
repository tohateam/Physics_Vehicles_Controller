using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SMG.EzRenamer
{
	public class EzRR_Remove : ScriptableObject 
	{
		private bool showOption = false;
		
		private enum RemoveOptions
		{
			start,
			end,
			custom
		}
		private RemoveOptions removeOption;
		
		//private bool removeTillEnd = true;
		private int startIndex;
		private int count;
		private string removeNewName;
		
		public void Draw()
		{
			EditorGUILayout.BeginHorizontal();
			showOption = EzRR_Style.DrawFoldoutHeader("Remove", showOption);
			EzRR_Style.DrawHelpButton("https://solomidgames.com/guides/ezRename/Remove.html");
			EditorGUILayout.EndHorizontal();
			if(showOption)
			{
				EditorGUI.indentLevel = 1;
				DrawRemoveCharacters();
				EditorGUILayout.Space();
				DrawButtons();
				EditorGUI.indentLevel = 0;
			}
		}
		
		private void DrawRemoveCharacters()
		{
			removeOption = (RemoveOptions)EditorGUILayout.EnumPopup("Remove From", removeOption);
			if(removeOption == RemoveOptions.custom)
			{
				startIndex = EditorGUILayout.IntField("Start Index", startIndex);
			}
			count = EditorGUILayout.IntField("Count", count);
			// Prevent input mistakes
			if(startIndex < 0)
				startIndex = 0;
			if(count < 0)
				count = 0;
		}
		
		private void DrawButtons()
		{
			EzRR_Style.DrawHeader("Do Remove On:");
			EzRR_Style.DrawButton("Hierarchy", DoRemoveHierarchy);
			EzRR_Style.DrawButton("Project Folder", DoRemoveProjectFolder);
		}
		
		private void DoRemoveHierarchy()
		{
			GameObject[] _gameObjectsSelected = Selection.gameObjects;
			if(ErrorsOnHierarchy(_gameObjectsSelected))
				return;
			// Calculate the amount that each file will increase in the progress bar
			float _result = (float)_gameObjectsSelected.Length / 100f;
			Undo.RecordObjects(_gameObjectsSelected, "remove");
			for (int i = 0; i < _gameObjectsSelected.Length; i++)
			{
				EditorUtility.DisplayProgressBar(FEEDBACKS.Title._04, FEEDBACKS.Remove._00, _result * i);
				removeNewName = _gameObjectsSelected[i].name;
				// Duplicates the startIndex and count to keep safe the wanted value
				int _startIndex = startIndex;
				int _count = count;
				// Remove
				if(removeOption == RemoveOptions.start)
				{
					if((startIndex + count) > removeNewName.Length)
						_count = removeNewName.Length - startIndex;
					removeNewName = removeNewName.Remove(0, _count);
				}
				else if(removeOption == RemoveOptions.end)
				{
					if(count > removeNewName.Length)
						_count = removeNewName.Length;
					startIndex = removeNewName.Length - _count;
					removeNewName = removeNewName.Remove(startIndex, _count);
				}
				else
				{
					if(startIndex > removeNewName.Length)
						_startIndex = removeNewName.Length;
					if((_startIndex + count) > removeNewName.Length)
						_count = removeNewName.Length - _startIndex;
					removeNewName = removeNewName.Remove(_startIndex, _count);
				}
				// Set the new name
				_gameObjectsSelected[i].name = removeNewName;
			}

			EditorUtility.ClearProgressBar();
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
		
		private bool ErrorsOnHierarchy(GameObject[] gameObjectsSelected)
		{
			// Verifies if there's at least one gameobject selected
			if (gameObjectsSelected.Length <= 0)
			{
				EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.General._00, FEEDBACKS.Button._00);
				return true;
			}
			else
				return false;
		}
		
		private void DoRemoveProjectFolder()
		{
			Object[] _objectsSelected = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
			if(ErrorsOnProjectFolder(_objectsSelected))
				return;
			// Keep the path of the current file
			string _path;
			// Calculate the amount that each file will increase in the progress bar
			float _result = (float)_objectsSelected.Length / 100f;
			// Rename the files
			for (int i = 0; i < _objectsSelected.Length; i++)
			{
				EditorUtility.DisplayProgressBar(FEEDBACKS.Title._04, FEEDBACKS.Remove._00, _result * i);
				_path = AssetDatabase.GetAssetPath(_objectsSelected[i]);
				removeNewName = _objectsSelected[i].name;
				// Duplicates the startIndex and count to keep safe the wanted value
				int _startIndex = startIndex;
				int _count = count;
				// Remove
				if(removeOption == RemoveOptions.start)
				{
					if((startIndex + count) > removeNewName.Length)
						_count = removeNewName.Length - startIndex;
					removeNewName = removeNewName.Remove(0, _count);
				}
				else if(removeOption == RemoveOptions.end)
				{
					if(count > removeNewName.Length)
						_count = removeNewName.Length;
					startIndex = removeNewName.Length - _count;
					removeNewName = removeNewName.Remove(startIndex, _count);
				}
				else
				{
					if(startIndex > removeNewName.Length)
						_startIndex = removeNewName.Length;
					if((_startIndex + count) > removeNewName.Length)
						_count = removeNewName.Length - _startIndex;
					removeNewName = removeNewName.Remove(_startIndex, _count);
				}
				// Set the new name
				string _message = AssetDatabase.RenameAsset(_path, removeNewName);
				if(!string.IsNullOrEmpty(_message))
					Debug.LogWarning(_message);
			}
			EditorUtility.ClearProgressBar();
		}
		
		private bool ErrorsOnProjectFolder(Object[] objectsSelecte)
		{
			// Verifies if there's at least one object selected
			if (objectsSelecte.Length <= 0)
			{
				EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.General._01, FEEDBACKS.Button._00);
				return true;	
			}
			else
				return false;
		}
	}
}