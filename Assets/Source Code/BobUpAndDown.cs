using UnityEngine;
using System.Collections;

public class BobUpAndDown : MonoBehaviour
{
    public float speed;
    public float units;

    Vector3 maxPos;
    Vector3 minPos;
    Vector3 target;

    float delay;
    float startTime;

    void Start()
    {
        maxPos = new Vector3(transform.position.x, transform.position.y + units, transform.position.z);
        minPos = new Vector3(transform.position.x, transform.position.y - units, transform.position.z);
        target = minPos;
        startTime = Time.time;
        delay = 0f;//Random.value;
    }

	// Update is called once per frame
	void Update ()
    {
        if ((Time.time - startTime) > delay)
        {
            if (Vector3.Distance(transform.position, target) > Mathf.Epsilon)
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            else
                target = (target == minPos) ? maxPos : minPos;
        }
    }
}
