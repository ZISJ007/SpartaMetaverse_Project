using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : BaseUi
{
    TextMeshProUGUI scoreText;
    TextMeshProUGUI comboText;
    TextMeshProUGUI bestScoreText;
    TextMeshProUGUI bestComboText;

    Button startButton;
    Button exitButton;

    public override void Init(UiManager uiManager)
    {
        base.Init(uiManager);
        scoreText = transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        comboText = transform.Find("ComboText").GetComponent<TextMeshProUGUI>();
        bestScoreText = transform.Find("BestScoreText").GetComponent<TextMeshProUGUI>();
        bestComboText = transform.Find("BestComboText").GetComponent<TextMeshProUGUI>();
        startButton = transform.Find("StartButton").GetComponent<Button>();
        exitButton = transform.Find("ExitButton").GetComponent<Button>();

        startButton.onClick.AddListener(OnClickStartButton);
        exitButton.onClick.AddListener(OnClickExitButton);
    }

    protected override UiState GetUIState()
    {
        return UiState.Score;
    }
    private void Start()
    {
        // PlayerPrefs에서 불러오기
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        int lastCombo = PlayerPrefs.GetInt("LastCombo", 0);
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        int bestCombo = PlayerPrefs.GetInt("BestCombo", 0);

        // UI 세팅
        SetUI(lastScore, lastCombo, bestScore, bestCombo);
    }
    public void SetUI(int score, int combo, int bestScore, int bestCombo)
    {
        scoreText.text = score.ToString();
        comboText.text = combo.ToString();
        bestScoreText.text = bestScore.ToString();
        bestComboText.text = bestCombo.ToString();
    }

    void OnClickStartButton()
    {
        uiManager.OnClickStart();
    }

    void OnClickExitButton()
    {
        uiManager.OnClickExit();
    }

}
