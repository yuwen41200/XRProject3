using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ReadCSV : MonoBehaviour
{
    public TextAsset t;
    // Start is called before the first frame update
    void Start()
    {
        string[] s = t.text.Split(',');
        foreach (string a in s)
            Debug.Log(a);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReadFile()
    {

    }
}
