using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpperBodyTask : MonoBehaviour
{
    public GameObject[] brickCombs;
    public GameObject star;
    public GameObject starTarget;



    public ObjectPickup objectPickup;
    public TV TV;

    


    public bool waitingNextRound = false;

    public Material afterCollision;
    public Material beforeCollision;

    public InteractableObject[] interactableOjects;


    private WaitForSeconds waitFor5Seconds = new WaitForSeconds(5.0f);
    private int idx = 0;  //for rounds
    private Vector3 initPosition;
    public TaskManager taskManager;





    // Start is called before the first frame update
    void Start()
    {
        initPosition = star.transform.position;
        brickCombs[idx].SetActive(true);

        beforeCollision = brickCombs[0].GetComponent<MeshRenderer>().sharedMaterial;

        objectPickup.SetObjectsList(interactableOjects);
    }

    

    public IEnumerator ShowResult()
    {
        waitingNextRound = true;
        Debug.Log("Hit target");
        TV.DisplayResultOnTV(true);
        yield return waitFor5Seconds;
        NextRound();

    }

    private void NextRound()
    {
        if (idx >= brickCombs.Length)
        {
            FinishUpperBodyTassk();
            return;
        }

        brickCombs[idx].SetActive(false);
        idx++;
        if (idx >= brickCombs.Length) return;
        brickCombs[idx].SetActive(true);

        objectPickup.Drop();
        star.transform.position = initPosition;
        waitingNextRound = false;
    }




    public void BrickCollision(GameObject brick)
    {
        TV.DisplayResultOnTV(false);
    }

    public void FinishUpperBodyTassk()
    {
        TV.DisplayResultOnTV(true);
        TV.SetText("Perfect! Let's go to another Task");
        // todo: call task manager to switching the task
    }
        
}
