using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IlluminController : MonoBehaviour
{
    [SerializeField] Texture kNonIlluminTexture = null;
    private Light point_light_ = null;
    private Material material_ = null;
    private Texture illumin_texture_ = null;
    private static Texture sNonIlluminTexture = null;

    private void Awake()
    {
        if (kNonIlluminTexture)
        {
            sNonIlluminTexture = kNonIlluminTexture;
        }
    }

    // Use this for initialization
    private void Start ()
    {
        point_light_ = GetComponent<Light>();
        if(point_light_)
        {
            point_light_.enabled = false;
        }

        var renderer = GetComponent<MeshRenderer>();
        if(renderer)
        {
            material_ = renderer.material;
            illumin_texture_ = material_.GetTexture("_Illum");
            material_.SetTexture("_Illum", sNonIlluminTexture);
        }
	}
	
	// Update is called once per frame
	private void Update ()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(point_light_)
        {
            point_light_.enabled = true;
        }

        if(material_)
        {
            material_.SetTexture("_Illum", illumin_texture_);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (point_light_)
        {
            point_light_.enabled = false;
        }

        if (material_)
        {
            material_.SetTexture("_Illum", sNonIlluminTexture);
        }
    }
}
