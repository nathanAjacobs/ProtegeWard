using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPunchController : MonoBehaviour {

    private EnemyController ec;

    private void Start()
    {
        ec = transform.root.GetComponent<EnemyController>();
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ec.SetInTrigger(true);
            ec.SetCurrentTartgetHit(collider.transform.root.gameObject);
            //ec.SetPunchedTarget(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ec.SetInTrigger(false);
        ec.SetCurrentTartgetHit(null);
    }
}
