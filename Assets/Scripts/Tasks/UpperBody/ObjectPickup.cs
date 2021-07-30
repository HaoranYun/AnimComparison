using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ObjectPickup : MonoBehaviour
{
    public  SteamVR_Action_Boolean grabAction = null;
    private SteamVR_Behaviour_Pose pose = null;

    //private FixedJoint joint = null;
    
    private InteractableObject currentInteractableObj;
    private List<InteractableObject> interactables = new List<InteractableObject>();

    // Start is called before the first frame update
    void Start()
    {
        pose = GetComponent<SteamVR_Behaviour_Pose>();
        //joint = GetComponent<FixedJoint>();
    
    }

    // Update is called once per frame
    void Update()
    {

        if (grabAction.GetLastStateDown(pose.inputSource))
        {
            Pick();
            Debug.Log("pick");
        }

        if (grabAction.GetLastStateUp(pose.inputSource))
        {
            Drop();
            Debug.Log("drop");
        }
    }

    public void SetObjectsList(InteractableObject[] interactableObjects)
    {
        interactables = new List<InteractableObject>();
        for(int i = 0; i < interactableObjects.Length; i++)
        {
            interactables.Add(interactableObjects[i]);
        }
        
    }

    public void Pick()
    {
        if (currentInteractableObj == null) return;
        currentInteractableObj.transform.SetParent(this.transform.GetChild(0));
        currentInteractableObj.transform.localPosition = new Vector3(0,0,0);
        currentInteractableObj.activeController = this;
        
    }

    public void Drop()
    {
        if (currentInteractableObj == null) return;
        Rigidbody targetBody = currentInteractableObj.GetComponent<Rigidbody>();
        targetBody.velocity = pose.GetVelocity();
        targetBody.angularVelocity = pose.GetAngularVelocity();

        currentInteractableObj.activeController = null;
        Rigidbody rb = currentInteractableObj.transform.GetComponent<Rigidbody>();
        currentInteractableObj.transform.SetParent(null);

        rb.useGravity = true;
        currentInteractableObj = null;
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<InteractableObject>() == null) return;
        Debug.Log("trigger enter");
        currentInteractableObj = other.transform.GetComponent<InteractableObject>();
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.transform.GetComponent<InteractableObject>() == null) return;
        Debug.Log("trigger exit");
        currentInteractableObj = null;
    }
}
