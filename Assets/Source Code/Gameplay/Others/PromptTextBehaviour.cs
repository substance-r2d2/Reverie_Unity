using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PromptTextBehaviour : MonoBehaviour {

    public string Message;
    public TutorialEventID tutorial;
    Text m_TextPrompt;
    public bool b_lockPlayer;

    void Start()
    {
        m_TextPrompt = GameObject.Find("TextPrompt").GetComponent<Text>() ;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_TextPrompt.text = Message;
            if (b_lockPlayer)
            {
                EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.LOCK);
                StartCoroutine(UnLockPlayer(1.4f));
            }

            //EventHandler.TriggerEvent(EEventID.EVENT_SHOW_PROMPT_MSG, Message);
            iTween.ScaleTo(m_TextPrompt.gameObject, Vector3.one, 1.5f);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //EventHandler.TriggerEvent(EEventID.EVENT_SHOW_PROMPT_MSG, Message);
            iTween.ScaleTo(m_TextPrompt.gameObject, Vector3.zero, 1.5f);
        }
    }

    public void ShowText()
    {
        this.GetComponent<BoxCollider2D>().enabled = false;
        m_TextPrompt = GameObject.Find("TextPrompt").GetComponent<Text>();
        m_TextPrompt.text = Message;
        iTween.ScaleTo(m_TextPrompt.gameObject, new Vector3(1.25f, 1.25f, 1.25f), 1.5f);
        Invoke("ReturnBack",1.55f);
    }

    void RegisterTutorialEvent(TutorialEventID id)
    {
        switch(id)
        {
            case TutorialEventID.DRAW_CIRCLE:
                //EventHandler.AddListener(EEventID.EVENT_GESTURE_CIRCLE, OnEventGestureCircle);
                break;
        }
    }

    IEnumerator UnLockPlayer(float time)
    {
        yield return new WaitForSeconds(time);
        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.IDLE);
    }

    void ReturnBack()
    {
        iTween.ScaleTo(m_TextPrompt.gameObject, Vector3.zero, 1.5f);
    }

}
