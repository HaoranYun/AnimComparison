using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // Start is called before the first frame update

    public ObjectPickup activeController;
    public UpperBodyTask upperBodyTask;
    void Start()
    {
        upperBodyTask = GameObject.Find("upper body task").GetComponent<UpperBodyTask>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.name == "target"&& !upperBodyTask.waitingNextRound) StartCoroutine(upperBodyTask.ShowResult());
    }

    
    }
