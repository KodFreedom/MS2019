using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IlluminObjectController : MonoBehaviour
{
    [SerializeField] SphereCollider IlluminAwaker = null;
    private Material material_ = null;

	private void Start ()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Destroy(this);
            return;
        }

        material_ = renderer.material;
	}
	
	private void Update ()
    {
        material_.SetVector("_IlluminAwakerPosition", IlluminAwaker.transform.position);
        material_.SetFloat("_IlluminAwakerRadius", IlluminAwaker.radius);
	}
}
