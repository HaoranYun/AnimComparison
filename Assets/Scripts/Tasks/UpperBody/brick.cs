using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brick : MonoBehaviour
{
    
    private WaitForSeconds waitFor1Second = new WaitForSeconds(1.0f);


    public Material afterCollision;
    private Material beforeCollision;
    private MeshRenderer myMesh;


    private UpperBodyTask upperBodyTask;
    // Start is called before the first frame update
    void Start()
    {
        upperBodyTask = GameObject.Find("upper body task").GetComponent<UpperBodyTask>();
        myMesh = this.transform.GetComponent<MeshRenderer>();
        beforeCollision = myMesh.sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collider)
    {
        // collisions happen between [interactable cube - brick] and [avatar collider - brick]
        if (collider.GetComponent<InteractableObject>() != null|| collider.tag == "Avatar")
        {
            
            upperBodyTask.BrickCollision(this.transform.gameObject);
 
        }
      
    }

    private void OnTriggerExit(Collider collider)
    {
        // collisions happen between [interactable cube - brick] and [avatar collider - brick]
        if (collider.GetComponent<InteractableObject>() != null || collider.tag == "Avatar")
        {

            upperBodyTask.BrickCollision(this.transform.gameObject);

        }

    }

    // Each brick change color in 
}
