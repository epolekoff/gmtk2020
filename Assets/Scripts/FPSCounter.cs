using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : Singleton<FPSCounter>
{
    public int CurrentFps { get { return m_CurrentFps; } }

    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;
    private int numCounts = 0;

    private void Start()
    {
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
    }


    private void Update()
    {
        // measure average frames per second
        m_FpsAccumulator++;
        if (Time.realtimeSinceStartup > m_FpsNextPeriod)
        {
            m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            numCounts++;
            m_FpsNextPeriod += fpsMeasurePeriod;
        }
    }

    public bool IsLegitimate()
    {
        return numCounts > 5;
    }
}
