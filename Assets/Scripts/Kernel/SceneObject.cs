using System.Collections;
using UnityEngine;

// @TODO : Automatically generate.
public enum Scene
{
    None,
    Deck,
    Lobby,
    TitleScene,
    Battle,
    Guild,
    Adventure,
    Treasure,
    Ranking,
    Achieve,
    NormalShop,
    StrangeShop,
    SecretBusiness,
    RevengeBattle,
    Franchise,
    Detect,
    DetectAR,
    Notice,
}

public class SceneObject : MonoBehaviour
{
    [SerializeField]
    Scene m_Scene;

    public Scene scene
    {
        get
        {
            return m_Scene;
        }
    }

    protected bool completed
    {
        get;
        set;
    }

    public delegate void OnPreprocessCompleteCallback(SceneObject sceneObject);
    public OnPreprocessCompleteCallback onPreprocessCompleteCallback;
    /*
    public float progress
    {
        get;
        protected set;
    }
    */
    protected virtual void Awake()
    {
#if UNITY_EDITOR
        if (FindObjectsOfType<SceneObject>().Length > 1)
        {
            Debug.LogError("");
        }
#endif
    }

    // Use this for initialization
    protected virtual void Start()
    {
        completed = true;
        Kernel.sceneManager.activeSceneObject = this;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    public virtual IEnumerator Preprocess()
    {
        while (!completed)
        {
            yield return 0;
        }

        if (onPreprocessCompleteCallback != null)
        {
            onPreprocessCompleteCallback(this);
        }

        yield break;
    }
}
