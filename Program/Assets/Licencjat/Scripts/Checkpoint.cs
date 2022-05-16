using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private CheckpointManager manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<CarScript>(out CarScript car))
        {
            manager.CarThroughCheckpoint(this, other.transform);
        }
    }


    public void SetCheckpoints(CheckpointManager manager)
    {
        this.manager = manager;
    }
}
