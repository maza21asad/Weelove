using UnityEngine;
using System.Collections;

namespace HealthbarGames
{
    public class TrafficLightController : MonoBehaviour
    {
        public TrafficLightBase trafficLight;

        [Header("Timing (seconds)")]
        public float redDuration = 5f;
        public float yellowDuration = 2f;
        public float greenDuration = 5f;

        [Header("Movers")]
        public CubeMover cubeMover;
        public CubeMover cubeMover2; // ← same script, no need for CubeMover2

        void Start()
        {
            StartCoroutine(RunTrafficLight());
        }

        IEnumerator RunTrafficLight()
        {
            while (true)
            {
                // RED ← both move
                trafficLight.OnLightStateChanged(true, false, false);
                if (cubeMover != null) cubeMover.SetRedLight(true);
                if (cubeMover2 != null) cubeMover2.SetRedLight(true);
                yield return new WaitForSeconds(redDuration);

                // YELLOW ← both stop
                trafficLight.OnLightStateChanged(false, true, false);
                if (cubeMover != null) cubeMover.SetRedLight(false);
                if (cubeMover2 != null) cubeMover2.SetRedLight(false);
                yield return new WaitForSeconds(yellowDuration);

                // GREEN ← both stop
                trafficLight.OnLightStateChanged(false, false, true);
                if (cubeMover != null) cubeMover.SetRedLight(false);
                if (cubeMover2 != null) cubeMover2.SetRedLight(false);
                yield return new WaitForSeconds(greenDuration);

                // YELLOW ← both stop
                trafficLight.OnLightStateChanged(false, true, false);
                if (cubeMover != null) cubeMover.SetRedLight(false);
                if (cubeMover2 != null) cubeMover2.SetRedLight(false);
                yield return new WaitForSeconds(yellowDuration);
            }
        }
    }
}