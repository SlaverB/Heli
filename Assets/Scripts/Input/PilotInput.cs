using UnityEngine;

namespace Heli.Controls
{
    public struct PilotInput
    {
        public Vector2 Cyclic;    // x = roll, y = pitch
        public float Collective;  // -1..1
        public float Pedal;       // -1..1
    }

    public interface IPilotInputSource
    {
        PilotInput Current { get; }
    }
}
