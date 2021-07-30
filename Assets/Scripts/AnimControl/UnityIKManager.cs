using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class UnityIKManager : MonoBehaviour


{
    [HideInInspector]
    public GameObject HMD,TrackerRoot,TrackerLeft,TrackerRight,ControllerModelR,ControllerModelL;

    private Animator Animator;
    private Quaternion InitialLocalHipsRotation;
    private Quaternion InitialWorldSpineRotation;
    private Quaternion InitialWorldNeckRotation;
    private float InitialHipsY;

    private Vector3 HipsOffset;
    private Vector3 ReferenceVectorSpine;
    private float ForwardRootOffset;

    public GameObject TrackerRootChild, TrackerLeftChild, TrackerRightChild;
    public GameObject AvatarRoot;

    public float offsetFromFinalIK;
    //public GameObject DuplicateModels, TrackerModelPrefab, HMDModelPrefeb, ControllerModelPrefab;
    //private GameObject controllerModelL, controllerModelR, trackerModelL, trackerModelR, HMDModel;

    private bool readyToUpdate = false;

    

    public Transform name;

    
    public GameObject avatar;


    private float avatarSize;

    [HideInInspector]
    public AnimController AnimController;


    private void Start()
    {

        Animator = transform.gameObject.GetComponent<Animator>();
        
        InitialLocalHipsRotation = Animator.GetBoneTransform(HumanBodyBones.Hips).localRotation;
        InitialWorldSpineRotation = Animator.GetBoneTransform(HumanBodyBones.Spine).rotation;
        InitialWorldNeckRotation = Animator.GetBoneTransform(HumanBodyBones.Head).rotation;
        name.position = name.position + new Vector3(offsetFromFinalIK, 0, 0);

        Debug.Log("ANIMATOR");


        ControllerModelL = AnimController.DevicesDict["ControllerModelL"];
        ControllerModelR = AnimController.DevicesDict["ControllerModelR"];
        TrackerRoot = AnimController.DevicesDict["TrackerRoot"];
        TrackerRight = AnimController.DevicesDict["TrackerRight"];
        TrackerLeft = AnimController.DevicesDict["TrackerLeft"];
        HMD = AnimController.DevicesDict["HMD"];
    }


    private void OnAnimatorIK(int layerIndex)
    {

        
        HipsOffset = TrackerRootChild.transform.position - Animator.GetBoneTransform(HumanBodyBones.Hips).position;

        

        // LeftHand end-effector
        if (ControllerModelL != null)
        {
            
            Vector3 pos = ControllerModelL.transform.position - ControllerModelL.transform.forward * 0.175f;
            Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            Animator.SetIKPosition(AvatarIKGoal.LeftHand, pos - HipsOffset);
            Animator.SetIKRotation(AvatarIKGoal.LeftHand, ControllerModelL.transform.rotation);
        }

        // RightHand end-effector
        if (ControllerModelR != null)
        {
            Vector3 pos = ControllerModelR.transform.position - ControllerModelR.transform.forward * 0.175f;
            Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            Animator.SetIKPosition(AvatarIKGoal.RightHand, pos - HipsOffset);
            Animator.SetIKRotation(AvatarIKGoal.RightHand, ControllerModelR.transform.rotation);
        }

        // LeftFoot end-effector
        if (TrackerLeftChild != null)
        {
            Vector3 pos = TrackerLeftChild.transform.position - TrackerLeftChild.transform.forward * 0.17f;
            Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            Animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            Animator.SetIKPosition(AvatarIKGoal.LeftFoot, pos - HipsOffset);
            Animator.SetIKRotation(AvatarIKGoal.LeftFoot, TrackerLeftChild.transform.rotation);
        }

        // RightFoot end-effector
        if (TrackerRightChild != null)
        {
            Vector3 pos = TrackerRightChild.transform.position - TrackerRightChild.transform.forward * 0.17f;
            Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            Animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            Animator.SetIKPosition(AvatarIKGoal.RightFoot, pos - HipsOffset);
            Animator.SetIKRotation(AvatarIKGoal.RightFoot, TrackerRightChild.transform.rotation);
        }

        //Root end-effector - NOT REALLY AN END - EFFECTOR, INSTEAD WE CHANGE THE OVERALL BODY TRANSFORM
        if (TrackerRootChild)
        {
            Transform hipsTransform = Animator.GetBoneTransform(HumanBodyBones.Hips);
            Animator.SetBoneLocalRotation(HumanBodyBones.Hips, (Quaternion.Inverse(hipsTransform.parent.rotation) * TrackerRootChild.transform.rotation) * InitialLocalHipsRotation);
        }

        // Spine
        Quaternion bodyRotation = TrackerRootChild.transform.rotation;
        Vector3 centerHead = HMD.transform.position - HMD.transform.forward * 0.12f;
        Vector3 rootToNeck = Quaternion.Inverse(bodyRotation) * (centerHead - TrackerRoot.transform.position);
        rootToNeck.Normalize();

        Quaternion newSpineWorldRot = Quaternion.FromToRotation(ReferenceVectorSpine, rootToNeck) * InitialWorldSpineRotation;
        Animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Inverse(Animator.GetBoneTransform(HumanBodyBones.Spine).transform.parent.rotation) * newSpineWorldRot);

        // Neck
        Quaternion newNeckWorldRot = HMD.transform.rotation * InitialWorldNeckRotation;
        Animator.SetBoneLocalRotation(HumanBodyBones.Head, Quaternion.Inverse(bodyRotation * Animator.GetBoneTransform(HumanBodyBones.Head).transform.parent.rotation) * newNeckWorldRot);
    }

    private void LateUpdate()
    {
        // Root
        if (TrackerRootChild)
        {

            Vector3 pos = TrackerRootChild.transform.position;
            pos.x = pos.x + offsetFromFinalIK;
            Vector3 offset = Animator.GetBoneTransform(HumanBodyBones.Hips).rotation * (Vector3.forward * ForwardRootOffset);
            Animator.GetBoneTransform(HumanBodyBones.Hips).position = pos + offset;
            //Animator.GetBoneTransform(HumanBodyBones.Hips).rotation = TrackerRootChild.transform.rotation;
            //Animator.GetBoneTransform(HumanBodyBones.Hips).rotation = Animator.GetBoneTransform(HumanBodyBones.Hips).rotation * new Quaternion(0, 180, 0, 0);
        }

        if (readyToUpdate)
        {
            // Tracker Children
            TrackerRootChild.transform.localRotation = Quaternion.Inverse(TrackerRoot.transform.localRotation);
            TrackerLeftChild.transform.localRotation = Quaternion.Inverse(TrackerLeft.transform.localRotation);
            TrackerRightChild.transform.localRotation = Quaternion.Inverse(TrackerRight.transform.localRotation);
            // Calibration
            Calibration();
            // ReferenceVectorSpine
            Vector3 centerHead = HMD.transform.position - HMD.transform.forward * 0.12f;
            ReferenceVectorSpine = Quaternion.Inverse(Animator.bodyRotation) * (centerHead - TrackerRoot.transform.position);
            ReferenceVectorSpine.Normalize();

            StopAllCoroutines();
            StartCoroutine(ComputeForwardRootOffset());

            readyToUpdate = false;
        }
    }


    public void Calibration(float size)
    {
        avatarSize = size;
        readyToUpdate = true;
        
    }

    private void Calibration()
    {
        Vector3 rootPos = Animator.GetBoneTransform(HumanBodyBones.Hips).position;
        float headY = HMD.transform.GetChild(0).transform.position.y;
        //float sizeF = (headY - rootPos.y) / (Animator.GetBoneTransform(HumanBodyBones.Head).position.y - Animator.GetBoneTransform(HumanBodyBones.Hips).position.y);

        
        this.transform.localScale = new  Vector3(avatarSize, avatarSize, avatarSize) ;

        
    }

    private IEnumerator ComputeForwardRootOffset()
    {
        // Init
        const float length = 0.2f;
        const float step = 0.005f;
        const int count = (int)(length / step);
        Vector3[] initialPos = new Vector3[count];
        float[] maxDistances = new float[count];
        Transform rootTransform = Animator.GetBoneTransform(HumanBodyBones.Hips);
        for (int i = 0; i < initialPos.Length; ++i)
        {
            initialPos[i] = rootTransform.position + rootTransform.TransformDirection(Vector3.forward * step * (i + 1));
        }
        // Compute max distance
        const int minNumPoints = 40;
        const float minDistanceRootPoints = 0.02f;
        int numPoints = 0;
        Vector3 lastRootPos = new Vector3(-10000.0f, -10000.0f, -10000.0f);
        while (numPoints < minNumPoints)
        {
            if (Vector3.Distance(lastRootPos, rootTransform.position) > minDistanceRootPoints)
            {
                for (int i = 0; i < initialPos.Length; ++i)
                {
                    Vector3 newPos = rootTransform.position + rootTransform.TransformDirection(Vector3.forward * step * (i + 1)); ;
                    maxDistances[i] = Mathf.Max(maxDistances[i], Vector3.Distance(newPos, initialPos[i]));
                }
                numPoints += 1;
                lastRootPos = rootTransform.position;
            }
            yield return null;
        }
        // Find point with minimum change
        float min = maxDistances[0];
        int minIndex = 0;
        for (int i = 1; i < count; ++i)
        {
            if (min > maxDistances[i])
            {
                min = maxDistances[i];
                minIndex = i;
            }
        }
        ForwardRootOffset = step * (minIndex + 1);
        Debug.Log("Calibration completed! ForwardRootOffset: " + ForwardRootOffset);
    }
}
