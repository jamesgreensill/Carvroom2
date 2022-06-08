using System.Collections;
using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    /// <summary>
    /// Every 10 seconds is equal to 1 hour in game.
    /// </summary>
    // 24
    [SerializeField] public int DayThresholdInTicks = 24;

    // 1
    [SerializeField] public int TickSize = 1;

    // 10 seconds
    [SerializeField] public float TickRateInMs = 1;

    [SerializeField] public bool Pause;

    [SerializeField] public GameObject LightSource;

    [SerializeField] public Light Light;

    internal int CurrentStateIndex;

    internal int CurrentTick;

    internal float RotationRate;

    private void Awake()
    {
        RotationRate = (float)360 / DayThresholdInTicks;
    }

    // Start is called before the first frame update

    private void Start() => StartCoroutine(CycleDay());

    private IEnumerator CycleDay()
    {
        while (true)
        {
            if (Pause) yield return new WaitForSeconds(1);
            yield return new WaitForSeconds(TickRateInMs / 1000);
            {
                // Update Time Here.
                if (CurrentTick >= DayThresholdInTicks)
                {
                    CurrentTick = 0;

                    continue;
                }

                CurrentTick += TickSize;
                
                SetLighting();

                Debug.Log(CurrentTick);
            }
        }
    }

    private void SetLighting()
    {
        LightSource.transform.SetPositionAndRotation(LightSource.transform.position, Quaternion.Euler(new Vector3(RotationRate * CurrentTick, 0, 0)));
    }
}