using UnityEngine;
using System.Collections;

public class SquareJumpBehaviour : MonoBehaviour
{

    public float jumpVelocity;
    public float ScaleChange;

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(other.contacts[0].normal.y < -0.9f)
            {
                iTween.PunchScale(this.gameObject, new Vector3(-ScaleChange, -ScaleChange), 0.5f);
                other.rigidbody.velocity = new Vector2(other.rigidbody.velocity.x, jumpVelocity);
            }
        }
    }

}
