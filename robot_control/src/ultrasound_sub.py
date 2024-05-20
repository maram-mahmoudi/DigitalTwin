#!/usr/bin/env python3

import rospy
from std_msgs.msg import Float32
from std_msgs.msg import Bool
from geometry_msgs.msg import Twist
from time import time
import socketio 
import time
from datetime import datetime
# from udp_com.msg import ultra_sonic

sio = socketio.Client()

interrupted = False
start_range = 2 
end_range = 10
send_timestamp = None


####### socket io to recieve distance
@sio.on('ultrasonicData',namespace='/ultrasonic')

def ultrasonicData(data):
    global interrupted
    if data is None:
        print("No data received from SocketIO")
    
    print("data from socket io: " + str(data))
    current_distance = data
    old_interrupt = interrupted
    if start_range <= current_distance <= end_range:  
        print("Robot interrupted")
        interrupted = True
        interrupt_pub.publish(interrupted)
        
        
    else:
        if interrupted:
            print("Robot resumed")
            interrupted = False
            interrupt_pub.publish(interrupted)
        # interrupt_pub.publish(interrupted)
    if old_interrupt != interrupted :
        # ultra_sonic_msg = ultra_sonic()
        # ultra_sonic_msg.interrupt = interrupted
        # ultra_sonic_msg.distance = current_distance
        ultra_sonic_unity_pub.publish(current_distance)
        print("maga")
    old_interrupt = interrupted
    # return interrupted
            

def send_data_with_timestamp(data):
    global send_timestamp
    send_timestamp = datetime.now().timestamp()
    #send_timestamp = time.time()
    #timestamp = time.time()  
    data_with_timestamp = {'data': interrupted, 'timestamp': send_timestamp} 
    sio.emit('ultrasonic_state_event', data_with_timestamp ,namespace='/ultrasonic_state_namespace')  


if __name__ == '__main__':
    rospy.init_node('ultrasonic_interrupt_node', anonymous=True)
    #subscriber = rospy.Subscriber("/ultrasonic_sensor", Float32, ultrasonicData)
    publisher = rospy.Publisher("/cmd_vel", Twist, queue_size=10)
    ultra_sonic_unity_pub = rospy.Publisher("/ultra_sonic_unity", Float32, queue_size=10)
    interrupt_pub = rospy.Publisher("/interrupt_state", Bool, queue_size=10)

    try:
        sio.connect('http://192.168.4.194:8000', namespaces=['/ultrasonic_state_namespace', '/ultrasonic'  ])
        while not rospy.is_shutdown():
            # print(interrupted)
            ## send state throught socket io to physical
            send_data_with_timestamp(interrupted)
            #sio.emit('ultrasonic_state_event',interrupted ,namespace='/ultrasonic_state_namespace')  
            rospy.sleep(1)
    except rospy.ROSInterruptException:
        pass
