
import rospy
import sys
from geometry_msgs.msg import Twist
import sys, select, os
from std_msgs.msg import String
from nav_msgs.msg import Odometry
import socketio
###
from std_msgs.msg import Bool
from datetime import datetime

#from AUC-Thesis-DT-Physical/RemoteDrivingDashboard-master/apps/home/views.py import sio


sio = socketio.Client()


BURGER_MAX_LIN_VEL = 5.0
BURGER_MAX_ANG_VEL = 2.84

WAFFLE_MAX_LIN_VEL = 0.26
WAFFLE_MAX_ANG_VEL = 1.82

LIN_VEL_STEP_SIZE = 0.01
ANG_VEL_STEP_SIZE = 0.1

####
interrupted = False

def makeSimpleProfile(output, input, slop):
    if input > output:
        output = min( input, output + slop )
    elif input < output:
        output = max( input, output - slop )
    else:
        output = input

    return output

def vels(target_linear_vel, target_angular_vel):
    return "currently:\tlinear vel %s\t angular vel %s " % (target_linear_vel,target_angular_vel)

def constrain(input, low, high):
    if input < low:
      input = low
    elif input > high:
      input = high
    else:
      input = input

    return input

def checkLinearLimitVelocity(vel):
    if turtlebot3_model == "burger":
      vel = constrain(vel, -BURGER_MAX_LIN_VEL, BURGER_MAX_LIN_VEL)
    elif turtlebot3_model == "waffle" or turtlebot3_model == "waffle_pi":
      vel = constrain(vel, -WAFFLE_MAX_LIN_VEL, WAFFLE_MAX_LIN_VEL)
    else:
      vel = constrain(vel, -BURGER_MAX_LIN_VEL, BURGER_MAX_LIN_VEL)

    return vel

def checkAngularLimitVelocity(vel):
    if turtlebot3_model == "burger":
      vel = constrain(vel, -BURGER_MAX_ANG_VEL, BURGER_MAX_ANG_VEL)
    elif turtlebot3_model == "waffle" or turtlebot3_model == "waffle_pi":
      vel = constrain(vel, -WAFFLE_MAX_ANG_VEL, WAFFLE_MAX_ANG_VEL)
    else:
      vel = constrain(vel, -BURGER_MAX_ANG_VEL, BURGER_MAX_ANG_VEL)

    return vel



# if __name__=="__main__": 


status = 0
target_linear_vel   = 0.0
target_angular_vel  = 0.0
control_linear_vel  = 0.0
control_angular_vel = 0.0

def c_move(key):
    global status, target_linear_vel, target_angular_vel, control_angular_vel, control_linear_vel
    twist = Twist()
    control_linear_vel = makeSimpleProfile(control_linear_vel, target_linear_vel, (LIN_VEL_STEP_SIZE/2.0))
    twist.linear.x = control_linear_vel; twist.linear.y = 0.0; twist.linear.z = 0.0

    control_angular_vel = makeSimpleProfile(control_angular_vel, target_angular_vel, (ANG_VEL_STEP_SIZE/2.0))
    twist.angular.x = 0.0; twist.angular.y = 0.0; twist.angular.z = control_angular_vel  
    pub.publish(twist)

def move(key):  
    #timestamp here ----------------------------
    #global target_linear_vel , target_angular_vel
    global pub
    try:
        # print(msg)
        # while not rospy.is_shutdown():
        global status, target_linear_vel, target_angular_vel, control_angular_vel, control_linear_vel
        if key == 'w' :
            print(key)
            target_linear_vel = checkLinearLimitVelocity(target_linear_vel + LIN_VEL_STEP_SIZE)
            status = status + 1
            print(vels(target_linear_vel,target_angular_vel))
        elif key == 'x' :
            target_linear_vel = checkLinearLimitVelocity(target_linear_vel - LIN_VEL_STEP_SIZE)
            status = status + 1
            print(vels(target_linear_vel,target_angular_vel))
        elif key == 'a' :
            target_angular_vel = checkAngularLimitVelocity(target_angular_vel + ANG_VEL_STEP_SIZE)
            status = status + 1
            print(vels(target_linear_vel,target_angular_vel))
        elif key == 'd' :
            target_angular_vel = checkAngularLimitVelocity(target_angular_vel - ANG_VEL_STEP_SIZE)
            status = status + 1
            print(vels(target_linear_vel,target_angular_vel))
        elif key == ' ' or key == 's' :
            target_linear_vel   = 0.0
            control_linear_vel  = 0.0
            target_angular_vel  = 0.0
            control_angular_vel = 0.0
            print(vels(target_linear_vel, target_angular_vel))
        # else:
            # if (key == '\x03'):
                # break
        key = ''
        if status == 20 :
            # print(msg)
            status = 0

        twist = Twist()
        control_linear_vel = makeSimpleProfile(control_linear_vel, target_linear_vel, (LIN_VEL_STEP_SIZE/2.0))
        twist.linear.x = control_linear_vel; twist.linear.y = 0.0; twist.linear.z = 0.0

        control_angular_vel = makeSimpleProfile(control_angular_vel, target_angular_vel, (ANG_VEL_STEP_SIZE/2.0))
        twist.angular.x = 0.0; twist.angular.y = 0.0; twist.angular.z = control_angular_vel
        #sio.emit('odom_data', odom_dict)
       # print(twist)
        pub.publish(twist)

    except:
        print("Exception")

    

    # finally:
    #     print("finally")
    #     twist = Twist()
    #     twist.linear.x = 0.0; twist.linear.y = 0.0; twist.linear.z = 0.0
    #     twist.angular.x = 0.0; twist.angular.y = 0.0; twist.angular.z = 0.0
    #     pub.publish(twist)

