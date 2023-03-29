using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TableOfContentManager : MonoBehaviour
{
    public GameObject scrollViewContent;
    public GameObject buttonTemplate;

    private int _btnCount = 10;
    void Start()
    {
        for (int i = 0; i < _btnCount; i++)
        {
            var btn = (GameObject)Instantiate(buttonTemplate, scrollViewContent.transform);
            btn.GetComponentInChildren<TMP_Text>().text = i.ToString();
        }
    }

    
}
