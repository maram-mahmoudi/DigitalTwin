#!/usr/bin/env python3

import rospy
import RPi.GPIO as GPIO
from gazebo_msgs.srv import SetJointProperties
from sensor_msgs.msg import JointState
import sys, select, os
from gazebo_msgs.srv import SetModelState
from std_msgs.msg import String
import socketio
import time
import math

sio = socketio.Client()

# Pin Definitions
BOTTOM_DECK_SERVO_PIN = 5  # GPIO PIN 5
BOTTOM_PIVOT_SERVO_PIN = 19  # GPIO PIN 19
MIDDLE_PIVOT_SERVO_PIN = 14  # GPIO PIN 21
UPPER_PIVOT_SERVO_PIN = 21  # GPIO PIN 14
FORK_CONTROL_SERVO_PIN = 8  # GPIO PIN 15

# Use 'GPIO naming'
GPIO.setmode(GPIO.BCM)

# Set pins as outputs
GPIO.setup(BOTTOM_DECK_SERVO_PIN, GPIO.OUT)
GPIO.setup(FORK_CONTROL_SERVO_PIN, GPIO.OUT)
GPIO.setup(BOTTOM_PIVOT_SERVO_PIN, GPIO.OUT)
GPIO.setup(MIDDLE_PIVOT_SERVO_PIN, GPIO.OUT)
GPIO.setup(UPPER_PIVOT_SERVO_PIN, GPIO.OUT)

# Set frequency to 50Hz, good for servos.
pwm_1 = GPIO.PWM(BOTTOM_DECK_SERVO_PIN, 50)
pwm_2 = GPIO.PWM(BOTTOM_PIVOT_SERVO_PIN, 50)
pwm_3 = GPIO.PWM(MIDDLE_PIVOT_SERVO_PIN, 50)
pwm_4 = GPIO.PWM(UPPER_PIVOT_SERVO_PIN, 50)
pwm_5 = GPIO.PWM(FORK_CONTROL_SERVO_PIN, 50)




def set_servo_angle(angle, pwm):
    
    angle_degrees = math.degrees(angle)
    print(f'''Pwm: {pwm} Angle: {angle_degrees}''')
    '''
    
    180     ->     133
    
    angle   ->     x
    
    x = (angle*133)/180
    
    '''
    
    angle_degrees = (angle_degrees*133)/180
    angle_degress +=90
    duty_cycle = (angle_degrees / 18.0) + 2
    duty_cycle = max(0, min(100, duty_cycle))
    pwm.ChangeDutyCycle(duty_cycle)
def initialize():
    pwm_1.start(0)
    pwm_2.start(0)
    pwm_3.start(0)
    pwm_4.start(0)
    pwm_5.start(0)
    duty_cycle = (66/18) + 2
    pwm_1.ChangeDutyCycle(duty_cycle)
    pwm_2.ChangeDutyCycle(duty_cycle)
    pwm_3.ChangeDutyCycle((133/18)+2) 
    pwm_4.ChangeDutyCycle(duty_cycle)
    
    #Fork
    pwm_5.ChangeDutyCycle(duty_cycle)
  



def set_joint_state(msg): 
    print("inside callback")
    joint_positions = {
        'joint1': msg['position_joint1'],
        'joint2': msg['position_joint2'],
        'joint3': (3.14 - (float(msg['position_joint3'])+1.57))-1.57,
        'joint4': msg['position_joint4'],
        'gripper': msg['position_gripper'],
    }
    set_servo_angle(joint_positions['joint1'], pwm_1)
    set_servo_angle(joint_positions['joint2'], pwm_2)
    set_servo_angle(joint_positions['joint3'], pwm_3)
    set_servo_angle(joint_positions['joint4'], pwm_4)
    
    # gripper
    set_servo_angle(joint_positions['gripper'], pwm_5)
      
@sio.on('armData',namespace='/arm_control')
def armData(data):

    print('Received Arm Data')
    set_joint_state(data)
    




if __name__ == '__main__':
    try:
        initialize()
        #rospy.init_node('arm_control_node')
        #GAZEBO_API_URL= "http://$HOST_IP:11346"
        # sio.connect(os.path.expandvars('http://$HOST_IP:8000'))
        #rospy.spin()
        while(1):
            duty_cycle = (133/18) +2
            pwm_3.ChangeDutyCycle(duty_cycle)


    except KeyboardInterrupt:
        # Clean up
        print('Failure')
        pwm_1.stop()
        pwm_2.stop()
        pwm_3.stop()
        pwm_4.stop()
        pwm_5.stop()
        GPIO.cleanup()