def key_callback(msg):
    msg = str(msg)[-2]
    move(msg)


# #just added this function
# def odom_callback(odom_data):
#     # Extract linear and angular velocities from the odom topic
#     linear_velocity = odom_data.twist.twist.linear
#     angular_velocity = odom_data.twist.twist.angular
    
#     # Create a Twist message and populate it
#     twist = Twist()
#     twist.linear.x = checkLinearLimitVelocity(linear_velocity.x)
#     twist.linear.y = checkLinearLimitVelocity(linear_velocity.y)
#     twist.linear.z = checkLinearLimitVelocity(linear_velocity.z)
#     twist.angular.x = checkAngularLimitVelocity(angular_velocity.x)
#     twist.angular.y = checkAngularLimitVelocity(angular_velocity.y)
#     twist.angular.z = checkAngularLimitVelocity(angular_velocity.z)

#     # Publish the Twist message
#     pub.publish(twist)

####### socket io to receive interrupt state 
@sio.on('interrupt_callback',namespace='/interruption_state')


def interrupt_callback(data):
    global interrupted
    received_timestamp = datetime.now().timestamp()
    sent_timestamp = data['timestamp']
    data = data['data']
    print(f"Received interrupt state at timestamp {received_timestamp}: {data}")
    interrupted = data
    print('getting data from socket io')
    print(data)
    delay = received_timestamp - sent_timestamp
    print(f"Delay since last data sent: {delay} seconds")
   
   

def odom_callback(odom_data):

    ###
    global interrupted
    global target_linear_vel , target_angular_vel
    # Extract linear and angular velocities from the odom topic
    linear_velocity = odom_data.twist.twist.linear
    angular_velocity = odom_data.twist.twist.angular
    
    # Convert the Odometry data to a dictionary
    odom_dict = {
        'position': {
            'x': odom_data.pose.pose.position.x,
            'y': odom_data.pose.pose.position.y,
            'z': odom_data.pose.pose.position.z
        },
        'orientation': {
            'x': odom_data.pose.pose.orientation.x,
            'y': odom_data.pose.pose.orientation.y,
            'z': odom_data.pose.pose.orientation.z,
            'w': odom_data.pose.pose.orientation.w
        },
        'linear_velocity': {
            'x': linear_velocity.x,
            'y': linear_velocity.y,
            'z': linear_velocity.z
        },
        'angular_velocity': {
            'x': angular_velocity.x,
            'y': angular_velocity.y,
            'z': angular_velocity.z
        }
    }
    # Send the odometry data to the Socket.io server
    # sio.emit('gazebo_event', odom_dict, namespace='/gazebo')
    #print("sent to digital")
    twist = Twist()
#    twist.linear.x = checkLinearLimitVelocity(linear_velocity.x)
#    twist.linear.y = checkLinearLimitVelocity(linear_velocity.y)
 #   twist.linear.z = checkLinearLimitVelocity(linear_velocity.z)
  #  twist.angular.x = checkAngularLimitVelocity(angular_velocity.x)
   # twist.angular.y = checkAngularLimitVelocity(angular_velocity.y)
 #   twist.angular.z = checkAngularLimitVelocity(angular_velocity.z)
    twist.linear.x =target_linear_vel
    twist.angular.z=target_angular_vel

    if interrupted == False:
        pub.publish(twist)
        # print("Alo 2")
       # print(twist)
        #print(interrupted)
    else:
        set_velocities(0, 0)
####


def set_velocities(linear, angular):
        twist = Twist()
        twist.linear.x = linear
        twist.angular.z = angular
        pub.publish(twist)

####


if __name__ == '__main__':
    turtlebot3_model = rospy.get_param("model", "waffle_pi")
    rospy.init_node('aa')
    #sio.connect('http://localhost:8000')
    #subscriber = rospy.Subscriber("/interrupt_state", Bool, interrupt_callback)
    pub = rospy.Publisher('/cmd_vel', Twist, queue_size=10)
    print("before odom")
    odom_sub = rospy.Subscriber('/odom', Odometry, odom_callback) # we are uncommenting this so that it mirrors the physical 
    print("before control")
    sub = rospy.Subscriber('/control',String,key_callback)
    try:
        sio.connect('http://192.168.105.194:8000', namespaces=[ '/interruption_state'])  # added namespace here
        print("connected!!!!!!!!!!!")
        rospy.spin()
    except rospy.ROSInterruptException:
        pass
    
    
    