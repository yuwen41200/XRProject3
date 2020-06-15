using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionSelectionController : MonoBehaviour
{
    public GameManagement gamemanagement;
    public GameObject[] RegionBtns;
    public SceneManagement sceneManagement;

    public int selectIndex;

    [SerializeField]
    private GameObject panel;
    // Start is called before the first frame update

    public bool enterButtonIsClicked;
    public GameState selectedRegion;

    // by Kuan ,record the region, 
    public GameRegion trackRegion;

    // confirm music
    public AudioClip ac;

    private void Start()
    {
        Init();
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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ConfirmEnFr();
            // ClosePanel();
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            // OpenPanel();
        }
        RegionBtns[selectIndex].GetComponent<Button>().Select();
    }
    private void Init()
    {
        selectIndex = 0;
        RegionBtns[selectIndex].GetComponent<Button>().Select();
        ShowTrack();
        enterButtonIsClicked = false;
    }

    public void ChooseNext()
    {
        selectIndex++;
        selectIndex %= RegionBtns.Length;
        RegionBtns[selectIndex].GetComponent<Button>().Select();
        ShowTrack();
        this.GetComponent<AudioSource>().Play();
    }

    public void ChoosePrevious()
    {
        selectIndex--;
        if (selectIndex < 0)
            selectIndex = RegionBtns.Length - 1;
        RegionBtns[selectIndex].GetComponent<Button>().Select();
        ShowTrack();
        this.GetComponent<AudioSource>().Play();
    }

    private void ShowTrack()
    {
        // detail.text = TrackBtns[selectIndex].GetComponent<TrackData>().GetDetail();
        // Img.sprite = TrackBtns[selectIndex].GetComponent<TrackData>().GetSprite();
        
    }

    public void ConfirmEnFr()
    {

        /* TrackData td = TrackBtns[selectIndex].GetComponent<TrackData>();
        TrackBtns[selectIndex].GetComponent<Button>().Select();
        string[] moveCSV = readcsv.ReadFile(td.GetMoveCSV());
        string[] attackCSV = readcsv.ReadFile(td.GetAttackCSV());
        Debug.Log("123123");
        // set track audioclip
        // set track move
        // set track attack
        gamemanagement.SetTrack(td.GetTrack());
        gamemanagement.SetMoveCSV(moveCSV);
        gamemanagement.SetAttackCSV(attackCSV); */

        //this region is track region (control track panel)
        RecordTrackRegion();

        this.GetComponent<AudioSource>().clip = ac;
        this.GetComponent<AudioSource>().Play();
        StartCoroutine(CancelClip());
        // this region is game state region (control scene)
        selectedRegion = GameState.EnFrRegion;
        enterButtonIsClicked = true;

    }

    IEnumerator CancelClip()
    {
        yield return new WaitForSeconds(1);
        this.GetComponent<AudioSource>().clip = null;
    }

    public void ConfirmJpKr()
    {

        /* TrackData td = TrackBtns[selectIndex].GetComponent<TrackData>();
        TrackBtns[selectIndex].GetComponent<Button>().Select();
        string[] moveCSV = readcsv.ReadFile(td.GetMoveCSV());
        string[] attackCSV = readcsv.ReadFile(td.GetAttackCSV());
        Debug.Log("123123");
        // set track audioclip
        // set track move
        // set track attack
        gamemanagement.SetTrack(td.GetTrack());
        gamemanagement.SetMoveCSV(moveCSV);
        gamemanagement.SetAttackCSV(attackCSV); */
        selectedRegion = GameState.JpKrRegion;
        enterButtonIsClicked = true;

    }

    private void ClosePanel()
    {
        panel.SetActive(false);
    }

    IEnumerator Show()
    {
        yield return new WaitForEndOfFrame();
        Init();
    }

    private void OpenPanel()
    {
        panel.SetActive(true);
        StartCoroutine(Show());
    }

    private void RecordTrackRegion()
    {
        switch (selectIndex){
        case 0:
                trackRegion = GameRegion.England;
            break;
        case 1:
            trackRegion = GameRegion.France;
            break;
        case 2:
            trackRegion = GameRegion.Japan;
            break;
        case 3:
            trackRegion = GameRegion.England;
            break;
        default:
            trackRegion = GameRegion.England;
            break;
        }
    }

    public int GetTrackPanelIndex()
    {
        return (int)trackRegion;
    }
}
