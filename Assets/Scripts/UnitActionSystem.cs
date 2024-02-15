using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.CanvasScaler;

public class UnitActionSystem : MonoBehaviour
{
    
    public static UnitActionSystem Instance { get; private set; }


    [SerializeField]  private Unit selectedUnit;
    [SerializeField] private LayerMask unitsLayerMask;


    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    private BaseAction selectedAction;
    private bool isBusy;


    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("There is more than one Unit Action System " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        
    }


    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }


    private void Update()
    {
        

       

        
        if (isBusy)
        {
            return;
        }

        if(!TurnSystem.Instance.IsPlayerTurn()) 
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject()) 
        {
            return;
        }
     
            if (TryHandleUnitSelection()) 
        { 
            return; 
        }
                
               
        HandleSelectedAction();
    }
    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPositionOnlyHitVisible());
            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                return;
            }

            if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
            {
                return;
            }

           
            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            OnActionStarted?.Invoke(this, EventArgs.Empty);




        }
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);

    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame()) 
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());

            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitsLayerMask))
            {

                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if(unit == selectedUnit)

                    {
                        //Unit Already Selected
                        return false;
                    }

                    if (unit.IsEnemy())
                    {
                        // Clicked On an Enemy
                        return false;
                    }

                    SetSelectedUnit(unit);

                    return true;
                }
            }
            
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);

    }
    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    { 
        return selectedUnit;
    }
   public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    public bool IsBusy()
    {
        return isBusy;
    }
}


