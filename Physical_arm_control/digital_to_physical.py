#!/usr/bin/env python3
import RPi.GPIO as GPIO
import time
import rospy
from std_msgs.msg import String
from sensor_msgs.msg import JointState
import math


# Pin Definitions
BOTTOM_DECK_SERVO_PIN = 18  # GPIO PIN 18
FORK_CONTROL_SERVO_PIN = 15  # GPIO PIN 15
BOTTOM_PIVOT_SERVO_PIN = 24  # GPIO PIN 0
MIDDLE_PIVOT_SERVO_PIN = 14  # GPIO PIN 14
UPPER_PIVOT_SERVO_PIN = 23  # GPIO PIN 0


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
pwm_2 = GPIO.PWM(FORK_CONTROL_SERVO_PIN, 50)
pwm_3 = GPIO.PWM(BOTTOM_PIVOT_SERVO_PIN, 50)
pwm_4 = GPIO.PWM(MIDDLE_PIVOT_SERVO_PIN, 50)
pwm_5 = GPIO.PWM(UPPER_PIVOT_SERVO_PIN, 50)

pwm_1.start(0)
pwm_2.start(0)
pwm_3.start(0)
pwm_4.start(0)
pwm_5.start(0)

def set_servo_angle(angle, pwm):
    # The HS-422 servo has a range of 180 degrees
    # It is controlled with a pulse width between 1ms and 2ms
    # 1ms corresponds to 0 degrees, and 2ms to 180 degrees
    angle_degrees = math.degrees(angle)
    duty_cycle = (angle_degrees / 18.0) + 2
    duty_cycle = max(0, min(100, duty_cycle))
    pwm.ChangeDutyCycle(duty_cycle)
    

def initialize():
    duty_cycle = 2
    pwm_1.ChangeDutyCycle(duty_cycle)
    pwm_2.ChangeDutyCycle(duty_cycle)
    pwm_3.ChangeDutyCycle(duty_cycle)
    pwm_4.ChangeDutyCycle(duty_cycle)
    pwm_5.ChangeDutyCycle(duty_cycle)


#######################################################
def joint_state_callback(msg):
    print("inside callback")
    joint_positions = {
        'joint1': msg.position[2],
        'joint2': msg.position[3],
        'joint3': msg.position[4],
        'joint4': msg.position[5],
        'gripper': msg.position[0],
    }

    set_servo_angle( joint_positions['joint4'], pwm_5)
    set_servo_angle( joint_positions['joint2'], pwm_3)
    set_servo_angle(joint_positions['joint1'], pwm_1)
    set_servo_angle(joint_positions['gripper'], pwm_2)
    set_servo_angle(joint_positions['joint3'], pwm_4)

########################################################

try:
    print("hiiiiiiiiiii")
    rospy.init_node('arm_nodeee')
    initialize()
    rospy.Subscriber('/joint_states', JointState, joint_state_callback)
    rospy.spin()

except KeyboardInterrupt:
    # Clean up
    pwm_1.stop()
    pwm_2.stop()
    pwm_3.stop()
    pwm_4.stop()
    pwm_5.stop()
    GPIO.cleanup()

    
