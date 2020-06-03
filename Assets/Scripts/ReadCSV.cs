using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadCSV
{ 
    public string[] ReadFile( TextAsset t)
    {
        string[] s = t.text.Split(',');
        return s;
    }
}
