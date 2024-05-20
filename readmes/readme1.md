# Launching The Dockerized Private Cloud

## Overview
This project involves controlling a TurtleBot3 robot and its components using Unity, leveraging ROS (Robot Operating System) and various scripts. The setup is done on Ubuntu 20.04. Below are the steps to set up and run the project.

## Prerequisites
- Ubuntu 20.04
- ROS Noetic
- Docker
- A TurtleBot3 robot with OpenManipulator

## Setup Instructions

### 1. Create a Catkin Workspace
Follow the [official tutorial](http://wiki.ros.org/catkin/Tutorials/create_a_workspace) to create a catkin workspace.

### 2. Clone Repositories
Clone the following repositories into your catkin workspace:
- [ROS TCP Endpoint](https://github.com/Unity-Technologies/ROS-TCP-Endpoint)

Follow the instructions [here](https://emanual.robotis.com/docs/en/platform/turtlebot3/manipulation/#turtlebot3-with-openmanipulator) to install the OpenManipulator dependencies.

## Running the Project

### Common Steps
The following steps must be performed for all functionalities:

#### On the PC:
```sh
cd DigitalTwin/RemoteDrivingDashboard-master
docker-compose up
```

### On the Turtlebot:
``` sh
export TURTLEBOT3_MODEL=${TB3_MODEL}
roslaunch turtlebot3_bringup turtlebot3_robot.launch
```

## Now if you want to control the Arm using unity do the following: 

#### On the PC: 
```
python3 cd ~/DigitalTwin/ROS-master/cloudconnect/python3 joint_motor_digital_to_physical.py
—
cd catkin_ws
source devel/setup.bash 
roslaunch ros_tcp_endpoint endpoint.launch
—-
```
### On the Turtlebot: 
```
export HOST_IP= #put the IP of the PC
cd ~/DigitalTwin/Physical_arm_control
sudo chmod a+rw /dev/i2c-*
python3 servo_driver_move_arm.py
```

## If you want to control the AGV using unity, do the following: 
### On the PC:
```
export HOST_IP= #put the IP of the PC
python3 cd Unity/physical_unity_cmdvel.py
```
### On the Turtlebot: 
```
export HOST_IP= #put the IP of the PC
python3 cd Unity/physical_unity_cmdvel.py
```

## If you want to enable the camera streaming do the following: 

### On the PC: 
```
python3 cd DigitalTwin/Unity/cam_to_unity.py
```
### On the turtlebot: 
```
export HOST_IP= #put the IP of the PC
python3 cd Unity/cam_physical.py
```

## Finally to use the ultrasonic, either for the heat map or to solve the digital shadowing, do the following: 

### On the turtlebot: 
```
sudo groupadd gpio
sudo usermod -a -G gpio ubuntu
sudo grep gpio /etc/group
sudo chown root.gpio /dev/gpiomem
sudo chmod g+rw /dev/gpiomem
python3 distance2.py
```
 
### On the PC: 
```
python3 cd DigitalTwin/robot_control/src/ultrasound_sub.py
```
