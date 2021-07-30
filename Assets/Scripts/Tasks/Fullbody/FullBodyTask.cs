using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Valve.VR;
using TMPro;
using RootMotion.FinalIK;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class FullBodyTask : MonoBehaviour
{

    public int version;
    public List<GameObject> WallsV1;
    public List<GameObject> WallsV2;
    public GameObject v1, v2;

    [Range(0, 1f)]
    public float movingSpeed;
    [Range(0, 1f)]
    public float passThreshold;
    [Range(0, 15f)]
    public float T_betweenRounds;

    public SteamVR_Input_Sources leftHand;
    public SteamVR_Input_Sources rightHand;

    [SerializeField]
    private ImageCollision collisionDetector;
    private bool movingShadowPlayer;

    public GameObject shadowPlayer;



    private int wallV1Idx = 0;


    public TV TV;

    private float collisionResult;
    private bool wallsV2Showing = false;
    private bool wallsV1Showing = false;

    private Transform currentSilhouette;
    private HumanPoseCopier Copier;
    private Player Player;
    internal TaskManager taskManager;

    void Start()
    {

        Copier = this.transform.GetComponentInChildren<HumanPoseCopier>();

        if (version == 1)
        {
            v1.SetActive(true);
            v2.SetActive(false);
        }

    }


    void Update()
    {



        if (SteamVR_Input.GetStateDown("Ready", rightHand))
        {
            Debug.Log("ready");
            GenerateShadowPlayer();

            movingShadowPlayer = true;
        }

        if (movingShadowPlayer)
        {
            Vector3 pos = shadowPlayer.transform.position;
            pos.z += movingSpeed;
            shadowPlayer.transform.position = pos;
            if (shadowPlayer.transform.position.z > currentSilhouette.position.z)
            {
                movingShadowPlayer = false;
                StartCoroutine(CollisionDetect());
            }
        }

    }

    private void GenerateShadowPlayer()
    {
        currentSilhouette = WallsV1[wallV1Idx].transform.GetChild(2);
        shadowPlayer = Instantiate(Player.animController.currentAvatar);
        shadowPlayer.transform.localRotation = new Quaternion(0, 0, 0, 0);

        shadowPlayer.GetComponent<VRIK>().enabled = false;
        Vector3 scale = currentSilhouette.localScale;
        scale.z = 0f;
        shadowPlayer.transform.localScale = scale;

        // only works when the avatar  is enabled
        if (Copier != null)
        {
            Copier.originalSkeleton = shadowPlayer.transform.GetChild(1);
            Copier.Copy();
            //still need to save the models manually
        }
    }

    public void SwitchVersion()
    {


        if (version == 1)
        {

            if (!wallsV2Showing)
            {
                v2.SetActive(true);
                v1.SetActive(false);
                //mirror.SetActive(true);

                wallsV2Showing = true;
                version = 2;

            }

        }
        else if (version == 2)
        {

            if (!wallsV1Showing)
            {
                v1.SetActive(true);
                v2.SetActive(false);


                WallsV1[wallV1Idx].SetActive(true);
                wallsV1Showing = true;
                version = 1;
            }


        }
    }



    IEnumerator CollisionDetect()
    {
        float IOU = -1f;
        Sprite sprite = null;
        Transform newShadow = shadowPlayer.transform;
        Vector3 originPosition = newShadow.position;
        for (float f = -0.5f; f < 0.5f; f += 0.01f)
        {
            newShadow.position = originPosition + new Vector3(f, 0, 0);
            float iou = collisionDetector.ComputeIntersection(TV.displayImg, shadowPlayer.transform.GetChild(0), currentSilhouette);
            if (iou > IOU)
            {
                IOU = iou;
                sprite = TV.displayImg.sprite;
            }
        }

        newShadow.position = originPosition;
        collisionResult = IOU;
        TV.displayImg.sprite = sprite;
        //collisionResult = 1-collisionDetector.ComputeIntersection(TVImage, shadowPlayer.transform.GetChild(0),currentSilhouette);
        shadowPlayer.SetActive(false);
        bool isPass = (collisionResult > passThreshold);
        TV.DisplayResultOnTV(isPass);


        yield return new WaitForSeconds(T_betweenRounds);
        Destroy(shadowPlayer);
        NextRound();

    }



    public void NextRound()
    {
        // put new pose to wall list
        WallsV1[wallV1Idx].SetActive(false);
        wallV1Idx++;
        if (wallV1Idx >= WallsV1.Count) return;
        WallsV1[wallV1Idx].SetActive(true);
    }



}

#if UNITY_EDITOR
[CustomEditor(typeof(FullBodyTask))]
public class FullbodyTaskEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Switch Side/Front Mirror"))
        {
            FullBodyTask fbt = (FullBodyTask)target;

            fbt.SwitchVersion();
        }
    }
}
#endif
