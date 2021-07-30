using RootMotion.FinalIK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTCRecorder : MonoBehaviour
{

    private static uint CAPTURE_NUM_POINTS_MAX = 12000;
    private  bool capturing = false; // Control both buffers

    public  GameObject hmd, controllerR, controllerL, trackerRoot, trackerR, trackerL;

    public  Vector3[,] positionsTrackers = new Vector3[CAPTURE_NUM_POINTS_MAX, 6]; // experiments / trackers / points
    public  Quaternion[,] rotationsTrackers = new Quaternion[CAPTURE_NUM_POINTS_MAX, 6]; // experiments / trackers / points
    public  Vector3[] autoForwardHead = new Vector3[CAPTURE_NUM_POINTS_MAX];
    public  Vector3[] autoRightHead = new Vector3[CAPTURE_NUM_POINTS_MAX];
    public  Vector3[] autoDeviceA = new Vector3[CAPTURE_NUM_POINTS_MAX]; // Currently NOT working with 2 buffers
    public  Vector3[] autoDeviceB = new Vector3[CAPTURE_NUM_POINTS_MAX]; // Currently NOT working with 2 buffers



    public AvatarType avatarType; 
    public GameObject avatar;
    private  int index;
    private  int playIndex;
    private  int fileNum;
    public  bool playback = false;
    private VRIK vrik;
    void Start()
    {
        vrik = GetComponent<VRIK>();
        index = 0;
        playIndex = 0;
        fileNum = 0;
        
    }

    // Update is called once per frame
    void Update()
    {

       
    }


    void LateUpdate()
    {
        if (capturing)
        {
            StoreDevicesLocation();
        }

        if (playback) PlaybackCurrentFrame();
    }

    
    private void PlaybackCurrentFrame()
    {

        if (!playback) return;
        if(playIndex >= CAPTURE_NUM_POINTS_MAX)
        {
            //EndPlayback();
            return;
        } 
        
        

        hmd.transform.position = positionsTrackers[playIndex, 0] ;
        controllerL.transform.position = positionsTrackers[playIndex, 1];
        controllerR.transform.position = positionsTrackers[playIndex, 2];
        trackerRoot.transform.position = positionsTrackers[playIndex, 3];
        trackerL.transform.position = positionsTrackers[playIndex, 4];
        trackerR.transform.position = positionsTrackers[playIndex, 5];

        // Rotations
        hmd.transform.rotation = rotationsTrackers[playIndex, 0];
        controllerL.transform.rotation = rotationsTrackers[playIndex, 1];
        controllerR.transform.rotation = rotationsTrackers[playIndex, 2];
        trackerRoot.transform.rotation = rotationsTrackers[playIndex, 3];
        trackerL.transform.rotation = rotationsTrackers[playIndex, 4];
        trackerR.transform.rotation = rotationsTrackers[playIndex, 5];

        playIndex++;
    }

    private void EndPlayback()
    {
        playIndex = 0;
        fileNum++;
        playback = false;
    }

    private void StoreDevicesLocation()
    {

        if (index >= CAPTURE_NUM_POINTS_MAX) return;

        // Positions
        positionsTrackers[index, 0] = hmd.transform.position;
        positionsTrackers[ index, 1] = controllerL.transform.position;
        positionsTrackers[index, 2] = controllerR.transform.position;
        positionsTrackers[ index,3] = trackerRoot.transform.position;
        positionsTrackers[ index,4] = trackerL.transform.position;
        positionsTrackers[ index,5] = trackerR.transform.position;
        // Rotations
        rotationsTrackers[index,0] = hmd.transform.rotation;
        rotationsTrackers[ index,1] = controllerL.transform.rotation;
        rotationsTrackers[index,2] = controllerR.transform.rotation;
        rotationsTrackers[ index,3] = trackerRoot.transform.rotation;
        rotationsTrackers[index,4] = trackerL.transform.rotation;
        rotationsTrackers[ index,5] = trackerR.transform.rotation;
        // Forward & Right
        autoForwardHead[index] = hmd.transform.forward;
        autoRightHead[index] = hmd.transform.right;
        // Devices
        //autoDeviceA[autoCurrentExercise, index] = capturingDeviceA[0].transform.position - capturingDeviceA[0].transform.forward * 0.175f;
        //autoDeviceB[autoCurrentExercise, index] = capturingDeviceB[0].transform.position - capturingDeviceB[0].transform.forward * 0.175f;

        // Increment Index
        index++;

        if (index >= CAPTURE_NUM_POINTS_MAX) EndRecord();
    }

    public void  PlayRecordedHTCInput()
    {

        positionsTrackers =  Utils.ReadVector3sFromFile("positions" + fileNum.ToString() + ".txt", ".", (int)CAPTURE_NUM_POINTS_MAX, 6);
        rotationsTrackers =  Utils.ReadQuaternionsFromFile("rotations" + fileNum.ToString() + ".txt", ".", (int)CAPTURE_NUM_POINTS_MAX, 6);
        playback = true;
        
    }


    public void StartRecord()
    {
        Debug.Log("START RECORD");
        capturing = true;
    }

    public void EndRecord()
    {
        capturing = false;
        index = 0;
        
        Debug.Log("EMD RECORD");
        Utils.WriteVector3IntoFile("positions"+fileNum.ToString()+".txt", ".", positionsTrackers, (int)CAPTURE_NUM_POINTS_MAX, 6);
        Utils.WriteQuaternionsIntoFile("rotations" + fileNum.ToString() + ".txt", ".", rotationsTrackers,(int) CAPTURE_NUM_POINTS_MAX, 6);
        fileNum++;
        Debug.Log("SAVED");
    }

}

public enum AvatarType
{
    UnityIK,
    FinalIK,
    XSens,
    XSensPlus,
}
