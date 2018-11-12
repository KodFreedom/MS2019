using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IlluminObjectController : MonoBehaviour
{
	private void Start ()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Destroy(this);
            return;
        }

        var shared_materials = renderer.sharedMaterials;
        foreach(var shared_material in shared_materials)
        {
            IlluminMaterialController.Instance.Register(shared_material);
        }
    }
}
