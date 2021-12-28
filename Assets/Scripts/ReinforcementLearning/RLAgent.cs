﻿using UnityEngine;
using Unity.MLAgents;

[RequireComponent(typeof(AbstractEntity))]
public abstract class RLAgent : Agent
{
    protected float currentReward = 0f;

    public void AddRLReward(float value)
    {
        AddReward(value);
        currentReward += value;
    }

    public void EndRLEpisode(string endEpisodeStatus)
    {
        GenerateCSVData(endEpisodeStatus);
        currentReward = 0f;
        EndEpisode();
    }

    public abstract void GenerateCSVData(string endEpisodeStatus);
}