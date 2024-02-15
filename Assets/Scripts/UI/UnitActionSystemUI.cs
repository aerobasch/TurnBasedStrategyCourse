using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }


    private void Start()
    {
        
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, System.EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }
    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();

    }

    private void CreateUnitActionButtons()
    {
        foreach(Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }
        actionButtonUIList.Clear();

       Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
      foreach(BaseAction baseAction in selectedUnit.GetBaseActionArray())
        {
          Transform actionButtonTransform =  Instantiate(actionButtonPrefab, actionButtonContainerTransform);
          ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
          actionButtonUI.SetBaseAction(baseAction);
          actionButtonUIList.Add(actionButtonUI);
        }



    }
    private void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {
       Unit selectedUint =  UnitActionSystem.Instance.GetSelectedUnit();


       actionPointsText.text = "Action Points: " + selectedUint.GetActionPoints();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }
}
