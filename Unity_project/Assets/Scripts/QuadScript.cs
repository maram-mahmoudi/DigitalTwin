using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using System;

namespace RosSharp.Control{


public class QuadScript : MonoBehaviour
{
  Material mMaterial;
  MeshRenderer mMeshRenderer;
  float[] mPoints;
  int mHitCount;
  float mDelay;

  public string topicName = "/ultra_sonic_unity";
  public ROSConnection ros;
  public AGVController agvControllerInstance; 
  void Start()
  {

    mMeshRenderer = GetComponent<MeshRenderer>();
    mMaterial = mMeshRenderer.material;

    mPoints = new float[40 * 3]; 
    ros = ROSConnection.GetOrCreateInstance();
        //ros.Subscribe<Image>(topicName, ReceiveImage);
    ros.Subscribe<Float32Msg >(topicName, ReceiveMsg);
    //addHitPoint(0.0f, -0.7f);
    agvControllerInstance = FindObjectOfType<AGVController>();

  }
  void ReceiveMsg(Float32Msg msg)
  {
    
    if(msg.data<=20.0f){

       // red if less than 15 cm 
      addHitPoint(0.0f, -0.7f);
      Debug.Log("msg.data<=20");
      agvControllerInstance.obstacleFlag = true;


    }
    else {

      mHitCount=0;
      Debug.Log("msg.data is ok");
      agvControllerInstance.obstacleFlag = false;

    }
      
  }


  public void addHitPoint(float xp,float yp)
  {
    mPoints[mHitCount * 3] = xp;
    mPoints[mHitCount * 3 + 1] = yp;
    mPoints[mHitCount * 3 + 2] = 5.0f;

    mHitCount++;
    mHitCount %= 32;

    mMaterial.SetFloatArray("_Hits", mPoints);
    mMaterial.SetInt("_HitCount", mHitCount);

  }

}



}