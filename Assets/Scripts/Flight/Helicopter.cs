using Heli.Controls;
using UnityEngine;

namespace Heli.Flight
{
    [RequireComponent(typeof(Rigidbody))]
    public class Helicopter : MonoBehaviour
    {
        [SerializeField] private HelicopterConfig config;
        [SerializeField] private LayerMask groundMask = ~0;
        [Tooltip("Distance from pivot down to the skids")]
        [SerializeField] private float skidOffset = 1f;

        private Rigidbody _body;
        private IPilotInputSource _pilot;
        private FlightComputer _computer;

        public float RpmNormalized { get; private set; }
        public float Collective => _computer.Collective;
        public float HeightAboveGround { get; private set; }
        public Vector3 Velocity => _body.linearVelocity;
        public bool Grounded => HeightAboveGround < 0.15f;

        void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _body.linearDamping = 0f;
            _body.angularDamping = config.angularDamping;
            _body.interpolation = RigidbodyInterpolation.Interpolate;

            _pilot = GetComponent<IPilotInputSource>();
            _computer = new FlightComputer(config);
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            RpmNormalized = Mathf.MoveTowards(RpmNormalized, 1f, dt / config.spoolTime);
            HeightAboveGround = MeasureHeight();

            float lift = config.maxThrust * RpmNormalized * RpmNormalized;
            lift *= Aerodynamics.GroundEffect(HeightAboveGround, config.rotorRadius, config.groundEffect);

            var state = new FlightState
            {
                Mass = _body.mass,
                Gravity = -Physics.gravity.y,
                Inertia = _body.inertiaTensor,
                Rotation = _body.rotation,
                LocalAngularVelocity = transform.InverseTransformDirection(_body.angularVelocity),
                VerticalSpeed = _body.linearVelocity.y,
                Rpm = RpmNormalized,
                AvailableThrust = lift,
                Grounded = Grounded
            };

            FlightCommand cmd = _computer.Step(state, _pilot != null ? _pilot.Current : default, dt);

            ApplyMainRotor(cmd);
            ApplyTailRotor(cmd.TailThrust);
            ApplyDrag();
        }

        private void ApplyMainRotor(FlightCommand cmd)
        {
            _body.AddForce(transform.up * cmd.Thrust);
            _body.AddRelativeTorque(cmd.CyclicTorque);
            _body.AddRelativeTorque(0f, -config.torqueFactor * cmd.Thrust, 0f);
        }

        private void ApplyTailRotor(float thrust)
        {
            if (config.tailSideForce)
            {
                Vector3 tail = _body.worldCenterOfMass - transform.forward * config.tailArm;
                _body.AddForceAtPosition(transform.right * thrust, tail);
            }
            else
            {
                _body.AddRelativeTorque(0f, -config.tailArm * thrust, 0f);
            }
        }

        private void ApplyDrag()
        {
            Vector3 local = transform.InverseTransformDirection(_body.linearVelocity);
            _body.AddRelativeForce(Aerodynamics.Drag(local, config.dragArea, config.airDensity));
        }

        private float MeasureHeight()
        {
            const float range = 500f;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, range, groundMask,
                    QueryTriggerInteraction.Ignore))
                return Mathf.Max(0f, hit.distance - skidOffset);

            return range;
        }
    }
}
