using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;


    public void Setup(Transform origionalRootBone)
    {
        MatchAllChildTransforms(origionalRootBone,ragdollRootBone);
        Vector3 randomDir = new Vector3(Random.Range(-1f,+1f),0,Random.Range(-1f,+1f));
        ApplyExplosionToRagdol(ragdollRootBone, 300f, transform.position + randomDir, 10f);
    }

    private void MatchAllChildTransforms(Transform root, Transform Clone)
    {
        foreach (Transform child in root) 
        {
           Transform cloneChild =  Clone.Find(child.name);
            if(cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;
                MatchAllChildTransforms(child,cloneChild);
            }
        }
    }

    private void ApplyExplosionToRagdol(Transform root,float explosionForce,Vector3 explosionPosition, float explosionRange )
    {
        foreach(Transform child in root) 
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce,explosionPosition,explosionRange);
                ApplyExplosionToRagdol(child,explosionForce, explosionPosition, explosionRange);
            }
        
        }
    }

    }

