using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPoseCopier : MonoBehaviour
{
    public Transform mySkeleton;
    public Transform originalSkeleton;

    Dictionary<string, List<Transform>> dict = new Dictionary<string, List<Transform>>();

    public void Copy()
    {
        ChangeName();
        foreach(KeyValuePair<string, List<Transform>> pair in dict)
        {
            Debug.Log(pair.Key + " " +pair.Value.Count);
            pair.Value[1].rotation = pair.Value[0].rotation;
        }
    }

    public void ChangeName()
    {
        foreach (Transform t in originalSkeleton.GetComponentsInChildren<Transform>())
        {
            dict.Add(t.gameObject.name, new List<Transform> { t });

        }

        foreach (Transform t in mySkeleton.GetComponentsInChildren<Transform>())
        {
            if(dict.ContainsKey(t.gameObject.name))dict[t.gameObject.name].Add(t);
        }
    }
}
