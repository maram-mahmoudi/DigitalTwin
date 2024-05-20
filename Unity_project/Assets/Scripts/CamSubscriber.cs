using System;
using UnityEngine;
using UnityEngine.UI;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

public class CamSubscriber : MonoBehaviour
{
    public Material quadMaterial;
    Texture2D texRos;
    
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<CompressedImageMsg>("/cam", OnImageReceived);


    }

    void OnImageReceived(CompressedImageMsg img)

    {

        byte[] decompressedData = DecompressImage(img.data);


        if (decompressedData != null)
        {
            if (texRos == null )
            {
                // texRos = new Texture2D(1280, 720, TextureFormat.RGB24, false);
                 texRos = new Texture2D(200, 200, TextureFormat.RGB24, false);
            }

            
            texRos.LoadRawTextureData(decompressedData);
            texRos.Apply();
            //Debug.Log("image message" + decompressedData);
            quadMaterial.mainTexture = texRos;
        }
    }

    void BgrToRgb(byte[] data)
    {
        //BRG: Blue, Green, Red,Blue, Green, Red, Blue, Green, Red, 
        //RGB: Red, Green, Blue, Red, Green, Blue, Red, Green, Blue, 
        // so we just loop over the bytes and swap R with B
        for (int i = 0; i < data.Length; i += 3)
        {
            byte dummy = data[i];
            data[i] = data[i + 2];
            data[i + 2] = dummy;
        }
    }


    byte[] DecompressImage(byte[] compressedData)
    {
        try
        {
            // Create a new Texture2D
            Texture2D texture = new Texture2D(2, 2);

            // Load the compressed data into the Texture2D
            texture.LoadImage(compressedData);

            // Convert the Texture2D to a byte array
            byte[] rawData = texture.GetRawTextureData();

            // Return the byte array
            return rawData;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error decompressing image: {e.Message}");
            return null;
        }
    }



}
