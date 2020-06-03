using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum GameState {
    Entry, RegionSelection, TrackSelection, GamePlaying, GameEnded
}

public class SceneManagement : MonoBehaviour {

    private GameState gameState;
    [SerializeField] private Image blackScreen;
    private bool isFading;

    [SerializeField] private GameObject entryPanel;
    private EntryController entryController;
    [SerializeField] private GameObject regionSelectionPanel;
    private RegionSelectionController regionSelectionController;
    [SerializeField] private GameObject trackSelectionPanel;
    private TrackSelectionController trackSelectionController;

    [SerializeField] private GameObject enFrScene;
    [SerializeField] private GameObject jpKrScene;

    private void Awake() {

        gameState = GameState.Entry;
        var resetColor = blackScreen.color;
        resetColor.a = 0;
        blackScreen.color = resetColor;
        isFading = false;

        entryController = entryPanel.GetComponent<EntryController>();
        regionSelectionController = regionSelectionPanel.GetComponent<RegionSelectionController>();
        trackSelectionController = trackSelectionPanel.GetComponent<TrackSelectionController>();

        entryPanel.SetActive(true);
        regionSelectionPanel.SetActive(false);
        trackSelectionPanel.SetActive(false);
        enFrScene.SetActive(false);
        jpKrScene.SetActive(false);

    }

    private void LateUpdate() {
        switch (gameState) {

            case GameState.Entry:
                if (!isFading && entryController.EnterButtonIsClicked) {
                    gameState = GameState.RegionSelection;
                    StartCoroutine(Fade(entryPanel, regionSelectionPanel));
                }
                break;

            case GameState.RegionSelection:
                if (!isFading && regionSelectionController.EnterButtonIsClicked) {
                    gameState = GameState.TrackSelection;
                    StartCoroutine(Fade(regionSelectionPanel, trackSelectionPanel));
                }
                break;

            case GameState.TrackSelection:
                if (!isFading && trackSelectionController.EnterButtonIsClicked) {
                    gameState = GameState.GamePlaying;
                    StartCoroutine(Fade(trackSelectionPanel, enFrScene));
                }
                break;

            case GameState.GamePlaying:
                break;

            case GameState.GameEnded:
                break;

            default:
                Debug.LogError("Unknown game state: " + gameState);
                break;

        }
    }

    private IEnumerator Fade(GameObject previousScene, GameObject nextScene) {

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

}
