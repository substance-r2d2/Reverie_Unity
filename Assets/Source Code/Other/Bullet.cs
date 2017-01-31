using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float movementSpeed = 30f;


    // Update is called once per frame
    void Update()
    {
        transform.Translate(-movementSpeed * Time.deltaTime, 0, 0);
    }


}

