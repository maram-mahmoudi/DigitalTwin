using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using System;


public class DistanceSubscriber : MonoBehaviour
{
    public string topicName = "/ultra_sonic_unity";
    public ROSConnection ros;


    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        //ros.Subscribe<Image>(topicName, ReceiveImage);
        ros.Subscribe<Float32Msg >(topicName, ReceiveImage);
    }

    void ReceiveImage(Float32Msg  msg)
    {
         Debug.Log($"Distance: " +msg.data);
        // Convert the ROS Image message to a Unity Texture2D
        // imageTexture.LoadRawTextureData(imageMsg.data);
        // imageTexture.Apply();

        // // Use the imageTexture in your Unity project as needed
        // // For example, you can set it as the texture of a GameObject's material
        // GetComponent<Renderer>().material.mainTexture = imageTexture;
    }

}