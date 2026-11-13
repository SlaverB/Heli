using Heli.Flight;
using TMPro;
using UnityEngine;

namespace Heli.View
{
    public class FlightHud : MonoBehaviour
    {
        [SerializeField] private Helicopter helicopter;
        [SerializeField] private TMP_Text label;

        private void Update()
        {
            UpdateFlightInfo();
        }

        private void UpdateFlightInfo()
        {
            Vector3 v = helicopter.Velocity;
            float ground = new Vector2(v.x, v.z).magnitude * 3.6f;

            label.SetText("AGL  {0:1} m\nSPD  {1:0} km/h\nV/S  {2:1} m/s\nCOL  {3:0}%\nRPM  {4:0}%",
                    helicopter.HeightAboveGround, ground, v.y,
                    helicopter.Collective * 100f, helicopter.RpmNormalized * 100f);
        }
    }
}
