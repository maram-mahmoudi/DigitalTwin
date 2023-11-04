import rospy
from geometry_msgs.msg import Twist
import sys, select, os
from std_msgs.msg import String
import time

import threading
import time


import socketio


def move(key):
    global pub
    pub.publish(key)                                         

sio = socketio.Client()


@sio.event(namespace='/arm')
def connect():
    print('[INFO] Successfully connected to server.')

@sio.event(namespace='/arm')
def connect_error():
    print('[INFO] Failed to connect to server.')

@sio.event(namespace='/arm')
def disconnect():
    print('[INFO] Disconnected from server.')

@sio.on('bottomPivot', namespace='/arm')
def bottomPivotData(data):
    print('Bottom Pivot Data Received!')
    global bottom_pivot_pub
    bottom_pivot_pub.publish(data)

@sio.on('middlePivot', namespace='/arm')
def middlePivotData(data):
    print('Middle Pivot Received!')
    global middle_pivot_pub
    middle_pivot_pub.publish(data)

@sio.on('upperPivot', namespace='/arm')
def upperPivotData(data):
    print('Upper Pivot Received!')
    global upper_pivot_pub
    upper_pivot_pub.publish(data)

@sio.on('bottomDeck', namespace='/arm')
def upperPivotData(data):
    print('Bottom Deck Data Received!')
    global bottom_deck_pub
    bottom_deck_pub.publish(data)

@sio.on('forkControl', namespace='/arm')
def forkControlData(data):
    print('Fork Control Data Received!')
    global fork_control_pub
    fork_control_pub.publish(data)    
    


# if __name__=="__main__": 
sio.connect(os.path.expandvars('http://$HOST_IP:8000'))
rospy.init_node('cloud_connect_move')
bottom_pivot_pub = rospy.Publisher('/bottom_pivot', String, queue_size=10)
middle_pivot_pub = rospy.Publisher('/middle_pivot', String, queue_size=10)
upper_pivot_pub  = rospy.Publisher('/upper_pivot', String, queue_size=10)

bottom_deck_pub  = rospy.Publisher('/bottom_deck', String, queue_size=10)

fork_control_pub  = rospy.Publisher('/fork_control', String, queue_size=10)






rospy.spin()

