using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private Button endTurnButton;

    [SerializeField] private GameObject enemyTurnVisualGameObject;




    private void Start()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });
        UpdateTurnText();
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisability();
    }
    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisability();

    }

    public void UpdateTurnText()
    {
        turnNumberText.text = "TURN " + TurnSystem.Instance.GetTurnNumber();



    }

    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButtonVisability()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
