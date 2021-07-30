using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarCollider : MonoBehaviour
{
    // Start is called before the first frame update
   

    public Transform currentAvatar;
    public Vector3 offsetFemale;
    public Vector3 offsetMale;

    private Player player;
    private Vector3 offset;


    private Dictionary<string, List<Transform>> avatarCollidersDict = new Dictionary<string, List<Transform>>();
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        if (player.playerGender == Gender.Female) offset = offsetFemale;
        else offset = offsetMale;
        InitColliders();
        FindBones(currentAvatar); //temp
    }



    // Update is called once per frame
    void Update()
    {

        UpdateColliders();
        //foreach (KeyValuePair<string, List<Transform>> pair in avatarCollidersDict)
        //{
        //    if (pair.Key == "LeftFoot" || pair.Key == "RightFoot"||
        //        pair.Key == "LeftArm" || pair.Key == "RightArm")
        //    {
        //        //update position and rotation
        //        // temp 
        //        pair.Value[0].position = pair.Value[1].position;
        //        pair.Value[0].rotation = pair.Value[1].rotation;


        //    }
        //    else
        //    {
        //        //update local rotation
        //        pair.Value[0].localRotation = pair.Value[1].localRotation;
        //    }
        //}
    }

    public void InitColliders()
    {
        foreach (Transform t in this.transform.GetComponentsInChildren<Transform>())
        {
            if (t == this.transform) continue;
           
            avatarCollidersDict.Add(t.gameObject.name, new List<Transform> { t });
        }
    }



    public void FindBones(Transform avatar)
    {
        currentAvatar = avatar;
        foreach(Transform bone in currentAvatar.GetChild(1).GetComponentsInChildren<Transform>())
        {
            if (bone.gameObject.name == "Camera") continue;

            string[] fullNameOfBone = bone.gameObject.name.Split(':');
            string name = "";
            if (fullNameOfBone.Length < 2) continue;
            else name = fullNameOfBone[1];
            if (avatarCollidersDict.ContainsKey(name))
            {
                
                avatarCollidersDict[name].Add(bone);
            }
        }
    }

    private void UpdateColliders()
    {
        foreach (KeyValuePair<string, List<Transform>> pair in avatarCollidersDict)
        {
            string name = pair.Key;
            Transform collider = pair.Value[0];
            Transform joint = pair.Value[1];
            if (name == "LeftFoot" || name == "RightFoot")
            {
                //update position and rotation
                // temp 
           
                Vector3 j1 = joint.TransformPoint(offset);
                Transform j2 = joint.GetChild(0);
                MatchCollider(VectorBetweenJoints(j1, j2.position), j1, collider);
                //collider.rotation = Quaternion.FromToRotation(collider.up, j1.up) * collider.rotation;
                //collider.position = j1.position + joint.TransformPoint(offset);
                //MatchCollider(joint.position, joint, collider);

            }
            else if (name == "LeftHand" || name == "RightHand")
            {
                //hands
                //update local rotation
                Transform j1 = joint;
                //hardcode
                Transform j2 = joint.GetChild(1); //todo: remember to check male avatar's finger
                collider.position = j2.position;
                collider.rotation = Quaternion.LookRotation(VectorBetweenJoints(j1.position, j2.position)) * Quaternion.Euler(90.0f, 0.0f, 0.0f); ;
                //MatchCollider(j1.position, j1, collider);
            }
            else
            {
                Transform j1 = joint;
                Transform j2 = joint.GetChild(0);
                MatchCollider(VectorBetweenJoints(j1.position, j2.position), j1.position, collider);
            }
            

        }
    }

    private Vector3 VectorBetweenJoints(Vector3 j1, Vector3 j2)
    {
       
        return j2 - j1;
    }

    private void MatchCollider(Vector3 VBetweenJoints, Vector3 j1, Transform collider)
    {
        collider.position = j1 + VBetweenJoints/2;
        // need to test
        collider.rotation = Quaternion.LookRotation(VBetweenJoints) * Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }
}
