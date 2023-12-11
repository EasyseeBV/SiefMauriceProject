using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class Displacement : MonoBehaviour
{
    public Transform Target;
    private Material Material;

    [Range(0,1f)]
    public float Scale;

	// Use this for initialization
	void Start ()
    {
        Material = GetComponent<SpriteRenderer>().sharedMaterial;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Target == null)
            Target = Camera.main.transform;
        Vector2 direction = (Target.position - transform.position).normalized;
        Material.SetVector("_Scale", direction * Scale);	
	}
}
