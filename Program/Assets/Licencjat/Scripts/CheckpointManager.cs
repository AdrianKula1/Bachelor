using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CheckpointManager : MonoBehaviour
{
    [SerializeField] List<Transform> carList;
    [SerializeField] List<int> nextCheckpointList;


    List<Checkpoint> checkpoints;

    void Start()
    {
        checkpoints = new List<Checkpoint>();
        foreach (Transform child in transform)
        {
            checkpoints.Add(child.GetComponent<Checkpoint>());
            child.GetComponent<Checkpoint>().SetCheckpoints(this);

        }
        nextCheckpointList = new List<int>();
        ResetIndexes(null);
    }


    public void CarThroughCheckpoint(Checkpoint checkpoint, Transform carTransform)
    {
        int nextCheckpointIndex = nextCheckpointList[carList.IndexOf(carTransform)];
        if (checkpoints.IndexOf(checkpoint) == nextCheckpointIndex)
        {
            nextCheckpointIndex = (nextCheckpointIndex + 1) % checkpoints.Count;
            nextCheckpointList[carList.IndexOf(carTransform)] = nextCheckpointIndex;
            carList[(carList.IndexOf(carTransform))].GetComponent<CarScript>().AddReward(5.0f);
        }
        else
        {
            carList[(carList.IndexOf(carTransform))].GetComponent<CarScript>().AddReward(-100.0f);
            carList[(carList.IndexOf(carTransform))].GetComponent<CarScript>().EndEpisode();
            nextCheckpointList[carList.IndexOf(carTransform)] = 0;
            ResetIndexes(carTransform);
        }
    }

    public void ResetIndexes(Transform carTransform)
    {
        if (nextCheckpointList.Count != carList.Count)
        {
            foreach (Transform car in carList)
            {
                nextCheckpointList.Add(0);
            }
        }
        else
        {
            nextCheckpointList[carList.IndexOf(carTransform)] = 0;
        }
    }

    public GameObject getNextCheckpoint(Transform carTransform)
    {
        int nextCheckpointIndex = nextCheckpointList[carList.IndexOf(carTransform)];
        return checkpoints[nextCheckpointIndex].gameObject;
    }
}
