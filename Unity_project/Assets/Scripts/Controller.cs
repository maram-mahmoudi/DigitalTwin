using System;
using Unity.Robotics;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor; 
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Management;
using UnityEngine.XR.Hands;


// using System.Diagnostics; // just added 



namespace Unity.Robotics.UrdfImporter.Control
{   
    public enum RotationDirection { None = 0, Positive = 1, Negative = -1 };
    public enum ControlType { PositionControl };
    public enum ControlMode {Gesture, Keyboard};
    public enum GestureType {Sequential, Numerical, Mixed};
    public class Controller : MonoBehaviour
    {
        private ArticulationBody[] articulationChain;
        private JointControl[] Joints;  
        // Stores original colors of the part being highlighted
        private Color[] prevColor;
        private int previousIndex;
        public float[] jointAngle = {0.0f , 0.0f, 0.0f , 0.0f,0,0f};
        [InspectorReadOnly(hideInEditMode: true)]
        public string selectedJoint;
        [HideInInspector]
        public int selectedIndex;
        public ControlMode controlMode = ControlMode.Keyboard;
        public ControlType control = ControlType.PositionControl;
        public GestureType gestureType = GestureType.Mixed;
        public float stiffness;
        public float damping;
        public float forceLimit;
        public float speed = 5f; // Units: degree/s
        public float torque = 100f; // Units: Nm or N
        public float acceleration = 5f;// Units: m/s^2 / degree/s^2
        ROSConnection ros;
        
        [Tooltip("Color to highlight the currently selected join")]
        public Color highLightColor = new Color(1.0f, 0, 0, 1.0f);


        bool initialZ= true ;
        float initialHandPosZ =0, initialHandPosX=0 , initialHandPosY =0 ;
        float currentHandPosY =0, currentHandPosX =0, currentHandPosZ = 0;
        void OnHandUpdate(XRHandSubsystem subsystem,
                        XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
                        XRHandSubsystem.UpdateType updateType)
            {
                XRHand rightHand = subsystem.rightHand;
                 
                var palmTrackingData = rightHand.GetJoint(XRHandJointIDUtility.FromIndex(1));
                var wristTrackingData = rightHand.GetJoint(XRHandJointIDUtility.FromIndex(0));
                if (initialZ == true) {
                    initialHandPosZ = rightHand.rootPose.position.z;
                    initialHandPosX = rightHand.rootPose.position.x;
                    initialHandPosY = rightHand.rootPose.position.y;

                }
                currentHandPosZ = rightHand.rootPose.position.z;                
                currentHandPosX = rightHand.rootPose.position.x;
                currentHandPosY = rightHand.rootPose.position.y;

                //UnityEngine.Debug.Log( "mangag : " + currentHandPosZ + "  " + currentHandPosX );
                
                // if (wristTrackingData.TryGetPose(out Pose pose))  UnityEngine.Debug.Log( "mangaW : " + pose.position + "\t");
                // UnityEngine.Debug.Log("manga"+ initialHandPosZ + "\t" + currentHandPosZ);
                // if (palmTrackingData.TryGetPose(out Pose pose))  UnityEngine.Debug.Log( "mangaP : " + pose.forward);
                // if (palmTrackingData.TryGetRadius(out float pose2))  UnityEngine.Debug.Log( "mangaR : " + pose2);
                if (rightHand.rootPose.position.z!=0) initialZ = false;
                if (!initialZ && gestureType==GestureType.Mixed ) HandMovement(currentHandPosZ - initialHandPosZ, currentHandPosY - initialHandPosY, currentHandPosX - initialHandPosX);

        }

