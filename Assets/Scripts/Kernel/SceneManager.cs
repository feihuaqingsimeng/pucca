
public class SceneManager : Singleton<SceneManager>
{
    SceneObject m_ActiveSceneObject;

    public SceneObject activeSceneObject
    {
        get
        {
            return m_ActiveSceneObject;
        }

        set
        {
            if (m_ActiveSceneObject != value)
            {
                m_ActiveSceneObject = value;

                if (m_ActiveSceneObject != null)
                {
                    Preprocess(m_ActiveSceneObject);
                }
            }
        }
    }

    public bool isSceneLoading
    {
        get;
        private set;
    }

    public delegate void OnStartLoadScene(Scene scene);
    public OnStartLoadScene onStartLoadScene;

    public delegate void OnCompleteLoadScene(Scene scene);
    public OnCompleteLoadScene onCompleteLoadScene;

    public delegate void OnLoadSceneCloudEvent(Scene scene);
    public OnLoadSceneCloudEvent onLoadSceneCloudEvent;


    // Use this for initialization

    // Update is called once per frame

    void OnPreprocessCompleteCallback(SceneObject sceneObject)
    {
        if (sceneObject != null)
        {
            sceneObject.onPreprocessCompleteCallback -= OnPreprocessCompleteCallback;

            if (onCompleteLoadScene != null)
            {
                onCompleteLoadScene(sceneObject.scene);
            }

            isSceneLoading = false;
        }
    }

    void Preprocess(SceneObject sceneObject)
    {
        if (sceneObject != null)
        {
            sceneObject.onPreprocessCompleteCallback += OnPreprocessCompleteCallback;

            StartCoroutine(sceneObject.Preprocess());
        }
    }

    public void LoadScene(Scene scene)
    {
        if (Equals(Scene.None, scene))
        {
            return;
        }

        string sceneName = GetSceneName(scene);
        if (!string.IsNullOrEmpty(sceneName))
        {
            isSceneLoading = true;

            if (onStartLoadScene != null)
            {
                onStartLoadScene(scene);
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }


    //구름 연출용 LoadScene.
    public void LoadScene(Scene scene, bool CloudEffectMode)
    {
        if (CloudEffectMode)    //구름연출 로딩.
            onLoadSceneCloudEvent(scene);
        else
            LoadScene(scene);
    }



    string GetSceneName(Scene scene)
    {
        return scene.ToString();
    }
}
