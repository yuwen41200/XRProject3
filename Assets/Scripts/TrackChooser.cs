using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackChooser : MonoBehaviour
{
    public GameObject gamemanagement;
    public GameObject[] TrackBtns;

    private int selectIndex;

    public Text detail;
    public Image Img;

    [SerializeField]
    private GameObject panel;
    // Start is called before the first frame update

    private void Start()
    {
        OpenPanel();
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
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ConfirmTrack();
            ClosePanel();
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            OpenPanel();
        }
    }
    private void Init()
    {
        selectIndex = 0;
        TrackBtns[selectIndex].GetComponent<Button>().Select();
        ShowTrack();
    }

    public void ChooseNext()
    {
        selectIndex++;
        selectIndex %= TrackBtns.Length;
        TrackBtns[selectIndex].GetComponent<Button>().Select();
        ShowTrack();
    }

    public void ChoosePrevious()
    {
        selectIndex--;
        if (selectIndex < 0)
            selectIndex = TrackBtns.Length - 1;
        TrackBtns[selectIndex].GetComponent<Button>().Select();
        ShowTrack();
    }

    private void ShowTrack()
    {
        detail.text = TrackBtns[selectIndex].GetComponent<TrackData>().GetDetail();
        Img.sprite = TrackBtns[selectIndex].GetComponent<TrackData>().GetSprite();
    }

    public void ConfirmTrack()
    {
        TrackBtns[selectIndex].GetComponent<Button>().Select();
        // set track audioclip
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
}
