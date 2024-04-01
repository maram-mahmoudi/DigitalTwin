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

MINIMUM_PULSE_WIDTH = 500
MAXIMUM_PULSE_WIDTH=2500

MIDDLE_PIVOT_SERVO_PIN = 14  # GPIO PIN 14
# Use 'GPIO naming'
GPIO.setmode(GPIO.BCM)
GPIO.setup(MIDDLE_PIVOT_SERVO_PIN, GPIO.OUT)

pwm_4 = GPIO.PWM(MIDDLE_PIVOT_SERVO_PIN, 50)
pwm_4.start(0)

pulse_width = MINIMUM_PULSE_WIDTH + ((90-0)/180)*(MAXIMUM_PULSE_WIDTH-MINIMUM_PULSE_WIDTH)
duty_cycle = (pulse_width/20000)*100

pwm_4.ChangeDutyCycle(duty_cycle)

while(1):
    pwm_4.ChangeDutyCycle(duty_cycle)