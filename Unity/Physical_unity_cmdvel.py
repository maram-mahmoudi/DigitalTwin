#!/usr/bin/env python

import rospy
from geometry_msgs.msg import Twist
import socketio

sio = socketio.Client()

rospy.init_node('physical_cmd_vel_from_unity', anonymous=True)
pub = rospy.Publisher('/cmd_vel', Twist, queue_size=10)


### receiving cmd_vel
@sio.on('unity_cmdvel_callback',namespace='/unity_cmdvel')

def unity_cmdvel_callback(data):
    twist = Twist()

    twist.linear.x = data['linear']['x']
    twist.linear.y = data['linear']['y']
    twist.linear.z = data['linear']['z']
    twist.angular.x = data['angular']['x']
    twist.angular.y = data['angular']['y']
    twist.angular.z = data['angular']['z']
    pub.publish(twist)




sio.connect('http://192.168.4.194:8000', namespaces=['/unity_cmdvel'])
rospy.spin()