using UnityEngine;
using System.Collections;

public class ParticleFilp : MonoBehaviour
{
    public  Transform   ParentTransform;

    private Vector3     BaseLocalScale;
    
    void Awake()
    {
        BaseLocalScale = transform.localScale;
    }

    void OnEnable()
    {
        if (ParentTransform == null)
            return;

        if (ParentTransform.localRotation.eulerAngles.y == 180.0f)    //부모가 180도 돌아가있으면.
            transform.localScale = new Vector3(BaseLocalScale.x * -1.0f, BaseLocalScale.y, BaseLocalScale.z); //반전.
        else
            transform.localScale = BaseLocalScale;
    }

}
