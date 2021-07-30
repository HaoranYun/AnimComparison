using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using Valve.VR;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimController : MonoBehaviour
{
    // Start is called before the first frame update

    public AnimControllerGroup maleControllerGroup;
    public AnimControllerGroup femaleControllerGroup;


    [HideInInspector]
    public AnimControllerGroup myControllerGroup;

    public GameObject currentAvatar;

    
    public AnimControlType currentControlType;


    public bool waitingCalibrationConfirm = true;

    [HideInInspector]
    public VRIKManager myVRIKManager;
    [HideInInspector]
    public UnityIKManager myUnityIKManager;
    [HideInInspector]
    public XSensManager myXSensManager;

    GameObject[] trackers = new GameObject[3];
    private int TrackerRootIndex;
    private int TrackerLeftIndex;
    private int TrackerRightIndex;
    private int[] trackerIndexes = new int[3] { -1, -1, -1 };

    private Player player;
    private SteamVRTrigger steamVRTrigger;

    [HideInInspector]
    public Dictionary<string, GameObject> DevicesDict = new Dictionary<string, GameObject>();
    
    [SerializeField]
    private GameObject MirrorForCalibration;

    void Start()
    {

        player = GameObject.Find("Player").GetComponent<Player>();
        if (player.playerGender == Gender.Female)
        {
            maleControllerGroup.ParentGameObject.SetActive(false);
            myControllerGroup = femaleControllerGroup;
        }

        else
        {
            femaleControllerGroup.ParentGameObject.SetActive(false);
            myControllerGroup = maleControllerGroup;
        }



        foreach(HTCDevice d in myControllerGroup.devices)
        {
            DevicesDict.Add(d.name, d.device);
            if(d.device==null) Debug.Log("DEVICES " + d.name);
        }
        

        InitiateAvatars();

        
       
        myVRIKManager = myControllerGroup.VRIKManager;
        myUnityIKManager = myControllerGroup.UnityIKManager;
        myXSensManager = myControllerGroup.XSensManager;

        steamVRTrigger = player.steamVRTrigger;
        waitingCalibrationConfirm = true;
    }
    

    public void InitiateAvatars()
    {
        
        myControllerGroup.VRIKManager.AnimController = this;
        myControllerGroup.UnityIKManager.AnimController = this;
        myControllerGroup.XSensManager.AnimController = this;

        SetUpTrackerIndexes();
    }



    void SetUpTrackerIndexes()
    {

        trackers[0] = DevicesDict["TrackerRoot"];
        trackers[1] = DevicesDict["TrackerLeft"];
        trackers[2] = DevicesDict["TrackerRight"];

        GameObject ControllerRight = DevicesDict["ControllerRight"];
        GameObject TrackerRight = DevicesDict["TrackerRight"];
        GameObject TrackerLeft = DevicesDict["TrackerLeft"];
        GameObject TrackerRoot = DevicesDict["TrackerRoot"];
       

        TrackedDevicePose_t[] trackedDevicePoses = new TrackedDevicePose_t[16];
        if (OpenVR.Settings != null)
        {
            OpenVR.System.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, trackedDevicePoses);
        }


        int pairedTracker = 0;
        for (uint i = 0; i < 16; ++i)
        {

            ETrackingResult status = trackedDevicePoses[i].eTrackingResult;
            var result = new System.Text.StringBuilder((int)64);
            var error = ETrackedPropertyError.TrackedProp_Success;
            if (SteamVR.active == false)
                return;

            // Dispatch any OpenVR events.
            var system = OpenVR.System;

            system.GetStringTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_RenderModelName_String, result, 64, ref error);
            //Debug.Log(result.ToString()+ " device " + i.ToString() + status.ToString());


            if (result.ToString().Contains("tracker"))
            {
                //Debug.Log("device " + i.ToString());
                trackers[pairedTracker].SetActive(true);
                trackerIndexes[pairedTracker] = (int)i;
                pairedTracker++;
            }

        }


        int pelvisTrackerIdx = -1;
        float posY = float.MinValue;
        float[] distances = new float[3];

        if (pairedTracker < 1) return;
        if (pairedTracker < 3) return;

        for (int i = 0; i < trackerIndexes.Length; i++)
        {
            var pose = new SteamVR_Utils.RigidTransform(trackedDevicePoses[trackerIndexes[i]].mDeviceToAbsoluteTracking);
            float pos = pose.pos.y;
            if (pos > posY)
            {
                posY = pos;
                pelvisTrackerIdx = trackerIndexes[i];
                trackers[0].GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)pelvisTrackerIdx;
                //TrackerRoot.transform.GetChild(0).localRotation = Quaternion.Inverse(pose.rot);
            }

        }

        if (pairedTracker < 3) return;

        float dist1 = -1f;
        float dist2 = -1f;
        int[] tempIdx = new int[2];
        for (int i = 0; i < trackerIndexes.Length; i++)
        {
            if (trackerIndexes[i] == pelvisTrackerIdx) continue;

            var pose = new SteamVR_Utils.RigidTransform(trackedDevicePoses[trackerIndexes[i]].mDeviceToAbsoluteTracking);
            Vector3 pos = pose.pos;
            if (dist1 < 0)
            {
                dist1 = Vector3.Distance(ControllerRight.transform.position, pos);
                tempIdx[0] = i;


            }
            else
            {

                dist2 = Vector3.Distance(ControllerRight.transform.position, pos);
                tempIdx[1] = i;

            }

        }


        if (dist1 < dist2) //the corresponding tracker is closer to right controller
        {
            TrackerRight.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)trackerIndexes[tempIdx[0]];
            TrackerLeft.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)trackerIndexes[tempIdx[1]];

        }
        else
        {
            TrackerRight.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)trackerIndexes[tempIdx[1]];
            TrackerLeft.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)trackerIndexes[tempIdx[0]];

        }


    }


    public void Calibaration()
    {
        Debug.Log("calibration begine");
        SetUpTrackerIndexes();
        //calibrate all avatars
        //vrik 
        player.size = myVRIKManager.Calibration(player.playerGender == Gender.Male);
        Debug.Log("size" + player.size);
        ////unity ik
        myUnityIKManager.Calibration(player.size);
        ////xsens
        myXSensManager.Calibration(player.size);

        //Debug.Log("calibration done" + player.size);

        //switch(player.playerAnimControll)
        //{
        //    case (AnimControlType.UnityIK):
        //        ChangeToUnityIK();
        //        break;
        //    case (AnimControlType.FinalIK):
        //        ChangeToFinalIK();
        //        break;
        //    case (AnimControlType.XSens):
        //        ChangeToXSens();
        //        break;
        //}

    }

    public void ManuallyConfirmCalibration()
    {
        if(waitingCalibrationConfirm)
        {
            MirrorForCalibration.SetActive(false);
            steamVRTrigger.calibrationFinished = true;
            waitingCalibrationConfirm = false;
        }
    }



    private void RenderModel(GameObject model, GameObject original,float offset)
    {
        Vector3 pos = original.transform.position;
        pos.x = pos.x + offset;
        model.transform.position = pos;

        Quaternion r = original.transform.rotation;

        if (model == DevicesDict["HMDModel"] || model == DevicesDict["ControllerModelR"] || model == DevicesDict["ControllerModelL"])
            model.transform.rotation = r * new Quaternion(0, 180, 0, 0);
        else model.transform.rotation = r;

    }

    public void ChangeToUnityIK()
    {
        myControllerGroup.AvatarUnityIK.transform.parent.gameObject.SetActive(true);
        myControllerGroup.AvatarFinalIK.transform.parent.gameObject.SetActive(false);
        myControllerGroup.AvatarXSens.transform.parent.gameObject.SetActive(false);
    }

    public void ChangeToFinalIK()
    {
        myControllerGroup.AvatarUnityIK.transform.parent.gameObject.SetActive(false);
        myControllerGroup.AvatarFinalIK.transform.parent.gameObject.SetActive(true);
        myControllerGroup.AvatarXSens.transform.parent.gameObject.SetActive(false);
    }

    public void ChangeToXSens()
    {
        myControllerGroup.AvatarUnityIK.transform.parent.gameObject.SetActive(false);
        myControllerGroup.AvatarFinalIK.transform.parent.gameObject.SetActive(false);
        myControllerGroup.AvatarXSens.transform.parent.gameObject.SetActive(true);
    }
}