        void Start()
        {
            XRHand x;
            XRHandSubsystem m_Subsystem = 
                XRGeneralSettings.Instance?
                .Manager?
                .activeLoader?
                .GetLoadedSubsystem<XRHandSubsystem>();

            if (m_Subsystem != null ) m_Subsystem.updatedHands += OnHandUpdate;

            
            ros = ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<JointStateMsg>("joint_states");
            
            previousIndex = selectedIndex = 1;
            this.gameObject.AddComponent<FKRobot>();
            articulationChain = this.GetComponentsInChildren<ArticulationBody>();
            int defDyanmicVal = 10; 
            foreach (ArticulationBody joint in articulationChain)
            {
                joint.gameObject.AddComponent<JointControl>();
                joint.jointFriction = defDyanmicVal;
                joint.angularDamping = defDyanmicVal;
                ArticulationDrive currentDrive = joint.xDrive;
                currentDrive.forceLimit = forceLimit;
                joint.xDrive = currentDrive;                
            }
            Joints = new JointControl[articulationChain.Length];
               
            for (int index = 0; index < articulationChain.Length; index++) {
               Joints[index] =  articulationChain[index].GetComponent<JointControl>();
            }
            DisplaySelectedJoint(selectedIndex);
            StoreJointColors(selectedIndex);

        }
        public void changeIdx(string x) {
            if (previousIndex != selectedIndex) previousIndex = selectedIndex; 
            // controlMode = ControlMode.Gesture;
            int idx  =selectedIndex ;
            if (gestureType==GestureType.Sequential){
                 if (x=="next")  {
                idx++;
                if (idx==5 ) idx=6;
                else if (idx==7)  idx=1;
                }
                else if (x=="prev") {
                    idx--;
                    if( idx==5 ) idx--;
                    else if (idx==0) idx =6;
                }
            }
            if (gestureType==GestureType.Numerical){
                if (x=="1" || x=="2" ||x=="3" || x=="4" || x=="6" ) idx = int.Parse(x) ;
            }
            SetSelectedJointIndex(idx);
            Highlight(selectedIndex);
            // UnityEngine.Debug.Log("cur idx " + selectedIndex );
            
        }
        public void HandMovement(float offsetZ, float offsetY, float offsetX ){
            UnityEngine.Debug.Log("hand move" + offsetX + " " + offsetY + " " + offsetZ);
            ArticulationDrive newDrive = Joints[2].joint.xDrive;
            newDrive.target = offsetZ*200;
            Joints[2].joint.xDrive = newDrive;

            newDrive = Joints[3].joint.xDrive;
            newDrive.target = offsetY*-150;
            Joints[3].joint.xDrive = newDrive;

            // newDrive = Joints[5].joint.xDrive;
            // newDrive.target = offsetY*100;
            // Joints[5].joint.xDrive = newDrive;
            // UnityEngine.Debug.Log("jjjoint"+Joints[5].joint.xDrive.target);
            
            newDrive = Joints[4].joint.xDrive;
            newDrive.target = -offsetY*120;
            newDrive.target = Math.Max(-70,newDrive.target);
            newDrive.target = Math.Min(70,newDrive.target);
            Joints[4].joint.xDrive = newDrive;

            newDrive = Joints[1].joint.xDrive;
            newDrive.target = offsetX*-200;
            Joints[1].joint.xDrive = newDrive;
            
        }
        
        public void gesture(String action){ 
            
            ArticulationDrive newDrive;
            if (gestureType == GestureType.Mixed){
                 if (action == "Grip") {
                    Joints[6].direction  = RotationDirection.Negative;
                    Joints[7].direction  = RotationDirection.Negative;
                    Joints[5].direction = RotationDirection.Negative;
                }
                else if (action == "open") {
                    Joints[6].direction  = RotationDirection.Positive;
                    Joints[7].direction  = RotationDirection.Positive;
                }
                else if (action == "Ungrip" || action == "Unopen") {
                    Joints[6].direction  = RotationDirection.None;
                    Joints[7].direction  = RotationDirection.None;

                }
                else if (action == "wrist_down" )
                {
                    newDrive = Joints[4].joint.xDrive;
                    newDrive.target = 60;
                    Joints[4].joint.xDrive = newDrive;
                    // Joints[4].direction  = RotationDirection.Positive;
                }
                else if (action == "wrist_up" ) {
                    newDrive = Joints[4].joint.xDrive;
                    newDrive.target = -60;
                    Joints[4].joint.xDrive = newDrive;
                    // Joints[4].direction  = RotationDirection.Negative;
                }
                else if (action == "wrist_normal"){
                    newDrive = Joints[4].joint.xDrive;
                    newDrive.target = 0;
                    Joints[4].joint.xDrive = newDrive;
                }
            }

            else if (gestureType==GestureType.Sequential || gestureType == GestureType.Numerical){
                if (selectedIndex == 6 || selectedIndex == 7) { // for two gripper joints
                    if (action == "Ungrip" || action == "Unopen") {
                        Joints[6].direction  = RotationDirection.None;
                        Joints[7].direction  = RotationDirection.None;
                    }
                    else if (action == "Grip") {

                        Joints[6].direction  = RotationDirection.Negative;
                        Joints[7].direction  = RotationDirection.Negative;
                    }
                    else if (action == "open") {
                        Joints[6].direction  = RotationDirection.Positive;
                        Joints[7].direction  = RotationDirection.Positive;
                    }
                    else if (action == "top" && gestureType==GestureType.Sequential){
                        Joints[6].direction  = RotationDirection.None;
                        Joints[7].direction  = RotationDirection.None;
                    }
                }
                else {  // for non gripper joints
                    UnityEngine.Debug.Log("sssa" + action); 

                    if (action == "Ungrip" || action == "Unopen")  Joints[selectedIndex].direction = RotationDirection.None;
                    else if (action == "Grip")   {
                        Joints[selectedIndex].direction = RotationDirection.Negative;
                        // newDrive = Joints[selectedIndex].joint.xDrive;
                        // newDrive.target -=10f;
                        // Joints[selectedIndex].joint.xDrive =  newDrive;

                    }
                    else if (action == "open")    Joints[selectedIndex].direction = RotationDirection.Positive;
                     else if (action == "top" && gestureType==GestureType.Sequential){
                        Joints[selectedIndex].direction  = RotationDirection.None;
                       
                    }
                }
            }
        }

        
        
