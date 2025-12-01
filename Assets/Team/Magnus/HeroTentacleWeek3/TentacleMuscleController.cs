using UnityEngine;

public class TentacleMuscleController : MonoBehaviour
{
    private HingeJoint2D joint;
    private JointMotor2D motor;

    [Header("Muscle/Wiggle Settings")]
    public float baseMotorSpeed = 0.5f; // Base rotation
    public float wiggleAmplitude = 0.5f; // Speed variation
    public float wiggleFrequency = 1f; // How frequently it will change amplitude
    public float maxTorque = 20f; // Must match editor

    private float timeOffset;

    void Start()
    {
        joint = GetComponent<HingeJoint2D>();

        if (joint == null)
        {
            Debug.LogError(
                "TentacleMuscleController needs a hinge joint 2d on the same game object."
            );
            return;
        }

        timeOffset = Random.Range(0f, 100f);

        motor = joint.motor;
        motor.maxMotorTorque = maxTorque;

        joint.useLimits = true;
        joint.useMotor = true;
    }

    void FixedUpdate()
    {
        if (joint == null)
            return;

        float wiggle = Mathf.Sin(Time.time * wiggleFrequency + timeOffset) * wiggleAmplitude;

        motor.motorSpeed = baseMotorSpeed + wiggle;

        joint.motor = motor;
    }
}
