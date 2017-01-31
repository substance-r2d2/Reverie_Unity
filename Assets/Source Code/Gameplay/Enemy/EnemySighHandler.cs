using UnityEngine;
using System.Collections;

public class EnemySighHandler : MonoBehaviour {


	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") {
            PlayerFSM player = other.GetComponent<PlayerFSM>();
            if(player != null)
            {
                player.ChangeState(player.deadState);
            }
		}
	}
}
