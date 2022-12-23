using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScore : MonoBehaviour
{
    Text kill_Text;

    void Start()
    {
        kill_Text = GameObject.Find("Background").transform.GetChild(0).GetComponent<Text>();
        kill_Text.text = "Kill : " + "<color=#ff0000>" + G_Manager.g_Manager.gameData.killCount.ToString() + "</color>";
        // kill_Text.text = "Kill : " + "<color=#ff0000>" + G_Manager.total.ToString() + "</color>";
    }
}
