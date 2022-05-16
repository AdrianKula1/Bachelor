using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CheckpointManager : MonoBehaviour
{
    [SerializeField] List<Transform> carList;
    [SerializeField] List<int> nextCheckpointList;
    List<Checkpoint> checkpoints;

    // Start is called before the first frame update
    void Start()
    {
        checkpoints = new List<Checkpoint>();
        foreach (Transform child in transform)
        {
            Debug.Log("Checkpoint");
            checkpoints.Add(child.GetComponent<Checkpoint>());
            child.GetComponent<Checkpoint>().SetCheckpoints(this);

        }
        nextCheckpointList = new List<int>();
        ResetIndexes();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CarThroughCheckpoint(Checkpoint checkpoint, Transform carTransform)
    {
        int nextCheckpointIndex = nextCheckpointList[carList.IndexOf(carTransform)];
        if (checkpoints.IndexOf(checkpoint) == nextCheckpointIndex)
        {
            Debug.Log("Correct");
            nextCheckpointIndex = (nextCheckpointIndex + 1) % checkpoints.Count;
            nextCheckpointList[carList.IndexOf(carTransform)] = nextCheckpointIndex;
            carList[(carList.IndexOf(carTransform))].GetComponent<CarScript>().AddReward(50.0f);
        }
        else
        {
            Debug.Log("Wrong");
            carList[(carList.IndexOf(carTransform))].GetComponent<CarScript>().AddReward(-100.0f);
            carList[(carList.IndexOf(carTransform))].GetComponent<CarScript>().EndEpisode();

            nextCheckpointList[carList.IndexOf(carTransform)] = 0;
        }
    }

    public void ResetIndexes()
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
            for(int i=0; i< nextCheckpointList.Count; i++)
            {
                nextCheckpointList[i] = 0;
            }
        }
    }

    public GameObject getNextCheckpoint(Transform carTransform)
    {
        int nextCheckpointIndex = nextCheckpointList[carList.IndexOf(carTransform)];
        return checkpoints[nextCheckpointIndex].gameObject;
    }
}
