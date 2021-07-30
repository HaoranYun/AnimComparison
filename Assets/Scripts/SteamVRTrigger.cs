using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class SteamVRTrigger : MonoBehaviour
{


    public SteamVR_Action_Boolean grabAction;

    

    public SteamVR_Behaviour_Pose pose1;
    public SteamVR_Behaviour_Pose pose2;
    


    private Player player;

    private AnimController animController;

    private TaskManager taskManager;

    [HideInInspector]
    // calibration first and then other tasks
    public bool calibrationFinished = false;

   

    // Start is called before the first frame update
    void Start()
    {

        
        player = GameObject.Find("Player").GetComponent<Player>();


        animController = player.animController;

        taskManager = player.taskManager;

        pose1 = animController.DevicesDict["ControllerLeft"].GetComponent<SteamVR_Behaviour_Pose>();
        pose2  = animController.DevicesDict["ControllerRight"].GetComponent<SteamVR_Behaviour_Pose>();

       
    }

    // Update is called once per frame
    void Update()
    {



        // using default vive controller setting "GrapPinch" boolean --> for interaction

        if (grabAction.GetLastStateDown(pose1.inputSource)|| grabAction.GetLastStateDown(pose2.inputSource))
        {
            //1. calibaration
            
            if (!calibrationFinished)
            {Debug.Log("CALIBARATION");
                animController.Calibaration();
            }
            else
            {
                switch (player.taskManager.currentTaskType)
                {
                    case (TaskType.LowerBody):
                        break;
                    case (TaskType.UpperBody):
                        player.taskManager.UpperBodyPickAction();
                        break;
                    case (TaskType.FullBody):
                        player.taskManager.FullBodyMatchAction();
                        break;

                }
            }
        }

        if (grabAction.GetLastStateUp(pose1.inputSource)|| grabAction.GetLastStateUp(pose2.inputSource))
        {
            if (calibrationFinished &&player.taskManager.currentTaskType == TaskType.UpperBody)
                player.taskManager.UpperBodyDropAction();
        }
    }

   


}