[System.Serializable]
public class AnimControllerGroup
{
    public GameObject ParentGameObject;
    public GameObject AvatarUnityIK, AvatarFinalIK, AvatarXSens;
    public List<HTCDevice> devices;

    //[Header("TRACKERS")]
    //public GameObject HMD;
    //public GameObject TrackerRoot, TrackerLeft, TrackerRight, ControllerRight, ControllerLeft, 
    //    HMDModel,ControllerModelL, ControllerModelR, AvatarHandDuplicant, AvatarRootDuplicantR;

    [Header("Contoller Ref")]
    public VRIKManager VRIKManager;
    public UnityIKManager UnityIKManager;
    public XSensManager XSensManager;

}


[System.Serializable]
public struct HTCDevice
{
    public string name;
    public GameObject device;
}

#if UNITY_EDITOR
[CustomEditor(typeof(AnimController))]
public class AnimControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Confirm Calibration"))
        {
            AnimController controller = (AnimController) target;

            controller.ManuallyConfirmCalibration();
        }

        if (GUILayout.Button("ChangeToUnityIK"))
        {
            AnimController controller = (AnimController)target;

            controller.ChangeToUnityIK();
        }
        if (GUILayout.Button("ChangeToFinalIK"))
        {
            AnimController controller = (AnimController)target;

            controller.ChangeToFinalIK();
        }
        if (GUILayout.Button("ChangeToXSens"))
        {
            AnimController controller = (AnimController)target;

            controller.ChangeToXSens();
        }
    }
}
#endif
