using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class HistoryManager : MonoBehaviour
{
    [System.Serializable]
    public class RoundResult
    {
        public string playerMove;
        public string enemyMove;
        public string outcome;
    }

    public List<RoundResult> history = new List<RoundResult>();

    [Header("UI Elements")]
    public TMP_Text row1Player, row1Enemy, row1Result;
    public TMP_Text row2Player, row2Enemy, row2Result;

    [Header("History Panel")]
    public GameObject historyPanel; 
    public Button historyButton; 
    public Button panelCloseButton;

    private void Start()
    {
        // Panel hidden by default, history button visible
        if (historyPanel != null)
            historyPanel.SetActive(false);

        if (historyButton != null)
            historyButton.gameObject.SetActive(true);

        // Hook up panel close button
        if (panelCloseButton != null)
            panelCloseButton.onClick.AddListener(HidePanelShowButton);
    }

    // Called by main history button
    public void ShowPanelHideButton()
    {
        if (historyPanel != null)
            historyPanel.SetActive(true);

        if (historyButton != null)
            historyButton.gameObject.SetActive(false);

        RefreshUI();
    }

    // Called by panel close button
    public void HidePanelShowButton()
    {
        if (historyPanel != null)
            historyPanel.SetActive(false);

        if (historyButton != null)
            historyButton.gameObject.SetActive(true);
    }

    // Add a new round to history
    public void AddResult(string player, string enemy, string result)
    {
        history.Insert(0, new RoundResult
        {
            playerMove = player,
            enemyMove = enemy,
            outcome = result
        });

        if (history.Count > 2)
            history.RemoveAt(history.Count - 1);

        // Refresh UI only if panel is active
        if (historyPanel != null && historyPanel.activeSelf)
            RefreshUI();
    }

    // Update the UI
    void RefreshUI()
    {
        if (history.Count == 0)
        {
            ClearUI();
            return;
        }

        if (history.Count >= 1)
        {
            row2Player.text = history[0].playerMove;
            row2Enemy.text  = history[0].enemyMove;
            row2Result.text = history[0].outcome;
        }

        if (history.Count == 2)
        {
            row1Player.text = history[1].playerMove;
            row1Enemy.text  = history[1].enemyMove;
            row1Result.text = history[1].outcome;
        }
        else if (history.Count < 2)
        {
            row1Player.text = "";
            row1Enemy.text  = "";
            row1Result.text = "";
        }
    }

    void ClearUI()
    {
        row1Player.text = row1Enemy.text = row1Result.text = "";
        row2Player.text = row2Enemy.text = row2Result.text = "";
    }
}
