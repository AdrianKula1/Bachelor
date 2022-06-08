using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class CarScript : Agent
{
    [SerializeField] Transform manager;
    float collisionTimer = 0f;
    public List<AxleInfo> axleLists;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    Vector3 startingPosition;
    
    private void Awake()
    {
        startingPosition = this.transform.position;
    }



    public override void CollectObservations(VectorSensor sensor)
    {
        //samochód przed czy za następnym checkpointem
        Vector3 nextCheckpointForward = manager.GetComponent<CheckpointManager>().getNextCheckpoint(this.transform).transform.forward;
        float Dot = Vector3.Dot(transform.forward, nextCheckpointForward);
        sensor.AddObservation(Dot);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0; 
        if (Input.GetKey(KeyCode.D))
        {
            discreteActions[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActions[0] = 2;
        }

        discreteActions[1] = 0;
        if (Input.GetKey(KeyCode.W))
        {
            discreteActions[1] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActions[1] = 2;
        }
    }



    public override void OnActionReceived(ActionBuffers actions){
        float moveLeftRight = actions.DiscreteActions[0];
        float moveForwardBackward = actions.DiscreteActions[1];
        switch (moveLeftRight){
            case 0:
                moveLeftRight = 0f;
                break;
            case 1:
                moveLeftRight = 1f;
                break;
            case 2:
                moveLeftRight = -1f;
                break;
        }

        switch (moveForwardBackward){
            case 0:
                moveForwardBackward = 0f;
                break;
            case 1:
                moveForwardBackward = 1f;
                break;
            case 2:
                moveForwardBackward = -1f;
                break;
        }


        float motor = maxMotorTorque * moveForwardBackward;
        float steering = maxSteeringAngle * moveLeftRight;

        foreach (AxleInfo axleInfo in axleLists){
            if (axleInfo.steering){
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor){
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }

            axleInfo.leftWheel.transform.localEulerAngles = new Vector3(axleInfo.leftWheel.transform.localEulerAngles.x, axleInfo.leftWheel.steerAngle - axleInfo.leftWheel.transform.localEulerAngles.z, axleInfo.leftWheel.transform.localEulerAngles.z);
            axleInfo.rightWheel.transform.localEulerAngles = new Vector3(axleInfo.rightWheel.transform.localEulerAngles.x, axleInfo.rightWheel.steerAngle - axleInfo.rightWheel.transform.localEulerAngles.z, axleInfo.rightWheel.transform.localEulerAngles.z);

            axleInfo.leftWheel.transform.Rotate(axleInfo.leftWheel.rpm / 60 * 360 * Time.deltaTime, 0, 0);
            axleInfo.rightWheel.transform.Rotate(axleInfo.rightWheel.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        }
    }

    private void OnCollisionStay(Collision collision)
    {

        if (collision.gameObject.TryGetComponent<Railing>(out Railing railing))
        {
            collisionTimer += Time.deltaTime;
            if (collisionTimer >= 2)
            {
                collisionTimer = 0;
                AddReward(-10.0f);
                EndEpisode();
            }
            AddReward(-5.0f);
        }
    }

    public override void OnEpisodeBegin()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        transform.position = startingPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);

        Rigidbody rigiBody = GetComponent<Rigidbody>();
        if (rigiBody)
        {
            rigiBody.velocity = Vector3.zero;
            rigiBody.angularVelocity = Vector3.zero;
        }
        transform.rotation = new Quaternion(0, 0, 0, 16);

        foreach (AxleInfo axleInfo in axleLists)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = 0;
                axleInfo.rightWheel.steerAngle = 0;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = 0;
                axleInfo.rightWheel.motorTorque = 0;
            }
        }
        GetComponent<Rigidbody>().isKinematic = false;

        manager.GetComponent<CheckpointManager>().ResetIndexes(this.transform);
       
    }



}

