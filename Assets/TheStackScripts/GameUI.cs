using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : BaseUi
{
    TextMeshProUGUI scoreText;
    TextMeshProUGUI comnboText;
    TextMeshProUGUI maxComnboText;
    protected override UiState GetUIState()
    {
        return UiState.Game;
    }
    public override void Init(UiManager uiManager)
    {
        base.Init(uiManager);
        // 추가 초기화 코드

        scoreText = transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        comnboText = transform.Find("ComboText").GetComponent<TextMeshProUGUI>();
        maxComnboText = transform.Find("MaxComboText").GetComponent<TextMeshProUGUI>();
    }
    public void SetUI(int score, int combo, int maxCombo)
    {
        scoreText.text = score.ToString();
        comnboText.text = combo.ToString();
        maxComnboText.text = maxCombo.ToString();
    }
}
