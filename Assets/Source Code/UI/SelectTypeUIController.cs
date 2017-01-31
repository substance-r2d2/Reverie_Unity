using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectTypeUIController : MonoBehaviour
{
    Animator m_anim;

    void Start()
    {
        m_anim = GetComponent<Animator>();
    }

    public void OnSelectTypeButton()
    {
        m_anim.SetBool("Open", !m_anim.GetBool("Open"));
    }

    public void OnSelectSubType(PrefabTypeID prefabType)
    {
        Hashtable table = new Hashtable();
        table.Add("id",prefabType.id);
        table.Add("prefab", prefabType.prefabName);
        EventHandler.TriggerEvent(EEventID.EVENT_SET_GESTURE_PREFAB, table);
        OnSelectTypeButton();
    }

}
