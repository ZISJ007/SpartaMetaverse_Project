using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InteractionTrigger : MonoBehaviour
{
    public GameObject interactionUI;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionUI.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionUI.SetActive(false);
        }
    }
    public void ClickToStart()
    {
        SceneManager.LoadScene("TheStack");
    }
}
