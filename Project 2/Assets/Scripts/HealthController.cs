using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour {

    private GameObject player;

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            player = collider.gameObject;
            PlayerController pc = player.GetComponent<PlayerController>();

            if (pc == null)
                return;

            if (pc.GetPlayerHealth() < 100)
            {
                Object.Destroy(this.gameObject);
            }
            pc.AddHealth(50);

        }
    }
}
