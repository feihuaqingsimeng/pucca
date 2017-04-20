/*
 * PlayerPrefs 초기화를 위한 에디터 스크립트
 */

using UnityEngine;
using UnityEditor;

public class PlayerPrefsEditorUtility : MonoBehaviour
{
    [MenuItem("Tools/PlayerPrefs/Delete All")]
    static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs deleted");
    }
}
