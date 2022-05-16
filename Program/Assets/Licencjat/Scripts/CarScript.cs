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
    public List<AxleInfo> axleLists;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    public CheckpointManager checkpointManager;

    void Start()
    {
 
    }

    void Update()
    {
 
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 nextCheckpointForward = manager.GetComponent<CheckpointManager>().getNextCheckpoint(this.transform).transform.forward;
        float Dot = Vector3.Dot(transform.forward, nextCheckpointForward);
        sensor.AddObservation(Dot);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuusActions = actionsOut.ContinuousActions;
        continuusActions[0] = Input.GetAxisRaw("Horizontal");
        continuusActions[1] = Input.GetAxisRaw("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float motor = maxMotorTorque * moveZ;
        float steering = maxSteeringAngle * moveX;

        foreach (AxleInfo axleInfo in axleLists)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }

            //https://answers.unity.com/questions/853476/making-mesh-rotate-with-wheelcollider.html
            axleInfo.leftWheel.transform.localEulerAngles = new Vector3(axleInfo.leftWheel.transform.localEulerAngles.x, axleInfo.leftWheel.steerAngle - axleInfo.leftWheel.transform.localEulerAngles.z, axleInfo.leftWheel.transform.localEulerAngles.z);
            axleInfo.rightWheel.transform.localEulerAngles = new Vector3(axleInfo.rightWheel.transform.localEulerAngles.x, axleInfo.rightWheel.steerAngle - axleInfo.rightWheel.transform.localEulerAngles.z, axleInfo.rightWheel.transform.localEulerAngles.z);

            axleInfo.leftWheel.transform.Rotate(axleInfo.leftWheel.rpm / 60 * 360 * Time.deltaTime, 0, 0);
            axleInfo.rightWheel.transform.Rotate(axleInfo.rightWheel.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
        transform.position = Vector3.zero;
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
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Finish>(out Finish finish))
        {
            AddReward(20.0f);
            EndEpisode();
            manager.GetComponent<CheckpointManager>().ResetIndexes();
        }

        if (other.TryGetComponent<Checkpoint>(out Checkpoint checkpoint))
        {

            //Vector3 carDirtection = transform.TransformDirection(Vector3.forward);
            //float dot = Vector3.Dot(carDirtection, attackerDirection.normalized); // the player position should be playerTransform.forward, enemy: attackerTransform.forward)... I think they are already normalized, so you probably won't need the .normalize


            /*if (this.transform.position.z - checkpoint.transform.position.z >= 0)
            {
                //Debug.Log("positive z, negative reward");
                AddReward(-10.0f);
                EndEpisode();
            }
            else
            {
                //Debug.Log("negative z, positive reward");
                AddReward(1.0f);
            }*/

        }
        Debug.Log("AAAAAAAAAAAAAAAAA");
        if (other.TryGetComponent<Railing>(out Railing railing))
        {
            AddReward(-10.0f);
            EndEpisode();
            manager.GetComponent<CheckpointManager>().ResetIndexes();
        }
    }

/*    private void OnTriggerExit(Collider other)
    {
        *//*        if (other.TryGetComponent<Checkpoint>(out Checkpoint checkpoint))
                {
                    if (this.transform.position.z - checkpoint.transform.position.z >= 0)
                    {
                        Debug.Log("positive z, leave from front, negative reward");
                        AddReward(1.0f);
                        //EndEpisode();
                    }
                    else
                    {
                        Debug.Log("negative z, leave from back, negative reward");
                        AddReward(-10.0f);
                        EndEpisode();
                    }
                }*//*

        if (other.TryGetComponent<CheckpointFront>(out CheckpointFront checkpointFront))
        {
            AddReward(1.0f);
            Debug.Log("Left correctly, positive reward");
            GameObject parent = checkpointFront.gameObject.transform.parent.gameObject;
            CheckpointBack back = parent.GetComponent<CheckpointBack>();
            back.gameObject.GetComponent<BoxCollider>().isTrigger = true;
        }
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        
    }
/*    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<Railing>(out Railing railing))
        {
            AddReward(-5.0f);
            Debug.Log("Negative reward");
        }
    }*/
}

