using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    // Start is called before the first frame update
    private LowerBodyTask  lowerBodyTask;
    void Start()
    {
        lowerBodyTask = GameObject.Find("lower body task").GetComponent<LowerBodyTask>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Avatar") lowerBodyTask.SpikeCollision(this.transform.gameObject);

    }
}
