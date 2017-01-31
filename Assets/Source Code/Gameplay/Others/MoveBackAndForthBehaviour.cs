using UnityEngine;
using System.Collections;

public class MoveBackAndForthBehaviour : MonoBehaviour {

    public Vector3[] PathPoints;
    public float Speed = 5f;

    Rigidbody2D m_ptrRigid;

    Vector3 Target;

    void Start()
    {
        m_ptrRigid = GetComponent<Rigidbody2D>();
        Target = PathPoints[0];
    }

    void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, PathPoints[0]) < 0.1f)
            Target = PathPoints[1];

        if (Vector2.Distance(transform.position, PathPoints[1]) < 0.1f)
            Target = PathPoints[0];

        Vector2 pos = Vector2.MoveTowards(transform.position, Target, Speed * Time.fixedDeltaTime);
        transform.position = pos;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.transform.parent = this.transform;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.transform.parent = null;
        }
    }

}
