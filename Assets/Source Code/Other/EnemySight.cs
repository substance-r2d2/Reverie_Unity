using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour
{

    public GameObject bullet;

    public Transform nozzlePosition;



    void InstantiateBullet()
    {
        GameObject bulletObject = Instantiate(bullet, nozzlePosition.position, Quaternion.identity) as GameObject;
    }



    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            InvokeRepeating("InstantiateBullet", 2f, 3f);
        }
    }

    void OnCollisionExit2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            CancelInvoke("InstantiateBullet");
        }
    }
}
