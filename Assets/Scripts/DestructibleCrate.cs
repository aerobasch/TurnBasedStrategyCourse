using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
public static event EventHandler OnAnyDestroyed;
private GridPosition gridPosition;

private void Start()
{
    gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
}

   public void Damage()

{

    Destroy(gameObject);
    OnAnyDestroyed?.Invoke(this,EventArgs.Empty);

}

public GridPosition  GetGridPosition()
{
    return gridPosition;
}

}
