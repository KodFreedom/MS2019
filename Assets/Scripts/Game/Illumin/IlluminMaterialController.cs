using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IlluminMaterialController : MonoBehaviour
{
    public static IlluminMaterialController Instance { get; private set; }
    [SerializeField] SphereCollider IlluminAwaker;
    private Dictionary<string, Material> shared_materials_ = new Dictionary<string, Material>(); 

    public void Register(Material shared_material)
    {
        if (!shared_materials_.ContainsKey(shared_material.name))
        {
            shared_materials_.Add(shared_material.name, shared_material);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

	private void Update ()
    {
		foreach(var at in shared_materials_)
        {
            var shared_material = at.Value;
            shared_material.SetVector("_IlluminAwakerPosition", IlluminAwaker.transform.position);
            shared_material.SetFloat("_IlluminAwakerRadius", IlluminAwaker.radius);
        }
    }
}
