using UnityEditor;
using UnityEngine;


public class esEditorWindowVerTitle : esLibs.esEditorWindowTitle
{
	protected override void setWinTitle(EditorWindow win, string title)
	{
		#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
			base.setWinTitle(win, title);
		#else
			win.titleContent = new GUIContent(title);
		#endif
	}
}
