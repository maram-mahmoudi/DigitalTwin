using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using Unity.Robotics.UrdfImporter.Control;
using RosMessageTypes.Sensor;

using System.Collections;

namespace RosSharp.Control
{
    public enum ControlMode { Keyboard, ROS, Gesture};

    public class AGVController : MonoBehaviour
    {
        public GameObject wheel1;
        public GameObject wheel2;
        public ControlMode mode = ControlMode.ROS;

        private ArticulationBody wA1;
        private ArticulationBody wA2;
        
        public float maxLinearSpeed = 2; //  m/s
        public float maxRotationalSpeed = 1;//
        public float wheelRadius = 0.033f; //meters
        public float trackWidth = 0.288f; // meters Distance between tyres
        public float forceLimit = 10;
        public float damping = 10;

        public float ROSTimeout = 0.5f;
        private float lastCmdReceived = 0f;

        ROSConnection ros;
        ROSConnection ros_pub;
        
        private RotationDirection direction;
        private float rosLinear = 0f;
        private float rosAngular = 0f;
        private float gestureLinear = 0f;
        private float gestureAngular = 0f;
        public bool obstacleFlag = false; 
        
        void Start()
        {
            wA1 = wheel1.GetComponent<ArticulationBody>();
            wA2 = wheel2.GetComponent<ArticulationBody>();
            SetParameters(wA1);
            SetParameters(wA2);
            ros = ROSConnection.GetOrCreateInstance();
            ros_pub =ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<TwistMsg>("cmd_vel");
            ros.Subscribe<TwistMsg>("cmd_vel", ReceiveROSCmd);
        }

        void ReceiveROSCmd(TwistMsg cmdVel)
        {
            rosLinear = (float)cmdVel.linear.x;
            rosAngular = (float)cmdVel.angular.z;
            lastCmdReceived = Time.time;
        }

        void FixedUpdate()
        {
            if (mode == ControlMode.Keyboard)
            {
                 KeyBoardUpdate();
            }
            else if (mode == ControlMode.ROS)
            {
                ROSUpdate();
            }     
            else if (mode == ControlMode.Gesture){
                GestureUpdate();
            }
        }

        private void SetParameters(ArticulationBody joint)
        {
            ArticulationDrive drive = joint.xDrive;
            drive.forceLimit = forceLimit;
            drive.damping = damping;
            joint.xDrive = drive;
        }

        private void SetSpeed(ArticulationBody joint, float wheelSpeed = float.NaN)
        {
            ArticulationDrive drive = joint.xDrive;

    
            if (float.IsNaN(wheelSpeed))
            {
                drive.targetVelocity = ((2 * maxLinearSpeed) / wheelRadius) * Mathf.Rad2Deg * (int)direction;
            }
            else
            {
                drive.targetVelocity = wheelSpeed;
            }
            joint.xDrive = drive;
        }
        
        private void KeyBoardUpdate()
        {
           

            float moveDirection = Input.GetAxis("Left_Right");
            float stop = Input.GetAxis("stop");
            //Debug.Log("Stop " + stop +" \n");

            float inputSpeed;
            float inputRotationSpeed;
            if (moveDirection < 0)
            {
                inputSpeed = maxLinearSpeed;
            }
            else if (moveDirection > 0)
            {
                inputSpeed = maxLinearSpeed * -1;
            }
            else
            {
                inputSpeed = 0;
            }
            
            float turnDirction = Input.GetAxis("Up_Down");
            //Debug.Log("turnDirction " + turnDirction +" \n");
            
            if (turnDirction > 0)
            {
                inputRotationSpeed = maxRotationalSpeed;
            }
            else if (turnDirction < 0)
            {
                inputRotationSpeed = maxRotationalSpeed * -1;
            }
            else
            {
                inputRotationSpeed = 0;
            }
            if (stop>0f){
                RobotInput(0, 0);
                moveDirection =0f;
                turnDirction=0f;
            } else{
                RobotInput(inputSpeed, inputRotationSpeed);
            }
            
        }


        private void ROSUpdate()
        {
            if (Time.time - lastCmdReceived > ROSTimeout)
            {
                rosLinear = 0f;
                rosAngular = 0f;
            }

            RobotInput(3.8f*rosLinear, -rosAngular);
        }

        private void GestureUpdate(){
            
   
                
            RobotInput(gestureLinear, gestureAngular);

        }
        public void Gesture (string action){
            Debug.Log("gesturex"+ action);
            if (action == "forward") {
                gestureLinear =maxLinearSpeed; 
                Debug.Log("apple1");
            }
            else if (action == "backward"){
                gestureLinear = -maxLinearSpeed;
                Debug.Log("apple2");
            }
            else if (action == "right"){
                gestureAngular = -maxRotationalSpeed;
                Debug.Log("apple3");
            }
            else if (action == "left"){
                Debug.Log("apple4");
                gestureAngular = maxRotationalSpeed;
            }
            else if (action == "stop"){         
                Debug.Log("apple5");
                gestureAngular = 0 ;
                gestureLinear = 0;
            }
        }

        

        public void RobotInput(float speed, float rotSpeed) // m/s and rad/s
        {
            if (obstacleFlag && speed >0f) {
                Debug.Log("stopped");
                speed= - speed;
                rotSpeed =0f;
            } 
            if (speed > maxLinearSpeed)
            {
                speed = maxLinearSpeed;
            }
            if (rotSpeed > maxRotationalSpeed)
            {
                rotSpeed = maxRotationalSpeed;
            }
            float wheel1Rotation = (speed / (wheelRadius*3.8f));
            float wheel2Rotation = wheel1Rotation;
            float wheelSpeedDiff = ((rotSpeed * trackWidth) / wheelRadius);
            if (rotSpeed != 0)
            {
                wheel1Rotation = (wheel1Rotation + (wheelSpeedDiff / 1)) * Mathf.Rad2Deg;
                wheel2Rotation = (wheel2Rotation - (wheelSpeedDiff / 1)) * Mathf.Rad2Deg;
            }
            else
            {
                wheel1Rotation *= Mathf.Rad2Deg;
                wheel2Rotation *= Mathf.Rad2Deg;
            }
            if (ControlMode.ROS != mode ){
                var tws = new TwistMsg();
                tws.linear.x =  (speed);
                tws.angular.z = rotSpeed;
                ros.Publish("cmd_vel",tws);
                //Debug.Log("sent to cmd" + speed + ", "+ rotSpeed);
            }

            //Debug.Log("speed " + speed  +  " rotSpeed: " + rotSpeed); 
            SetSpeed(wA1, wheel1Rotation);
            //Debug.Log(" wheel1Rotation: " + wheel1Rotation); 
            SetSpeed(wA2, wheel2Rotation);
            //Debug.Log(" whee12Rotation: " + wheel2Rotation); 

  
        }
    }
}
