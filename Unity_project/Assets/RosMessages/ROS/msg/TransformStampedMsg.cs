//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;

namespace RosMessageTypes.ROS
{
    [Serializable]
    public class TransformStampedMsg : Message
    {
        public const string k_RosMessageName = "ROS/TransformStamped";
        public override string RosMessageName => k_RosMessageName;

        //  This expresses a transform from coordinate frame header.frame_id
        //  to the coordinate frame child_frame_id
        // 
        //  This message is mostly used by the 
        //  <a href="http://wiki.ros.org/tf">tf</a> package. 
        //  See its documentation for more information.
        public HeaderMsg header;
        public string child_frame_id;
        //  the frame id of the child frame
        public TransformMsg transform;

        public TransformStampedMsg()
        {
            this.header = new HeaderMsg();
            this.child_frame_id = "";
            this.transform = new TransformMsg();
        }

        public TransformStampedMsg(HeaderMsg header, string child_frame_id, TransformMsg transform)
        {
            this.header = header;
            this.child_frame_id = child_frame_id;
            this.transform = transform;
        }

        public static TransformStampedMsg Deserialize(MessageDeserializer deserializer) => new TransformStampedMsg(deserializer);

        private TransformStampedMsg(MessageDeserializer deserializer)
        {
            this.header = HeaderMsg.Deserialize(deserializer);
            deserializer.Read(out this.child_frame_id);
            this.transform = TransformMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.child_frame_id);
            serializer.Write(this.transform);
        }

        public override string ToString()
        {
            return "TransformStampedMsg: " +
            "\nheader: " + header.ToString() +
            "\nchild_frame_id: " + child_frame_id.ToString() +
            "\ntransform: " + transform.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}