using UnityEngine;
using System.Collections;

public class GetParentScale : MonoBehaviour
{
    public Transform ParentObj;

    void Update()
    {
        if (transform.localScale != ParentObj.localScale)
            transform.localScale = ParentObj.localScale;
    }

}
