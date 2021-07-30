using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using xsens;

public class Director : MonoBehaviour
{
    // Start is called before the first frame update
    [Tooltip("Avatars")]
    public GameObject avartarUnityIK, avartarFinalIK, avartarXSens, avartarXSensPlus;
    public GameObject cameraUnityIK, cameraFinalIK, cameraXSensPlus;
    public GameObject HMD;
    private GameObject[] cameras;
    
    private List<GameObject> avartars = new List<GameObject>();

    private List<Vector3> initialPos = new List<Vector3>();
    private HTCRecorder HTCRecorder;
    private XsAnimationRecorder XsAnimationRecorder;
    public SteamVR_Input_Sources rightHand;
    public bool Playback;
    private bool recording = false;

    public enum cameraState {
        UnityIK,
        FinalIK,
        XSensPlus
    }
    public int currentCameraSate =1;

    private float timer = 0f;
      public bool startMark = false;
    private Transform xsensTransform;
    public AvatarCollider AvatarCollider;
    void Start()
    {
        avartars.Add(avartarUnityIK);
        avartars.Add(avartarFinalIK);
        avartars.Add(avartarXSens);
        avartars.Add(avartarXSensPlus);

        HTCRecorder = avartarFinalIK.GetComponent<HTCRecorder>();
        
        XsAnimationRecorder = avartarXSensPlus.GetComponent<XsAnimationRecorder>();


        initialPos.Add(avartarUnityIK.transform.position);
        initialPos.Add(avartarFinalIK.transform.position);
        initialPos.Add(avartarXSens.transform.position);
        initialPos.Add(avartarXSensPlus.transform.position);

        cameras = new GameObject[] { cameraUnityIK, cameraFinalIK, cameraXSensPlus };
        xsensTransform = avartarXSensPlus.transform;

        //AvatarCollider.FindBones(avartars[currentCameraSate].tra);
    }

    // Update is called once per frame
    void Update()
    {
        if (SteamVR_Input.GetStateDown("switchPOV", rightHand))
        {
            Debug.Log("change my camera");

            cameras[currentCameraSate].GetComponent<Camera>().enabled = false;
            currentCameraSate += 1;
            if (currentCameraSate > 2) currentCameraSate = 0;
            cameras[currentCameraSate].GetComponent<Camera>().enabled = true;



            //if (!Playback)
            //{
            //    if (  !recording)
            //    {
            //        HTCRecorder.StartRecord();
            //        StartCoroutine(XsAnimationRecorder.Record());
            //    }
            //    else
            //    {
            //        HTCRecorder.EndRecord();
            //        XsAnimationRecorder.SaveRecording();
            //    }

            //    recording = !recording;

            //}

            //else StartPlayback();
        }

        if (currentCameraSate != 1) cameras[currentCameraSate].transform.rotation = HMD.transform.rotation;

        if (startMark) {
            Debug.Log("xsens pos " + xsensTransform.position);
        }

    }

    private void LateUpdate()
    {
       
        
    }

    private void StartPlayback()
    {
        HTCRecorder.playback = true;
        HTCRecorder.PlayRecordedHTCInput();

    }
}
