using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{

    private LowerBodyTask lowerBodyTask;
    public Vector3 pos1,pos2;
    public bool atPos1;


    public bool ready = false;
    void Start()
    {
        atPos1 = true;
        
        lowerBodyTask = GameObject.Find("lower body task").GetComponent<LowerBodyTask>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.tag == "Avatar" && !lowerBodyTask.waitingNextRound && ready)
    //    {
    //        Debug.Log("hit end point " + collision.collider.name);
    //        StartCoroutine(lowerBodyTask.NextRound());

    //        if (atPos1)
    //        {
    //            transform.localPosition = pos2;
    //            atPos1 = false;
    //        }
    //        else
    //        {
    //            transform.localPosition = pos1;
    //            atPos1 = true;
    //        }
    //    }
    //}


        public void MovePosition()
    {
        if (atPos1)
        {
            transform.localPosition = pos2;
            atPos1 = false;
        }
        else
        {
            transform.localPosition = pos1;
            atPos1 = true;
        }
    }
    public void ResetEndPoint()
    {

        this.transform.gameObject.SetActive(true);
        atPos1 = true;
        transform.localPosition = pos1;
    }

    public void Disappare()
    {
        this.transform.gameObject.SetActive(false);
    }
}
