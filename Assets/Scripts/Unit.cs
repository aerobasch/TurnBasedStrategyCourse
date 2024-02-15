using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;


    [SerializeField] private bool isEnemy;

    private HealthSystem healthSystem;
   
    private BaseAction[] baseActionArray;
    [SerializeField] private int actionPoints = 2;
    [SerializeField] private int actionPointsMax = 2;

    
    private GridPosition gridPosition;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
      
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition,this);
        Destroy(gameObject);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
              
       

       GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition) 
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach(BaseAction baseAction in baseActionArray)
        {
            if(baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

        
   

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        return false;

    }


    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this,EventArgs.Empty);
    }


    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        if((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = actionPointsMax;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }

    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }
}

