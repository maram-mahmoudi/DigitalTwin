using UnityEngine;
using  Unity.XR.CoreUtils;

public class QuadSetting : MonoBehaviour
{
    public MeshRenderer Quad;

    public void Start()
    {
        Quad.enabled = true;
    }


    public void Gesture(string msg)
    {
        // Toggle the visibility of the quad
        if (msg=="stop") Quad.enabled = false;
        else if (msg=="start") Quad.enabled = true;
      
    }
}
