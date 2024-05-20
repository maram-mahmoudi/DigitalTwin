# Immersive Digital Twins for AGV Readme

## Overview

This readme provides instructions on how to connect Unity with ROS (Robot Operating System) using the DockerizedPrivateCloud_SDK and ROS-Unity connector. This allows you to leverage ROS capabilities within a Unity project, achieving Immersive experience and enhancing Human-Computer Interaction.

## Prerequisites

Before you begin, ensure you have the following installed:

- **Unity**: Latest version of Unity 3D.
- **ROS**: Latest version of ROS (preferably ROS Noetic for Ubuntu or ROS Melodic for older Ubuntu distributions).
- **ROS-Unity Connector**: Unity ROS package from the Unity Asset Store or GitHub.

## Step-by-Step Guide

### Step 1: Install ROS

1. Follow the [official ROS installation guide](http://wiki.ros.org/ROS/Installation) for your operating system. guide for your operating system.
2. Ensure that you can run `roscore` and other ROS nodes from your terminal.

### Step 2: Install Unity

1. Download and install Unity Hub from [Unity's official website](https://unity.com/).
2. Use Unity Hub to install the 2022 version of Unity Editor.

### Step 3: Set Up ROS-Unity Connector

1. **Download the ROS-Unity Connector**:
   - You can download it from the [Unity Asset Store](https://assetstore.unity.com/) or from the [official GitHub repository](https://github.com/Unity-Technologies/ROS-TCP-Connector).

2. **Import the Connector Package into Unity**:
   - Open Unity and create a new project.
   - Go to `Assets > Import Package > Custom Package` and import the ROS-Unity connector package you downloaded.

### Step 4: Configure ROS

1. **Install ROS Bridge**:
   - Install `rosbridge_suite` to facilitate communication between ROS and Unity. Use the following command:
     ```sh
     sudo apt-get install ros-noetic-rosbridge-server
     ```
   - Start `rosbridge` by running:
     ```sh
     roslaunch rosbridge_server rosbridge_websocket.launch
     ```

2. **Install ROS Unity Message Definitions**:
   - Clone the `ros_unity_msgs` repository and build the workspace:
     ```sh
     cd ~/catkin_ws/src
     git clone https://github.com/Unity-Technologies/ROS-TCP-Endpoint.git
     cd ~/catkin_ws
     catkin_make
     source devel/setup.bash
     ```

### Step 5: Configure Unity

1. **Set Up ROS Settings in Unity**:
   - In Unity, go to `Window > ROS Settings`.
   - Set the ROS IP Address to the IP of your ROS machine (or `localhost` if running on the same machine).
   - Set the ROS Port to `9090` (default for rosbridge).

2. **Add ROS Connection Script**:
   - Create an empty GameObject in your Unity scene.
   - Attach the `ROSConnection` script to the GameObject.
   - Configure the ROS settings in the `ROSConnection` script (e.g., IP Address, Port).

### Step 6: Send and Receive Messages

1. **Create ROS Publishers and Subscribers in Unity**:
   - Create scripts for ROS Publishers and Subscribers. Example for a Publisher:
     ```csharp
     using UnityEngine;
     using RosMessageTypes.Std;
     using Unity.Robotics.ROSTCPConnector;

     public class ROSPublisherExample : MonoBehaviour
     {
         ROSConnection ros;
         public string topicName = "unity_pub";

         void Start()
         {
             ros = ROSConnection.GetOrCreateInstance();
             ros.RegisterPublisher<StringMsg>(topicName);
         }

         void Update()
         {
             StringMsg msg = new StringMsg("Hello from Unity!");
             ros.Publish(topicName, msg);
         }
     }
     ```

2. **Running and Testing**:
   - Start `roscore` and `rosbridge_server` on your ROS machine.
   - Play the Unity scene to start sending/receiving ROS messages.
   - Monitor the communication in ROS using `rostopic echo /unity_pub`.

### Troubleshooting

- **Connection Issues**:
  - Verify the IP address and port settings in Unity match those of your ROS setup.
  - Ensure `rosbridge_server` is running without errors.

- **Message Errors**:
  - Ensure the message types used in Unity scripts match the ROS message definitions.

## Conclusion

By following this guide, you should be able to establish a connection between Unity and ROS using the ROS-Unity connector. This setup allows for the simulation and control of robotic applications within Unity, leveraging ROS's powerful middleware.

For more detailed information, refer to the official documentation of the ROS-Unity connector and ROS.
