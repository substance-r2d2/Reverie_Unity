using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameCollider : MonoBehaviour {

    bool b_eventTrigerred = false;

    void OnTriggerEnter2D(Collider2D other)
    {   
        if(other.tag == "Player" && !b_eventTrigerred)
        {
            b_eventTrigerred = true;
            if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
                EventHandler.TriggerEvent(EEventID.EVENT_LOAD_LEVEL, (SceneManager.GetActiveScene().buildIndex + 1));
            else
                EventHandler.TriggerEvent(EEventID.EVENT_LOAD_LEVEL, SCENEID.MAIN_MENU);
        }
    }
}
