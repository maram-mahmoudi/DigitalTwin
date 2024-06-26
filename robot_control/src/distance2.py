#!/usr/bin/env python3

import RPi.GPIO as GPIO
import time
import rospy
from std_msgs.msg import Float32 
import socketio

sio = socketio.Client()


GPIO.setmode(GPIO.BCM)

TRIG_PIN = 17
ECHO_PIN = 14

GPIO.setup(TRIG_PIN, GPIO.OUT)
GPIO.setup(ECHO_PIN, GPIO.IN)

def get_distance():
    GPIO.output(TRIG_PIN, True)
    time.sleep(0.00001)
    GPIO.output(TRIG_PIN, False)

    start_time = time.time()
    stop_time = time.time()

    while GPIO.input(ECHO_PIN) == 0:
        start_time = time.time()

    while GPIO.input(ECHO_PIN) == 1:
        stop_time = time.time()

    elapsed_time = stop_time - start_time
    distance = (elapsed_time * 34300) / 2

    return distance



if __name__ == "__main__":
    sio.connect('http://192.168.105.194:8000', namespaces=['/ultrasonic_namespace'])
    rospy.init_node('ultrasonic_publisher')
    pub = rospy.Publisher('/ultrasonic_sensor', Float32, queue_size=10)
    try:
        while not rospy.is_shutdown():
            distance = get_distance()
            print("Distance: {:.2f} cm".format(distance))
            sio.emit('ultrasonic_event',distance ,namespace='/ultrasonic_namespace')
            pub.publish(distance)
            time.sleep(1)
            
    except KeyboardInterrupt:
        GPIO.cleanup()

