//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.ROS
{
    [Serializable]
    public class Pose2DMsg : Message
    {
        public const string k_RosMessageName = "ROS/Pose2D";
        public override string RosMessageName => k_RosMessageName;

        //  Deprecated
        //  Please use the full 3D pose.
        //  In general our recommendation is to use a full 3D representation of everything and for 2D specific applications make the appropriate projections into the plane for their calculations but optimally will preserve the 3D information during processing.
        //  If we have parallel copies of 2D datatypes every UI and other pipeline will end up needing to have dual interfaces to plot everything. And you will end up with not being able to use 3D tools for 2D use cases even if they're completely valid, as you'd have to reimplement it with different inputs and outputs. It's not particularly hard to plot the 2D pose or compute the yaw error for the Pose message and there are already tools and libraries that can do this for you.
        //  This expresses a position and orientation on a 2D manifold.
        public double x;
        public double y;
        public double theta;

        public Pose2DMsg()
        {
            this.x = 0.0;
            this.y = 0.0;
            this.theta = 0.0;
        }

        public Pose2DMsg(double x, double y, double theta)
        {
            this.x = x;
            this.y = y;
            this.theta = theta;
        }

        public static Pose2DMsg Deserialize(MessageDeserializer deserializer) => new Pose2DMsg(deserializer);

        private Pose2DMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.x);
            deserializer.Read(out this.y);
            deserializer.Read(out this.theta);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.x);
            serializer.Write(this.y);
            serializer.Write(this.theta);
        }

        public override string ToString()
        {
            return "Pose2DMsg: " +
            "\nx: " + x.ToString() +
            "\ny: " + y.ToString() +
            "\ntheta: " + theta.ToString();
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