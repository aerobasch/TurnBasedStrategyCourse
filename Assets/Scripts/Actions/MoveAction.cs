using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    public event EventHandler<OnChangedfloorsStartedEventArgs> OnChangedFloorsStarted;

    public class OnChangedfloorsStartedEventArgs : EventArgs
    {
        public GridPosition unitGridPosition;
        public GridPosition targetGridPosition;
    }

    [SerializeField] private int maxMoveDistance = 4;

    private List<Vector3> positionList;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float stoppingDistance;
    
    private int currentPositionIndex;
    private bool isChangingFloors;
    private float differentFloorsTeleportTimer;
    private float differentFloorsTeleportTimerMax = .5f;

  

    

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
            Vector3 targetPosition = positionList[currentPositionIndex];
            if(isChangingFloors)
            {
                //stop and teleport logic
                Vector3 targetSameFloorPosition = targetPosition;
                targetSameFloorPosition.y = transform.position.y;
                Vector3 rotateDirection = (targetSameFloorPosition - transform.position).normalized;

                float rotateSpeed = 10f;
                transform.forward = Vector3.Slerp(transform.forward, rotateDirection, Time.deltaTime * rotateSpeed);

                differentFloorsTeleportTimer -= Time.deltaTime;

                if(differentFloorsTeleportTimer < 0f )
                {
                    isChangingFloors = false;
                    transform.position = targetPosition;

                }
            }
            else
            {
                //regular move logic
            
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }
            

            if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
            {
                currentPositionIndex++;
                if(currentPositionIndex >= positionList.Count)
                {
                    OnStopMoving?.Invoke(this, EventArgs.Empty);
                    ActionComplete();
                } 
                else 
                {
                    targetPosition = positionList[currentPositionIndex];
                    GridPosition targetGridPosition = LevelGrid.Instance.GetGridPosition(targetPosition);
                    GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

                    if(targetGridPosition.floor != unitGridPosition.floor)
                    {
                        // Diffrent Floors
                        isChangingFloors = true;
                        differentFloorsTeleportTimer = differentFloorsTeleportTimerMax;
                        OnChangedFloorsStarted?.Invoke(this,new OnChangedfloorsStartedEventArgs{
                            unitGridPosition = unitGridPosition,
                            targetGridPosition = targetGridPosition
                            });
                    }

                }
                
        }
            
        
    }



    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList =  Pathfinding.Instance.FindPath(unit.GetGridPosition(),gridPosition,out int pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>();
        foreach(GridPosition pathGridposition in pathGridPositionList)
        {
                positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridposition));
        }
        
        OnStartMoving?.Invoke(this,EventArgs.Empty);
        ActionStart(onActionComplete);
    }

   

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++) 
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                for(int floor = -maxMoveDistance;floor <=maxMoveDistance;floor++)
                {
                     GridPosition offsetGridPosition = new GridPosition(x,z,floor);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;


               if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))

                {
                    continue;
                }

                
               if (unitGridPosition == testGridPosition)
                {
                    //Same Grid Position where the unit is at
                    continue;
                }

               if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    //GridPosition already occupied by unit
                    continue;
                }

                if(!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    //GridPosition not Walkable
                    continue;
                }

                if(!Pathfinding.Instance.HasPath(unitGridPosition ,testGridPosition))
                {
                    //GridPosition not Walkable
                    continue;
                }
                int pathfindingDistanceMultiplier = 10;
               if( Pathfinding.Instance.GetPathLength(unitGridPosition,testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
               {
                    //Path Length is too long
                    continue;
               }

                validGridPositionList.Add(testGridPosition);
                }   
            }
               
        }


        return validGridPositionList;
    }
    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
       int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction
        {

            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
}
