#!/usr/bin/env python3

import rospy
from gazebo_msgs.srv import SetJointProperties
from sensor_msgs.msg import JointState
import sys, select, os
from gazebo_msgs.srv import SetModelState
from std_msgs.msg import String
import socketio


sio = socketio.Client()
GAZEBO_API_URL= "http://$HOST_IP:11346"

def set_joint_state(msg): 
    rospy.wait_for_service('gazebo_msgs/SetJointProperties')
    try:
        joint_state = rospy.ServiceProxy('gazebo_msgs/SetJointProperties', SetJointProperties)

        joint_state = SetJointProperties()
        joint_state.joint_name = msg['name']
        joint_state.position = msg['position']

        response = set_joint_state(joint_state)

        if response.success:
            rospy.loginfo(f"Successfully set state for {joint_state.joint_name}")
        else:
            rospy.logerr(f"Failed to set state for {joint_state.joint_name}: {response.status_message}")

    except rospy.ServiceException as e:
        rospy.logerr(f"Service call failed: {e}")
        

      
@sio.on('armData',namespace='/arm_control')
def armData(data):
    print("HI!!")
    set_joint_state(data)
    print("Data received from socketio")




if __name__ == '__main__':
    rospy.init_node('arm_control_node')
    sio.connect('http://0.0.0.0:8000')
    rospy.spin()
