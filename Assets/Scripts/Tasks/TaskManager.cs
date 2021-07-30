using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class TaskManager : MonoBehaviour
{
    // Start is called before the first frame update

    // lower; upper; full
    [Header("Task Settings")]
    public GameObject[] tasks;
    

    // status:
    // -1 not executed 
    // 0 doing 
    // 1 finished
    private int[] taskStatus;

    public TaskType currentTaskType;

    [HideInInspector]
    public TaskType[] taskOrder;

    

    public LowerBodyTask lowerBodyTask;
    public UpperBodyTask upperBodyTask;
    public FullBodyTask fullBodyTask;

    void Start()
    {
        lowerBodyTask.taskManager = this;
        upperBodyTask.taskManager = this;
        fullBodyTask.taskManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTaskOrder(TaskType[] assignedTasks)
    {
        taskOrder = new TaskType[assignedTasks.Length];
        for(int i = 0; i < assignedTasks.Length; i ++)
        {
            taskOrder[i] = assignedTasks[i];
        }
    }

    // reset all
    public void ResetTask()
    {
        // reset status
        for (int i = 0; i < taskStatus.Length; i++)
        {
            taskStatus[0] = -1;
        }

        //restore task objects
    }

    // finish lower/upper/fullbody task
    public void FinishTask()
    {
        
        // arrange next task in certain order (latin square)
        // order is assigned in Player script

    }



    private void StartLowerBodyTask()
    {

    }


    private void StartUpperBodyTask()
    {

    }

    public void UpperBodyPickAction()
    {
        upperBodyTask.objectPickup.Pick();
    }

    public void UpperBodyDropAction()
    {
        upperBodyTask.objectPickup.Drop();
    }

    

    private void StartFullBodyTask()
    {

    }

    public void FullBodyMatchAction()
    {
        
    }

    private void End()
    {

    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(TaskManager))]
public class TaskManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reset Task"))
        {
            TaskManager taskManager = (TaskManager)target;

            taskManager.ResetTask();
        }
    }
}
#endif