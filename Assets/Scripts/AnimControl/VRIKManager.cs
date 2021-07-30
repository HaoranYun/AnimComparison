using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RootMotion.FinalIK;
using Valve.VR;
using System;

namespace RootMotion.Demos
{ 
public class VRIKManager : MonoBehaviour
{
    


        //[HideInInspector]
        //public GameObject HMD, TrackerRoot, TrackerLeft, TrackerRight, ControllerRight, ControllerLeft;

        

        //public GameObject ControllerModelR, AvatarHandDuplicant; //for the calibration
        //public GameObject AvatarRootDuplicant;
        
        public float AvatarSize;
        public AnimController AnimController;


        public GameObject avatar;
        private VRIKCalibrationController VRIKCalibrationController;
        private VRIK VRIK;

        
        
        //root lf rf
        void Start()
        {
            //BVHRecorder = avatar.GetComponent<BVHRecorder>();
            VRIKCalibrationController = avatar.GetComponent<VRIKCalibrationController>();
            VRIK = avatar.GetComponent<VRIK>();

            
            
        }

 
        private void Update()
        {
            FixModelsPosition();
        }


        // bool parameter male just because the while loop won't return
        // when the avatar is male
        public float Calibration(bool male)
        { 
            if(male)
            {
                AvatarSize = VRIKCalibrationController.Calibration();
                return AvatarSize;
            }
            while (!CheckHandsRotation()) AvatarSize = VRIKCalibrationController.Calibration();

            return AvatarSize;
        }



        private bool CheckHandsRotation()
        {
            
            return Vector3.Dot(AnimController.DevicesDict["ControllerModelR"].transform.forward, AnimController.DevicesDict["AvatarHandDuplicantR"].transform.up) > 0.95f;
        }


    
        public void FixModelsPosition()
        {
            // let the skeleton follow root position
            AnimController.DevicesDict["AvatarRootDuplicant"].transform.position = AnimController.DevicesDict["TrackerRoot"].transform.position;

        }
    }


}
