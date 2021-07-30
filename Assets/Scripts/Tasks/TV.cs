using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TV : MonoBehaviour
{

    public Image displayImg;
    public Image binaryResultImg;
    public TextMeshProUGUI TVText;

    public Sprite starImg;
    public Sprite pass, fail;

    public void DisplayResultOnTV(bool isPass)
    {
        

        if (!isPass)
        {
            SetText("You can do better!");
            displayImg.sprite = fail;
            binaryResultImg.sprite = fail;
        }
        else
        {
            SetText("Done! Next!");
            displayImg.sprite = starImg;
            binaryResultImg.sprite = pass;
        }

    }

    public void SetText(string text)
    {
        TVText.text = text;
    }
}
