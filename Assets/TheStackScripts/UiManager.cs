using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public enum UiState
{
    Home,
    Game,
    Score,
}
public class UiManager : MonoBehaviour
{
    static UiManager instance;
    public static UiManager Instance
    {
        get { return instance; }
    }

    UiState currentState = UiState.Home;
    HomeUI homeUI = null;
    GameUI gameUI = null;
    ScoreUI scoreUI = null;

    TheStack theStack = null;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        theStack = FindObjectOfType<TheStack>();

        homeUI = GetComponentInChildren<HomeUI>(true); // 매개변수 전달 가능 역할은 꺼져있는 오브젝트도 포함
        homeUI?.Init(this);// homeUI가 null이 아닐때만 Init을 호출

        gameUI = GetComponentInChildren<GameUI>(true);
        gameUI?.Init(this);

        scoreUI = GetComponentInChildren<ScoreUI>(true);
        scoreUI?.Init(this);

        ChangeState(UiState.Home);
    }
    public void ChangeState(UiState state)
    {
        currentState = state;
        homeUI?.SetActive(currentState);
        gameUI?.SetActive(currentState);
        scoreUI?.SetActive(currentState);
    }
    public void OnClickStart()
    {
        theStack.Restart();
        ChangeState(UiState.Game);
    }
    public void OnClickExit()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void UpdateScore()
    {
        gameUI.SetUI(theStack.Score, theStack.Combo, theStack.MaxCombo);
    }

    public void SetScoreUI()
    {
        scoreUI.SetUI(theStack.Score, theStack.MaxCombo, theStack.BestScore, theStack.BestCombo);
        ChangeState(UiState.Score);
    }
}
