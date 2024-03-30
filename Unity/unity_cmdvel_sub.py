#!/usr/bin/env python

import rospy
from geometry_msgs.msg import Twist
import socketio

sio = socketio.Client()

cmd_vel_msg = None

def cmd_vel_callback(data):
    global cmd_vel_msg
    
    cmd_vel_msg = {
        'linear': {
            'x': data.linear.x,
            'y': data.linear.y,
            'z': data.linear.z
        },
        'angular': {
            'x': data.angular.x,
            'y': data.angular.y,
            'z': data.angular.z
        }
    }

    
    rospy.loginfo("Received cmd_vel message:\n{}".format(data))
    ## send cmdvel throught socket io to physical
    sio.emit('unity_cmdvel_event', cmd_vel_msg ,namespace='/unity_cmdvel_namespace') 



if __name__ == '__main__':
    rospy.init_node('Unity_cmd_vel_subscriber', anonymous=True)
    subscriber = rospy.Subscriber("/cmd_vel", Twist, cmd_vel_callback)
    try:
        sio.connect('http://192.168.23.77:8000', namespaces=['/unity_cmdvel_namespace'])
        while not rospy.is_shutdown():
            rospy.sleep(1)
        
    except rospy.ROSInterruptException:
        pass

