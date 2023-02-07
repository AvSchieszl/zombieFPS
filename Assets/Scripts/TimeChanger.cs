using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChanger : MonoBehaviour
{
    // original fixedDeltaTime
    private float originalFixedDeltaTime;

    private void Start()
    {
        // store the original fixedDeltaTime
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void SlowMotion()
    {
        Time.fixedDeltaTime = 0.02f;
    }

    public void NormalMotion()
    {
        Time.fixedDeltaTime = originalFixedDeltaTime;
    }

}
