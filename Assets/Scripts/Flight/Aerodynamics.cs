using UnityEngine;

namespace Heli.Flight
{
    public static class Aerodynamics
    {
        // F = -1/2 * rho * CdA * v * |v|, per body axis
        public static Vector3 Drag(Vector3 localVelocity, Vector3 dragArea, float airDensity)
        {
            float k = 0.5f * airDensity;
            return new Vector3(
                -k * dragArea.x * localVelocity.x * Mathf.Abs(localVelocity.x),
                -k * dragArea.y * localVelocity.y * Mathf.Abs(localVelocity.y),
                -k * dragArea.z * localVelocity.z * Mathf.Abs(localVelocity.z));
        }

        // Thrust gain near the ground, fades out at about one rotor diameter.
        public static float GroundEffect(float height, float rotorRadius, float strength)
        {
            float t = Mathf.Clamp01(1f - height / (2f * rotorRadius));
            return 1f + strength * t * t;
        }
    }
}
