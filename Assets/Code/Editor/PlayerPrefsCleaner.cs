using UnityEditor;
using UnityEngine;

namespace Rubickanov.Opal.EditorHelpers
{
    public static class PlayerPrefsCleaner
    {
        [MenuItem("Tools/Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            if (EditorUtility.DisplayDialog("Clear PlayerPrefs", "Are you sure you want to clear PlayerPrefs?", "Yes", "No"))
            {
                PlayerPrefs.DeleteAll();
                Debug.Log("PlayerPrefs are cleared!");
            }
        }
    }
}