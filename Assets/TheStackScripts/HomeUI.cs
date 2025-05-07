using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : BaseUi
{
    Button startButton;
    Button exitButton;


    protected override UiState GetUIState()
    {
        return UiState.Home;
    }

    public override void Init(UiManager uiManager)
    {
        base.Init(uiManager);
        // 추가 초기화 코드

        startButton = transform.Find("StartButton").GetComponent<Button>();
        exitButton = transform.Find("ExitButton").GetComponent<Button>();

        startButton.onClick.AddListener(OnClickStartButton);
        exitButton.onClick.AddListener(OnClickExitButton);
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
