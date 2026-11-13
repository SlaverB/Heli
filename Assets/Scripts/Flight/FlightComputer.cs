using Heli.Controls;
using UnityEngine;

namespace Heli.Flight
{
    public struct FlightState
    {
        public float Mass;
        public float Gravity;
        public Vector3 Inertia;
        public Quaternion Rotation;
        public Vector3 LocalAngularVelocity;
        public float VerticalSpeed;
        public float Rpm;
        public float AvailableThrust;
        public bool Grounded;
    }

    public struct FlightCommand
    {
        public float Thrust;
        public Vector3 CyclicTorque;
        public float TailThrust;  
    }
    
    public class FlightComputer
    {
        private readonly HelicopterConfig _cfg;

        public float Collective { get; private set; }

        public FlightComputer(HelicopterConfig config)
        {
            _cfg = config;
        }

        public FlightCommand Step(in FlightState s, PilotInput input, float dt)
        {
            bool parked = s.Grounded && input.Collective <= 0.01f;

            UpdateCollective(s, input, parked, dt);
            float thrust = Collective * s.AvailableThrust;

            return new FlightCommand
            {
                Thrust = thrust,
                CyclicTorque = parked ? Vector3.zero : CyclicTorque(s, input),
                TailThrust = TailThrust(s, input, thrust)
            };
        }

        private void UpdateCollective(in FlightState s, PilotInput input, bool parked, float dt)
        {
            var target = 0f;

            if (!parked && s.AvailableThrust > 1f)
            {
                float climb = input.Collective * (input.Collective >= 0f ? _cfg.maxClimbRate : _cfg.maxDescentRate);
                float cosTilt = Mathf.Max(0.5f, (s.Rotation * Vector3.up).y);
                float lift = s.Mass * (s.Gravity + _cfg.climbGain * (climb - s.VerticalSpeed)) / cosTilt;
                target = Mathf.Clamp01(lift / s.AvailableThrust);
            }

            Collective = Mathf.MoveTowards(Collective, target, _cfg.collectiveRate * dt);
        }

        private Vector3 CyclicTorque(in FlightState s, PilotInput input)
        {
            float heading = s.Rotation.eulerAngles.y;
            Quaternion wanted = Quaternion.Euler(0f, heading, 0f) *
                                Quaternion.Euler(input.Cyclic.y * _cfg.maxTilt, 0f, -input.Cyclic.x * _cfg.maxTilt);

            Quaternion delta = wanted * Quaternion.Inverse(s.Rotation);
            delta.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;

            Vector3 error = Quaternion.Inverse(s.Rotation) * (axis * (angle * Mathf.Deg2Rad));
            Vector3 accel = _cfg.attitudeKp * error - _cfg.attitudeKd * s.LocalAngularVelocity;
            accel.y = 0f;
            accel = Vector3.ClampMagnitude(accel, _cfg.maxAngularAccel);
            
            return Vector3.Scale(s.Inertia, accel) * (s.Rpm * s.Rpm);
        }

        private float TailThrust(in FlightState s, PilotInput input, float mainThrust)
        {
            float wantedRate = input.Pedal * _cfg.maxYawRate * Mathf.Deg2Rad;
            float accel = _cfg.yawGain * (wantedRate - s.LocalAngularVelocity.y);
            
            float torque = s.Inertia.y * accel + _cfg.torqueFactor * mainThrust;

            float limit = _cfg.maxTailThrust * s.Rpm * s.Rpm;
            return Mathf.Clamp(-torque / _cfg.tailArm, -limit, limit);
        }
    }
}
