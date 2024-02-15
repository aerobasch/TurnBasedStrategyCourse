using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;

    private void Awake()
    {
        if(TryGetComponent<MoveAction>(out MoveAction moveAction))
            {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
            moveAction.OnChangedFloorsStarted += MoveAction_OnChangedFloorsStarted;

            }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
         if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }
    }

    private void MoveAction_OnChangedFloorsStarted(object sender, MoveAction.OnChangedfloorsStartedEventArgs e)
    {
       if (e.targetGridPosition.floor > e.unitGridPosition.floor)
       {
        //target is above unit
        //jump
        animator.SetTrigger("JumpUp");
       }
       else
       {
        // target below unit
        //Drop down
        animator.SetTrigger("JumpDown");
       }
    }

    private void Start()
{
    EquipRifle();
}
    private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        Equipsword();
        animator.SetTrigger("SwordSlash");
        
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {

        animator.SetTrigger("Shoot");
        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab,shootPointTransform.position,Quaternion.identity);
        BulletProjectile bulletProjectile= bulletProjectileTransform.GetComponent<BulletProjectile>();
        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        float unitSholderHeight = 1.7f;
        targetUnitShootAtPosition.y  +=  unitSholderHeight;
        bulletProjectile.Setup(targetUnitShootAtPosition);

    }

    private void MoveAction_OnStopMoving(object sender, System.EventArgs e)
    {
        animator.SetBool("IsWalking", false);
    }

    private void MoveAction_OnStartMoving(object sender, System.EventArgs e)
    {
        animator.SetBool("IsWalking",true);
    }

    private void EquipRifle()
    {
        swordTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
    }
     private void Equipsword()
    {   
        swordTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
        
        
    }
}
