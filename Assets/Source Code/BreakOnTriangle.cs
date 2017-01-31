using UnityEngine;
using System.Collections;

public class BreakOnTriangle : MonoBehaviour {


    void OnCollisionEnter2D(Collision2D other)
    {
        
        if(other.rigidbody != null && other.gameObject.tag == "GesturePrefab")
        {
            //Debug.Log(other.rigidbody.velocity.y);
            if(other.rigidbody.mass > 2.5f)
            {
                iTween.ScaleTo(this.gameObject, Vector3.zero,0.5f);
            }
        }
    }

}