        void SetSelectedJointIndex(int index)
        {
            
            if (articulationChain.Length > 0)
            {
                selectedIndex = (index + articulationChain.Length) % articulationChain.Length;
            }
        }

        void Update()
        {
            bool SelectionInput1 = Input.GetKeyDown("right");
            bool SelectionInput2 = Input.GetKeyDown("left");
            // UnityEngine.Debug.Log("hiii");   
            //  var message = new JointStateMsg ();
            // // message.name = {"ahmed"};
            // string[]name = {"ahmed"};
            // double[] arr  = {-1.1f,-2.2f};
            // message.name = name; 
            // message.position = arr;
            // ros.Publish("joint_states", message);
            SetSelectedJointIndex(selectedIndex); // to make sure it is in the valid range
            UpdateDirection(selectedIndex);
            
            if (SelectionInput2)
            {
                SetSelectedJointIndex(selectedIndex - 1);
                Highlight(selectedIndex);
            }
            else if (SelectionInput1)
            {
                SetSelectedJointIndex(selectedIndex + 1);
                Highlight(selectedIndex);
            }

            UpdateDirection(selectedIndex);
            // Highlight(selectedIndex);
            publish2ROS();
            // int idx =0;
            // foreach (JointControl joint in Joints)
            // {
                
            //     UnityEngine.Debug.Log(idx++ + " angle : " + joint.joint.xDrive.target + "  " + joint.direction +"\n");
                
            // }
        }
        uint seq = 1;
        private void publish2ROS (){

            string[] names = {"gripper", "joint0", "joint1", "joint2", "joint3", "joint4" };
            double[] angles = new double[6]; 

            for (int index =2; index <= 5; index++) { 
                angles[index] = Joints[index-1].joint.xDrive.target;
                angles[index] =  Math.PI * angles[index]/ 180f;
            }
            // check which joints need to be mul by -1
            angles[3]-=0.25f;
            angles[4] = -1*(angles[4]-0.2f);
            angles[5] =-(angles[5]-0.175f);
            //UnityEngine.Debug.Log("gripaa" + 180f*(angles[5]+(angles[5]-0.175f))/Math.PI);
            if (Joints[6].joint.xDrive.target<0.0f) angles[0]= 0.05; // gripper
            if (Joints[6].joint.xDrive.target>0.0f) angles[0]=0; // gripper
            
            angles[1] = 0.0f;
            

            DateTime now = DateTime.Now;
            var message = new JointStateMsg();

            message.header.seq = seq++;
            // message.header.stamp.sec = (uint)now.Minute+(uint)now.Second  ;// = timeStamp;
            message.header.stamp.nanosec =  (uint)now.Minute*60*1000+(uint)now.Second*1000+(uint)now.Millisecond ;
            // // message.header.frame_id = (string)now.Minute;
            //message.header.stamp.nanosec  =(uint)nanoSeconds ;
            
            message.name = names;
            message.position = angles;
            Debug.Log("jointx" + message);//                                                                                                                                                                                                                                                                                                                                                                                                                                                           );
            ros.Publish("joint_states",message);
        }
        /// <summary>
        /// Highlights the color of the robot by changing the color of the part to a color set by the user in the inspector window
        /// </summary>
        /// <param name="selectedIndex">Index of the link selected in the Articulation Chain</param>
        private void Highlight(int selectedIndex)
        {    //UnityEngine.Debug.Log("Highlited enter"); 
            if (selectedIndex == previousIndex || selectedIndex < 0 || selectedIndex >= articulationChain.Length)
            {
                return;
            }
            
            // reset colors for the previously selected joint
            ///UnityEngine.Debug.Log("prev" + previousIndex + "  " + selectedIndex);
            ResetJointColors(previousIndex);
            if (previousIndex==6) ResetJointColors(7);


            // store colors for the current selected joint
            StoreJointColors(selectedIndex);
            //UnityEngine.Debug.Log("Highlited pass 2 "); 
            Renderer[] rendererList = articulationChain[selectedIndex].transform.GetChild(1).GetComponentsInChildren<Renderer>();

            //UnityEngine.Debug.Log("Number of Renderers: " + rendererList.Length);

            foreach (var mesh in rendererList)
            {
                if (mesh != null && mesh.material != null)
                {
                    UnityEngine.Debug.Log("Renderer: " + mesh.name);
                    UnityEngine.Debug.Log("Material: " + mesh.material.name);
                    MaterialExtensions.SetMaterialColor(mesh.material, highLightColor);
                    UnityEngine.Debug.Log("Highlighted");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Renderer or Material is null");
                }
            }
            if (selectedIndex==6){
                Renderer[] rendererList2 = articulationChain[7].transform.GetChild(1).GetComponentsInChildren<Renderer>();
                foreach (var mesh in rendererList2)
                {
                    if (mesh != null && mesh.material != null)
                    {
                        UnityEngine.Debug.Log("Renderer: " + mesh.name);
                        UnityEngine.Debug.Log("Material: " + mesh.material.name);
                        MaterialExtensions.SetMaterialColor(mesh.material, highLightColor);
                        UnityEngine.Debug.Log("Highlighted");
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("Renderer or Material is null");
                        }
                }
            }
        }

