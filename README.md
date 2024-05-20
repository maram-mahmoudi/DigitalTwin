# Immersive Digital Twins for AGV Readme

## Overview

This readme provides instructions on how to connect Unity with ROS (Robot Operating System) using the DockerizedPrivateCloud_SDK and ROS-Unity connector. This allows you to leverage ROS capabilities within a Unity project, achieving Immersive experience and enhancing Human-Computer Interaction.

## Prerequisites

Before you begin, ensure you have the following installed:

- **Unity**: Latest version of Unity 3D.
- **ROS**: Latest version of ROS (preferably ROS Noetic for Ubuntu or ROS Melodic for older Ubuntu distributions).
- **Dockerized Private Cloud**: [Run the Dockerized Private Cloud services and the physical asset using the instructions in this readme file. ](https://github.com/maram-mahmoudi/DigitalTwin/blob/f7edef0c66dbbc5ae361a9f8bb4d7ba4fe2c9ebc/readmes/README.md)
- **ROS-Unity Connector**: Unity ROS package from the Unity Asset Store or GitHub.

## Step-by-Step Guide

### Step 1: Install ROS

1. Follow the [official ROS installation guide](http://wiki.ros.org/ROS/Installation) for your operating system. guide for your operating system.
2. Ensure you can run `roscore` and other ROS nodes from your terminal.

### Step 2: Install Unity

1. Download and install Unity Hub from [Unity's official website](https://unity.com/).
2. Use Unity Hub to install the 2022 version of Unity Editor.

### Step 3: Set Up ROS-Unity Connector

1. Use the [official Unity Robotics Hub guide](https://github.com/Unity-Technologies/ROS-TCP-Connector)


### Step 4: Use the SDK to generate the files that you want to subscribe/ publish data from/ to Unity 


1. Clone this repository to your local machine:

    ```bash
    git clone https://github.com/MokhtarBaWahal/DockerizedPrivateCloud_SDK.git
    ```

2. Install dependencies using `pip`:

    ```bash
    pip install -r requirements.txt
    ```


3. Modify the `config.yaml` file to specify the ROS node configuration, including topics to publish and subscribe, middleware flags, and Unity integration flags.

    ```yaml
    ros_node:
      name: test
      topics:
        publish:
          topic_1:
            name: "cmd_vel"
            type:  "geometry_msgs/Twist"
            middleware_flag: 1
            unity_sub_flag: 1
            unity_pub_flag: 1
        subscribe:
          topic_1:
            name: "control"
            type:  "std_msgs/String"
            middleware_flag: 1
            unity_sub_flag: 0
            unity_pub_flag: 0
          topic_2:
            name: "metric"
            type:  "std_msgs/String"
            middleware_flag: 1
            unity_sub_flag: 0
            unity_pub_flag: 0
          topic_3: 
            name: "cam"
            type:  "sensor_msgs/CompressedImage"
            middleware_flag: 1
            unity_sub_flag: 1
            unity_pub_flag: 0
          topic_4: 
            name: "ultra_sonic_unity"
            type:  "std_msgs/Float32"
            middleware_flag: 0
            unity_sub_flag: 1
            unity_pub_flag: 0
    ```

4. Run the SDK with the configured settings:

    ```bash
    python setup.py
    ```
5. Files will be generated, copy the scripts folder located in the unity level folder to your project in unity under the Assets folder.
6. Implement the callbacks.
8. Attach the ready scripts to game objects,  for Example: the AGV controller script assign it to the AGV Gameobject.
9. Launch the endpoint in ROS
     ```bash
   source devel/setup.bash
     roslaunch ros_tcp_endpoint endpoint.launch
    ```
10. Connect Unity to ros through the ROS Settings in Unity
11. Run and have fun.

### Troubleshooting

- **Connection Issues**:
  - Verify the IP address and port settings in Unity match those of your ROS setup.
  - Ensure `ros_tcp endpoint` is running without errors.

- **Message Errors**:
  - Ensure the message types used in Unity scripts match the ROS message definitions.


