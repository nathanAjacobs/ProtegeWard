using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {


    private Vector3 targetPosition;
    private float bulletSpeed;

    private float bulletDamage;
    private float timer = 0f;
    private GameObject player;
    private PlayerController pc;
    private LayerMask playerMask;
    private Vector3 lastPos;

    // called once script is instanced on an object
    private void Awake()
    {
        // initialize variables
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        playerMask = LayerMask.GetMask("Player");
        lastPos = transform.position;
    }

    void Update ()
    {
        // after 3 seconds destroy the bullet
        timer += Time.deltaTime;
        if(timer >= 6f)
        {
            GameObject.Destroy(this.gameObject);
        }   
    }

    private void FixedUpdate()
    {
        // save current position
        Vector3 currentPos = transform.position;

        // calculate distance and direction from last position to current position
        float distance = Vector3.Distance(currentPos, lastPos);
        Vector3 dirToCurrentPos = (currentPos - lastPos).normalized;

        // raycast from last postion to current position
        RaycastHit hit;
        if (Physics.Raycast(lastPos, dirToCurrentPos, out hit, distance, playerMask))
        {
            // if the player was hit call playerHit()
            playerHit(hit.transform.gameObject.GetComponent<CapsuleCollider>());
        }
        else
        {
            // did not hit the player, so bullet moves to next position
            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, targetPosition, bulletSpeed * Time.fixedDeltaTime);
        }

        // set last position to this ticks current position
        lastPos = currentPos;
    }

    private void playerHit(CapsuleCollider collider)
    {
        pc.RemoveHealth(bulletDamage);
        //GameObject.Destroy(this.gameObject);
    }

    public void SetBulletSpeed(float f)
    {
        bulletSpeed = f;
    }

    public void SetTargetPosition(Vector3 v)
    {
        targetPosition = v;
    }

    public void SetBulletDamage(float f)
    {
        bulletDamage = f;
    }

}