        void DisplaySelectedJoint(int selectedIndex)
        {
            if (selectedIndex < 0 || selectedIndex >= articulationChain.Length)
            {
                return;
            }
            selectedJoint = articulationChain[selectedIndex].name + " (" + selectedIndex + ")";
        }

        /// <summary>
        /// Sets the direction of movement of the joint on every update
        /// </summary>
        /// <param name="jointIndex">Index of the link selected in the Articulation Chain</param>

       

        private void UpdateDirection(int jointIndex)
        {

            if (jointIndex < 0 || jointIndex >= articulationChain.Length)
            {
                return;
            }
            
            if (controlMode == ControlMode.Keyboard){
            
                float moveDirection = Input.GetAxis("Vertical"); 
                //UnityEngine.Debug.Log("dir" + moveDirection);

                //UnityEngine.Debug.Log("IDX" + jointIndex); 
                JointControl current = Joints[jointIndex];
                
                if (previousIndex != jointIndex) 
                {
                    JointControl previous = Joints[previousIndex];
                    previous.direction = RotationDirection.None;
                    previousIndex = jointIndex; 
                }

                if (current.controltype != control)
                {
                    UpdateControlType(current);
                }

                if (moveDirection > 0)
                {
                    current.direction = RotationDirection.Positive;
                }
                else if (moveDirection < 0)
                {
                    current.direction = RotationDirection.Negative;
                }
                else
                {
                    current.direction = RotationDirection.None;
                }
            }
            
        }

        /// <summary>
        /// Stores original color of the part being highlighted
        /// </summary>
        /// <param name="index">Index of the part in the Articulation chain</param>
        private void StoreJointColors(int index)
        {
            Renderer[] materialLists = articulationChain[index].transform.GetChild(1).GetComponentsInChildren<Renderer>();
            prevColor = new Color[materialLists.Length];
            for (int counter = 0; counter < materialLists.Length; counter++)
            {
                prevColor[counter] = MaterialExtensions.GetMaterialColor(materialLists[counter]);
            }
        }

        /// <summary>   
        /// Resets original color of the part being highlighted
        /// </summary>
        /// <param name="index">Index of the part in the Articulation chain</param>
        private void ResetJointColors(int index)
        {
            Renderer[] previousRendererList = articulationChain[index].transform.GetChild(1).GetComponentsInChildren<Renderer>();
            for (int counter = 0; counter < previousRendererList.Length; counter++)
            {
                MaterialExtensions.SetMaterialColor(previousRendererList[counter].material, prevColor[counter]);
            }
        }

        public void UpdateControlType(JointControl joint)
        {

            joint.controltype = control;
            if (control == ControlType.PositionControl)
            {
                ArticulationDrive drive = joint.joint.xDrive;
                drive.stiffness = stiffness;
                drive.damping = damping;
                joint.joint.xDrive = drive;
            }
        }

        public void OnGUI()
        {   
           
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            GUI.Label(new Rect(Screen.width / 2 - 200, 10, 400, 20), "Press left/right arrow keys to select a robot joint.", centeredStyle);
            GUI.Label(new Rect(Screen.width / 2 - 200, 30, 400, 20), "Press up/down arrow keys to move " + selectedJoint + ".", centeredStyle);
        }
    }
}