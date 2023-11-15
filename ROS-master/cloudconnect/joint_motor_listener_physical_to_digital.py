import rospy
import sys
import sys, select, os
from std_msgs.msg import String
from gazebo_msgs.srv import SetJointProperties
from sensor_msgs.msg import JointState
import socketio


sio = socketio.Client()

def joint_callback(data):
    print("entered callback")
    joint_state_dict= {
        'name' : data.name,
        'position' : data.position
    }
    sio.emit('arm_event',joint_state_dict, namespace='/arm_namespace')
    print ("data sent to socket io")

    joinState=JointState()
    joinState=data
    pub.publish(joinState)
turtlebot3_model = rospy.get_param("model", "waffle_pi")



rospy.init_node('joint_state_node')
#sio.connect('http://localhost:8000')
sio.connect('http://localhost:8000', namespaces=['/arm_namespace'])
pub = rospy.Publisher('/joint_states', JointState, queue_size=10 )
arm_sub = rospy.Subscriber('/joint_states', JointState, joint_callback)
rospy.spin()