#!/usr/bin/env python3

from __future__ import division
import time
import Adafruit_PCA9685
import rospy
from gazebo_msgs.srv import SetJointProperties
from sensor_msgs.msg import JointState
import sys, select, os
from std_msgs.msg import String
import socketio
import time
import math
from adafruit_servokit import ServoKit



kit = ServoKit(channels=16)

sio = socketio.Client() 

# Channels Definitions
BOTTOM_DECK_SERVO_CHANNEL = 0  # PCA9685 CHANNEL 0
BOTTOM_PIVOT_SERVO_CHANNEL = 1  # PCA9685 CHANNEL 1
MIDDLE_PIVOT_SERVO_CHANNEL = 2   # PCA9685 CHANNEL 2
UPPER_PIVOT_SERVO_CHANNEL =  3 # PCA9685 CHANNEL 3 
FORK_CONTROL_SERVO_CHANNEL = 4  # PCA9685 CHANNEL 4 


def set_servo_angle(angle, channel):
    
    angle_degrees = math.degrees(angle)
    print(f'''Pwm: {channel} Angle: {angle_degrees}''')

   # angle_degrees = (angle_degrees * 133) / 180
    if channel == 4: 
        angle_cal =angle_degrees * 180 / 1.08
        angle_degrees = max(0, min(180, angle_cal ))
    else: 
        angle_degrees += 90
    
    kit.servo[channel].angle = angle_degrees
   


def initialize():
    # Set the servos to their initial positions
    set_servo_angle(0, BOTTOM_DECK_SERVO_CHANNEL)
    set_servo_angle(0, BOTTOM_PIVOT_SERVO_CHANNEL)
    set_servo_angle(0, MIDDLE_PIVOT_SERVO_CHANNEL)
    set_servo_angle(0, UPPER_PIVOT_SERVO_CHANNEL)

    set_servo_angle(0, FORK_CONTROL_SERVO_CHANNEL)


def set_joint_state(msg):
    print("inside callback")
    joint_positions = {
        'joint1': msg['position_joint1'],
        'joint2': msg['position_joint2'],
        'joint3': (3.14 - (float(msg['position_joint3']) + 1.57)) - 1.57,
        'joint4': msg['position_joint4'],

        'gripper': msg['position_gripper'],
    }
    set_servo_angle(joint_positions['joint1'], BOTTOM_DECK_SERVO_CHANNEL)
    set_servo_angle(joint_positions['joint2'], BOTTOM_PIVOT_SERVO_CHANNEL)
    set_servo_angle(joint_positions['joint3'], MIDDLE_PIVOT_SERVO_CHANNEL)
    set_servo_angle(joint_positions['joint4'], UPPER_PIVOT_SERVO_CHANNEL)

    set_servo_angle(joint_positions['gripper'], FORK_CONTROL_SERVO_CHANNEL)

@sio.on('armData',namespace='/arm_control')
def armData(data):

    print('Received Arm Data')
    set_joint_state(data)


if __name__ == '__main__':
    try:
        initialize()
        rospy.init_node('arm_control_node')
        GAZEBO_API_URL= "http://$HOST_IP:11346"
        sio.connect(os.path.expandvars('http://$HOST_IP:8000'))
        rospy.spin()
        # while True:
        #     # Your main control loop here
        #     time.sleep(1)  

    except KeyboardInterrupt:
        # Clean up
        print('Failure')
        pwm.set_all_pwm(0, 0)  # Turn off all PWM channels


