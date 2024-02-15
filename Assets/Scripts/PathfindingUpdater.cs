using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
   private void Start()
   {
    DestructibleCrate.OnAnyDestroyed += DestructableCrate_OnAnyDestroyed;
   }

    private void DestructableCrate_OnAnyDestroyed(object sender, EventArgs e)
    {
        DestructibleCrate destructibleCrate = sender as DestructibleCrate;
        Pathfinding.Instance.SetIsWalkableGridPosition(destructibleCrate.GetGridPosition(),true);
    }
}
