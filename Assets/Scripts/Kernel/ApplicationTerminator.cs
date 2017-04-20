using UnityEngine;

public class ApplicationTerminator : MonoBehaviour
{

    // Use this for initialization

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //** 웹뷰 문제로 인한 공지사항 씬에서는 뒤로가기가 안됨.
            if (Equals(Kernel.sceneManager.activeSceneObject.scene.ToString(), Scene.Notice.ToString()))
                return;

            UIAlerter.Alert(Languages.ToStringBuiltIn(TEXT_UI.APP_OFF), UIAlerter.Composition.Confirm_Cancel, OnApplicationQuitResponded);
        }
    }

    void OnApplicationQuitResponded(UIAlerter.Response response, params object[] args)
    {
        if (response != UIAlerter.Response.Confirm)
        {
            return;
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
