using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class RendererUtility : MonoBehaviour
{
    [SerializeField]
    string m_SortingLayerName;

    public string sortingLayerName
    {
        get
        {
            return m_SortingLayerName;
        }

        set
        {
            //if (m_SortingLayerName != value)
            {
                m_SortingLayerName = value;

                foreach (var renderer in GetComponents<Renderer>())
                {
                    SetSortingLayerName(renderer, m_SortingLayerName);
                }
            }
        }
    }

    [SerializeField]
    int m_SortingOrder;

    public int sortingOrder
    {
        get
        {
            return m_SortingOrder;
        }

        set
        {
            //if (m_SortingOrder != value)
            {
                m_SortingOrder = value;

                foreach (var renderer in GetComponents<Renderer>())
                {
                    SetSortingOrder(renderer, m_SortingOrder);
                }
            }
        }
    }

    // Use this for initialization

    // Update is called once per frame

    void OnEnable()
    {
        sortingLayerName = m_SortingLayerName;
        sortingOrder = m_SortingOrder;
    }

    void OnValidate()
    {
        sortingLayerName = m_SortingLayerName;
        sortingOrder = m_SortingOrder;
    }

    public static void SetSortingLayerName(Renderer renderer, string sortingLayerName)
    {
        if (renderer != null)
        {
            renderer.sortingLayerName = sortingLayerName;
        }
    }

    public static void SetSortingOrder(Renderer renderer, int sortingOrder)
    {
        if (renderer != null)
        {
            renderer.sortingOrder = sortingOrder;
        }
    }
}
