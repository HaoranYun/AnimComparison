using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class Player : MonoBehaviour
{

    public string PlayerDataPath;
    private string[] subFolderNames = new string[] { "LowerBody", "UpperBody", "FullBody" };

    [Header("Player Data")]
    public string playerName;

    public Gender playerGender;

    public AnimControlType playerAnimControll;

    public TaskType[] taskOrder;

    public float size = 0;
    
    [SerializeField]
    [TextArea]
    private string notesForThisExperiment;



    [Header("Contoller Ref")]
    public AnimController animController;
  
    public TaskManager taskManager;

    public SteamVRTrigger steamVRTrigger;
    


    // Start is called before the first frame update

    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



   

}


