using UnityEngine;
using System.Collections;

public class TriangleBlastBehaviour : MonoBehaviour
{
    public float timer;
    public float blastRadius;

    SpriteRenderer m_ptrRenderer;
    GameObject m_objBlastFX;

    void Start()
    {
        m_ptrRenderer = GetComponent<SpriteRenderer>();
        m_objBlastFX = Resources.Load<GameObject>("Prefabs/FX/D_FX_Explosion_Red");
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        var startTime = Time.time;
        while((Time.time - startTime) < timer)
        {
            m_ptrRenderer.color = Color.Lerp(m_ptrRenderer.color, Color.red, timer * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        GameObject obj = GameObject.Instantiate(m_objBlastFX, transform.position, Quaternion.identity) as GameObject;
        CheckObjectsInRange();
        Destroy(obj, 1f);
        EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJET_DESTROY, this.gameObject);
        Destroy(this.gameObject);
    }

    void CheckObjectsInRange()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, blastRadius);
        foreach(var collider in colliders)
        {
            if (collider.tag == "Enemy")
                EventHandler.TriggerEvent(EEventID.EVENT_DESTROY_ENEMY, collider.gameObject);
            else if ((collider.tag == "GesturePrefab") && (collider.gameObject != this.gameObject))
                EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJET_DESTROY, collider.gameObject);
            else if (collider.tag == "Player")
            {
                PlayerFSM player = collider.GetComponent<PlayerFSM>();
                player.ChangeState(player.deadState);
            }
        }
    }
}
