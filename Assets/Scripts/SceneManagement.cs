using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum GameRegion{
    England, France, Japan, Korea
}

public enum GameState {
    Entry, RegionSelection, TrackSelection, EnFrRegion, JpKrRegion, GameEnded
}

public class SceneManagement : MonoBehaviour {

    [SerializeField]
    private GameState gameState;
    [SerializeField] private Image blackScreen;
    private bool isFading;

    [SerializeField] private GameObject entryPanel;
    [SerializeField] private EntryController entryController;
    [SerializeField] private GameObject regionSelectionPanel;
    [SerializeField] private RegionSelectionController regionSelectionController;
    [SerializeField] private GameObject[] trackSelectionPanel;
    [SerializeField] private TrackSelectionController trackSelectionController;

    [SerializeField] private GameObject enFrScene;
    [SerializeField] private GameObject jpKrScene;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject coordinateMarks;

    [SerializeField] private GameManagement gameManagement;

    // by Kuan, record the region choosed, used to choose track panel
    private int regionIndex;

    private void Awake() {

        gameState = GameState.Entry;
        var resetColor = blackScreen.color;
        resetColor.a = 0;
        blackScreen.color = resetColor;
        isFading = false;

        // entryController = entryPanel.GetComponent<EntryController>();
        // regionSelectionController = regionSelectionPanel.GetComponent<RegionSelectionController>();
        // trackSelectionController = trackSelectionPanel.GetComponent<TrackSelectionController>();

        entryPanel.SetActive(true);
        regionSelectionPanel.SetActive(false);

        //trackSelectionPanel.SetActive(false);
        // by Kuan
        foreach (GameObject g in trackSelectionPanel)
            g.SetActive(false);

        enFrScene.SetActive(false);
        jpKrScene.SetActive(false);
        gameUI.SetActive(false);
        coordinateMarks.SetActive(false);

    }

    private void LateUpdate() {
        switch (gameState) {

            case GameState.Entry:
                if (!isFading && entryController.enterButtonIsClicked) {
                    entryController.enterButtonIsClicked = false;
                    gameState = GameState.RegionSelection;
                    StartCoroutine(Fade(entryPanel, new []{regionSelectionPanel}));
                }
                break;

            case GameState.RegionSelection:
                if (!isFading && regionSelectionController.enterButtonIsClicked) {
                    regionSelectionController.enterButtonIsClicked = false;
                    gameState = GameState.TrackSelection;
                    regionIndex = regionSelectionController.GetTrackPanelIndex();
                    // trackSelectionController.panel = trackSelectionPanel[regionIndex]; // set panel
                    StartCoroutine(Fade(regionSelectionPanel, new []{trackSelectionPanel[regionIndex] }));
                }
                break;

            case GameState.TrackSelection:
                if (!isFading && trackSelectionController.enterButtonIsClicked) {
                    trackSelectionController.enterButtonIsClicked = false;
                    if (regionSelectionController.selectedRegion == GameState.EnFrRegion) {
                        gameState = GameState.EnFrRegion;
                        StartCoroutine(Fade(trackSelectionPanel[regionIndex], new []{enFrScene, gameUI, coordinateMarks}));
                    }
                    else if (regionSelectionController.selectedRegion == GameState.JpKrRegion) {
                        gameState = GameState.JpKrRegion;
                        StartCoroutine(Fade(trackSelectionPanel[regionIndex], new []{jpKrScene, gameUI, coordinateMarks}));
                    }
                    StartCoroutine(RegionStartPlay());
                }
                break;

            case GameState.EnFrRegion:
                break;

            case GameState.JpKrRegion:
                break;

            case GameState.GameEnded:
                break;

            default:
                Debug.LogError("Unknown game state: " + gameState);
                break;

        }
    }

    private IEnumerator Fade(GameObject previousScene, GameObject[] nextScenes) {

        isFading = true;

        // 畫面淡出
        for (var alpha = 0f; alpha <= 1.01f; alpha += 0.025f) {
            var newColor = blackScreen.color;
            newColor.a = alpha;
            blackScreen.color = newColor;
            yield return new WaitForSeconds(0.05f);
        }

        // 切換場景
        previousScene.SetActive(false);
        foreach (var nextScene in nextScenes)
            nextScene.SetActive(true);

        // 畫面淡入
        for (var alpha = 1f; alpha >= -0.01f; alpha -= 0.025f) {
            var newColor = blackScreen.color;
            newColor.a = alpha;
            blackScreen.color = newColor;
            yield return new WaitForSeconds(0.05f);
        }

        isFading = false;

    }

    public void ChangeState(GameState gs)
    {
        this.gameState = gs;
    }

    IEnumerator RegionStartPlay()
    {
        yield return new WaitForSeconds(2);
        gameManagement.StartPlay();
    }
}
