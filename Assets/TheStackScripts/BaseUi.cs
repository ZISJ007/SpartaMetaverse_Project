using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public abstract class BaseUi : MonoBehaviour
{
    protected UiManager uiManager;

    public virtual void Init(UiManager uiManager)
    {
        this.uiManager = uiManager;
    }

    protected abstract UiState GetUIState();
    public void SetActive(UiState state)
    {
        gameObject.SetActive(GetUIState() == state);
    }
}