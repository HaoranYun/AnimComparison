using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LowerBodyTask : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject[] stones;
    private GameObject[] spikes;

    private float timer;

    private bool roundRunnign;

    private int idx = 0;

    public Material afterCollision;
    public Material beforeCollision;

    [SerializeField]
    private GameObject[] rounds;
    public bool waitingNextRound = false;

    public EndPoint EndPoint;

    public SteamVR_Action_Boolean grabAction = null;
    private SteamVR_Behaviour_Pose pose = null;
    public SteamVR_Input_Sources leftHand;
    public SteamVR_Input_Sources rightHand;


    public WaitForSeconds waitFor = new WaitForSeconds(1.0f);
    internal TaskManager taskManager;

    void Start()
    {
        //tasksCopies = new GameObject[tasks.Length];
        //for(int i = 0; i < tasks.Length; i ++)
        //for(int i = 0; i < tasks.Length; i ++)
        //{
        //    tasksCopies[i] = Instantiate(tasks[i]);
        //    tasksCopies[i].transform.parent = tasks[i].transform.parent;
        //    tasksCopies[i].SetActive(false);
        //    tasks[i].SetActive(false);
        //}
        rounds[idx].SetActive(true);
        pose = GetComponent<SteamVR_Behaviour_Pose>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SteamVR_Input.GetStateDown("GrabGrip", rightHand))
        {
            StartCoroutine(NextRound());

        }
    }

    public IEnumerator NextRound()
    {
        waitingNextRound = true;
        
        rounds[idx].SetActive(false);
        idx++;
        if (idx >= rounds.Length)
        {
            idx--;
            FinishTask();
            yield break;
        }

        yield return new WaitForSeconds(1f);
        rounds[idx].SetActive(true);
        EndPoint.MovePosition();
        
    }



    public void SpikeCollision(GameObject spike)
    {
        waitingNextRound = true;
        Debug.Log("spike collision ");
        spike.GetComponent<MeshRenderer>().enabled = true;
        spike.GetComponent<MeshRenderer>().sharedMaterial = afterCollision;
        
        StartCoroutine(StartOver(spike));
    }

    public IEnumerator StartOver(GameObject spike)
    {
        Debug.Log("idx " + idx + " round" + spike.transform.parent.parent.parent.parent.name);
        yield return new WaitForSeconds(1f);
        rounds[idx].SetActive(false);
        EndPoint.Disappare();

        spike.GetComponent<MeshRenderer>().sharedMaterial = beforeCollision;
        spike.GetComponent<MeshRenderer>().enabled = false;
        idx = 0;
        rounds[idx].SetActive(true);
        EndPoint.ResetEndPoint();

        waitingNextRound = false;

       
    }

    private void FinishTask()
    {
        
    }

    public IEnumerator WaitForChangeColor()
    {
        yield return waitFor;


    }
}
