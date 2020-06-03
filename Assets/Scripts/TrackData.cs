using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackData : MonoBehaviour
{
    public AudioClip track;
    public string detail;
    public Sprite sprite;
    public TextAsset moveCSV;
    public TextAsset attackCSV;


    public AudioClip GetTrack()
    {
        return track;
    }
    public string GetDetail()
    {
        return detail;
    }
    public Sprite GetSprite()
    {
        return sprite;
    }

    public TextAsset GetMoveCSV()
    {
        return moveCSV;
    }

    public TextAsset GetAttackCSV()
    {
        return attackCSV;
    }
}
