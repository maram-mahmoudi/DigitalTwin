using UnityEngine;
using Unity.Robotics;
using Unity.Robotics.UrdfImporter.Control;
using UrdfControlRobot = Unity.Robotics.UrdfImporter.Control;
using UnityEngine.InputSystem;
public class RobotController : MonoBehaviour
{
    public enum RotationDirection { None = 0, Positive = 1, Negative = -1 };
    public enum ControlType { PositionControl };
    //Fields for inputs
    [Header("Robot Control References")]
    [SerializeField] private InputActionProperty moveJointInput;
    [SerializeField] private InputActionProperty selectJointInput;
    //Robot properties
    private ArticulationBody[] articulationChain;
    private Color[] prevColor;
    private int previousIndex;
    [InspectorReadOnly(hideInEditMode: true)]
    public string selectedJoint;
    [HideInInspector]
    public int selectedIndex;
    [Header("Robot properties")]
    public ControlType control = ControlType.PositionControl;
    public float stiffness;
    public float damping;
    public float forceLimit;
    public float speed = 5f; // Units: degree/s
    public float torque = 100f; // Units: Nm or N
    public float acceleration = 5f;// Units: m/s^2 / degree/s^2
    [Tooltip("Color to highlight the currently selected Join")]
    public Color highLightColor = new Color(1, 0, 0, 1);
    //The old controller
    private Controller controller;
    private void OnEnable()
    {
        this.gameObject.AddComponent(typeof(Controller));
        controller = GetComponent<Controller>();
        SetControllerValues();
    }
    void Start()
    {
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
        DisplaySelectedJoint(selectedIndex);
        StoreJointColors(selectedIndex);
        //Enable to read new inputs
        selectJointInput.action.Enable();
        moveJointInput.action.Enable();
    }
    private void Update()
    {
        SetSelectedJointIndex(selectedIndex);
        UpdateDirection(selectedIndex);
        //If one key has been triggered to selectJointInput
        if (selectJointInput.action.triggered)
        {
            //Read the value: Up =  (0.0, 1.0) // Down = (0.0, -1.0)
            Vector2 inputValue = selectJointInput.action.ReadValue<Vector2>();
            if (inputValue.y > 0)
            {
                SetSelectedJointIndex(selectedIndex + 1);
                Highlight(selectedIndex);
            }
            else
            {
                SetSelectedJointIndex(selectedIndex - 1);
                Highlight(selectedIndex);
            }
        }
        UpdateDirection(selectedIndex);
    }
    private void SetSelectedJointIndex(int index)
    {
        if (articulationChain.Length > 0)
        {
            selectedIndex = (index + articulationChain.Length) % articulationChain.Length;
        }
    }
    private void Highlight(int selectedIndex)
    {
        if (selectedIndex == previousIndex || selectedIndex < 0 || selectedIndex >= articulationChain.Length)
        {
            return;
        }
        ResetJointColors(previousIndex);
        StoreJointColors(selectedIndex);
        DisplaySelectedJoint(selectedIndex);
        Renderer[] rendererList = articulationChain[selectedIndex].transform.GetChild(1).GetComponentsInChildren<Renderer>();
        foreach (var mesh in rendererList)
        {
            MaterialExtensions.SetMaterialColor(mesh.material, highLightColor);
        }
    }
    private void UpdateDirection(int jointIndex)
    {
        if (jointIndex < 0 || jointIndex >= articulationChain.Length)
            return;
        //Read the current value from moveJointInput
        Vector2 inputValue = moveJointInput.action.ReadValue<Vector2>();
        JointControl current = articulationChain[jointIndex].GetComponent<JointControl>();
        if (previousIndex != jointIndex)
        {
            JointControl previous = articulationChain[previousIndex].GetComponent<JointControl>();
            previous.direction = UrdfControlRobot.RotationDirection.None;
            previousIndex = jointIndex;
        }
        if (current.controltype != UrdfControlRobot.ControlType.PositionControl)
        {
            UpdateControlType(current);
        }
        // Move Positive = (1.0, 0.0)
        if (inputValue.x > 0)
        {
            current.direction = UrdfControlRobot.RotationDirection.Positive;
            Debug.Log(inputValue);
        }
        // Move Negative = (1.0, 0.0)
        else if (inputValue.x < 0)
        {
            Debug.Log(inputValue);
            current.direction = UrdfControlRobot.RotationDirection.Negative;
        }
        else
        {
            current.direction = UrdfControlRobot.RotationDirection.None;
        }
    }
    private void StoreJointColors(int index)
    {
        Renderer[] materialLists = articulationChain[index].transform.GetChild(1).GetComponentsInChildren<Renderer>();
        prevColor = new Color[materialLists.Length];
        for (int counter = 0; counter < materialLists.Length; counter++)
        {
            prevColor[counter] = MaterialExtensions.GetMaterialColor(materialLists[counter]);
        }
    }
    private void ResetJointColors(int index)
    {
        Renderer[] previousRendererList = articulationChain[index].transform.GetChild(1).GetComponentsInChildren<Renderer>();
        for (int counter = 0; counter < previousRendererList.Length; counter++)
        {
            MaterialExtensions.SetMaterialColor(previousRendererList[counter].material, prevColor[counter]);
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
    public void UpdateControlType(JointControl joint)
    {
        joint.controltype = UrdfControlRobot.ControlType.PositionControl;
        if (control == ControlType.PositionControl)
        {
            ArticulationDrive drive = joint.joint.xDrive;
            drive.stiffness = stiffness;
            drive.damping = damping;
            joint.joint.xDrive = drive;
        }
    }
    private void SetControllerValues()
    {
        controller.stiffness = stiffness;
        controller.damping = damping;
        controller.forceLimit = forceLimit;
        controller.speed = speed;
        controller.torque = torque;
        controller.acceleration = acceleration;
    }
    private void FixedUpdate()
    {
        SetControllerValues();
    }
}
 