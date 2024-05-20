using UnityEngine;

public class HighlightArea : MonoBehaviour
{
    public Material highlightMaterial; // Drag and drop the highlight material in the Unity Editor

    void Start()
    {
        if (highlightMaterial == null)
        {
            Debug.LogError("Highlight material not assigned! Please assign a material in the Unity Editor.");
            enabled = false;
        }
    }

    void Update()
    {
        // Check for input or condition to trigger the highlight
        if (Input.GetKeyDown(KeyCode.H))
        {
            HighlightAreaOnPlane();
        }
    }

    void HighlightAreaOnPlane()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        
        if (meshRenderer != null)
        {
            Material[] originalMaterials = meshRenderer.materials;
            Material[] newMaterials = new Material[originalMaterials.Length];

            for (int i = 0; i < originalMaterials.Length; i++)
            {
                newMaterials[i] = highlightMaterial;
            }

            meshRenderer.materials = newMaterials;
        }
        else
        {
            Debug.LogError("MeshRenderer component not found on the GameObject!");
        }
    }
}
