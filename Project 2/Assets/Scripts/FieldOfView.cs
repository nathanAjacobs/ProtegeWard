using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();
    public List<Rigidbody> targetsRBs = new List<Rigidbody>();

    private void Update()
    {
        
    }

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        FindVisisbleTargets();
    }


    void FindVisisbleTargets()
    {
        visibleTargets.Clear();
        targetsRBs.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Rigidbody rb = targetsInViewRadius[i].GetComponent<Rigidbody>();
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if(!Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    targetsRBs.Add(rb);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

}
