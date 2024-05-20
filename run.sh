#!/bin/bash

# Terminal 1: DigitalTwin/RemoteDrivingDashboard-master
gnome-terminal -- bash -c "cd DigitalTwin/RemoteDrivingDashboard-master; docker-compose up; exec bash"

# Terminal 2: DigitalTwin/OTA_RemoteDrivingConfigurator-main/Designs
gnome-terminal -- bash -c "export HOST_IP=localhost; cd ~/DigitalTwin/OTA_RemoteDrivingConfigurator-main/Designs; python3 QtGUI.py; exec bash"

# Terminal 3: ROS Core
gnome-terminal -- bash -c "roscore; exec bash"

# Terminal 4: ROS Actuate
gnome-terminal -- bash -c "export HOST_IP=localhost; cd ~/DigitalTwin/ROS-master/actuate/src; python3 actuate.py; exec bash"

# Terminal 5: Gazebo Actuate
gnome-terminal -- bash -c "export HOST_IP=localhost; cd ~/DigitalTwin/ROS-master/actuate; python3 gazebo_actuate.py; exec bash"

# Terminal 6: Ultrasound Sub
gnome-terminal -- bash -c "cd ~/catkin_ws; source devel/setup.bash; cd ~/catkin_ws/src/robot_control/src; python3 ultrasound_sub.py; exec bash"

# Terminal 7: Turtlebot3 Gazebo
gnome-terminal -- bash -c "cd ~/catkin_ws; source devel/setup.bash; export TURTLEBOT3_MODEL=waffle_pi; roslaunch turtlebot3_gazebo turtlebot3_house.launch; exec bash"

