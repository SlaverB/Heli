using UnityEngine;

namespace Heli.Flight
{
    [CreateAssetMenu(menuName = "Heli/Helicopter Config", fileName = "HelicopterConfig")]
    public class HelicopterConfig : ScriptableObject
    {
        [Header("Main rotor")]
        public float maxThrust = 30000f;
        public float rotorRadius = 5f;
        public float spoolTime = 4f;
        [Tooltip("Collective lever travel per second")]
        public float collectiveRate = 3f;
        [Tooltip("Reaction torque on the fuselage per newton of thrust, N*m/N")]
        public float torqueFactor = 0.06f;
        [Range(0f, 0.4f)] public float groundEffect = 0.15f;

        [Header("Tail rotor")]
        public float tailArm = 6f;
        public float maxTailThrust = 3000f;
        [Tooltip("Apply tail thrust as a real force at the tail (adds sideways drift in hover)")]
        public bool tailSideForce;

        [Header("Handling")]
        public float maxTilt = 25f;
        public float maxClimbRate = 6f;
        public float maxDescentRate = 4f;
        public float maxYawRate = 60f;
        public float attitudeKp = 10f;
        public float attitudeKd = 4.5f;
        public float maxAngularAccel = 4f;
        public float climbGain = 2.5f;
        public float yawGain = 4f;

        [Header("Aero")]
        public float airDensity = 1.225f;
        [Tooltip("Cd * A per local axis, m^2")]
        public Vector3 dragArea = new Vector3(6f, 8f, 3f);
        public float angularDamping = 0.5f;
    }
}
