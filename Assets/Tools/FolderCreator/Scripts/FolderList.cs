using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HawkQ
{
    [CreateAssetMenu(fileName = "New Folder List", menuName = "Folder Creator/Folder List")]
    [System.Serializable]
    public class FolderList : ScriptableObject
    {
        [HideInInspector]
        public List<Folder> folderList = new List<Folder>();
    }
}
