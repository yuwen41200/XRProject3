using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackMenu : MonoBehaviour
{
    public GameManagement gamemanagement;
    public GameObject[] TrackBtns;

    private int selectIndex;

    public Text detail;
    public Image Img;

    // Start is called before the first frame update

    // 讀取歌曲的move和attack的CSV
    private ReadCSV readcsv;

    public TrackSelectionController trackSelectionController;
    //public bool enterButtonIsClicked;

    public AudioClip ac;

    private void Start()
    {
        readcsv = new ReadCSV();
        // OpenPanel();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.N))
        {
            ChooseNext();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            ChoosePrevious();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ConfirmTrack();
            // ClosePanel();
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            // OpenPanel();
        }
        TrackBtns[selectIndex].GetComponent<Button>().Select();
    }
    private void Init()
    {
        selectIndex = 0;
        TrackBtns[selectIndex].GetComponent<Button>().Select();
        ShowTrack();
        //enterButtonIsClicked = false;
    }

    public void ChooseNext()
    {
        selectIndex++;
        selectIndex %= TrackBtns.Length;
        TrackBtns[selectIndex].GetComponent<Button>().Select();
        ShowTrack();
        this.GetComponent<AudioSource>().Play();
    }

    public void ChoosePrevious()
    {
        selectIndex--;
        if (selectIndex < 0)
            selectIndex = TrackBtns.Length - 1;
        TrackBtns[selectIndex].GetComponent<Button>().Select();
        ShowTrack();
        this.GetComponent<AudioSource>().Play();
    }

    private void ShowTrack()
    {
        detail.text = TrackBtns[selectIndex].GetComponent<TrackData>().GetDetail();
        Img.sprite = TrackBtns[selectIndex].GetComponent<TrackData>().GetSprite();
    }

    public void ConfirmTrack()
    {

        TrackData td = TrackBtns[selectIndex].GetComponent<TrackData>();
        TrackBtns[selectIndex].GetComponent<Button>().Select();
        string[] moveCSV = readcsv.ReadFile(td.GetMoveCSV());
        string[] attackCSV = readcsv.ReadFile(td.GetAttackCSV());
        // set track audioclip
        // set track move
        // set track attack
        gamemanagement.SetTrack(td.GetTrack());
        gamemanagement.SetMoveCSV(moveCSV);
        gamemanagement.SetAttackCSV(attackCSV);
        trackSelectionController.enterButtonIsClicked = true;


        this.GetComponent<AudioSource>().clip = ac;
        this.GetComponent<AudioSource>().Play();
    }

}
