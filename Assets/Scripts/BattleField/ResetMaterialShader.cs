using UnityEngine;
using System.Collections;

public class ResetMaterialShader : MonoBehaviour 
{
	void OnEnable()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material[] materials = meshRenderer.sharedMaterials;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].shader = Shader.Find("Spine/SkeletonGhost");
        }
	}

}
