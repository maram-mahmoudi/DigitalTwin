import rospy
import sys, select, os
from std_msgs.msg import String
from gazebo_msgs.srv import SetJointProperties
from sensor_msgs.msg import JointState
import socketio
import time
import datetime


sio = socketio.Client()
counter = 0

def joint_callback(data):

    global counter
    try:
        current_datetime = datetime.datetime.now()
        mills =  current_datetime.minute*60*1000 + current_datetime.second*1000+ current_datetime.microsecond//1000
        print(f"{data.header.stamp.nsecs} --- {mills}")

        # Calculate delay
       

        joint_state_positions= {
            'name' : data.name,
            'position_joint1' : data.position[2],
            'position_joint2' : data.position[3],
            'position_joint3' : data.position[4],
            'position_joint4' : data.position[5],
            'position_gripper': data.position[0],
            'time' : data.header.stamp.nsecs,
        }
        # Avoiding high throughput
        counter += 1
        if (counter%50) == 0:
            sio.emit('arm_event',joint_state_positions, namespace='/arm_namespace')
            print ("data sent to socket io")
    except:
        print(f'''Ignoring Wheel State''')
   
    

    # joinState=JointState()
    # joinState=data
    #pub.publish(joinState)
turtlebot3_model = rospy.get_param("model", "waffle_pi")



rospy.init_node('joint_state_node')
#sio.connect('http://localhost:8000')
sio.connect('http://localhost:8000', namespaces=['/arm_namespace'])
#pub = rospy.Publisher('/joint_states', JointState, queue_size=10 )
arm_sub = rospy.Subscriber('/joint_states', JointState, joint_callback)
rospy.spin()