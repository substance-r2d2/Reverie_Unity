using UnityEngine;
using System.Collections;

public class PointLightIntensityIncrease : MonoBehaviour {

    float TargetValue = 8f;
    float StartIntensity = 0f;

    Light m_light;
    bool b_looped;
	// Use this for initialization
	void Start ()
    {
        b_looped = false;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1f);
        m_light = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(b_looped)
        {
            if (m_light.intensity < 1f)
            {
                Destroy(this.gameObject);
                EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJET_DESTROY, null);
            }
        }
        m_light.intensity = Mathf.MoveTowards(m_light.intensity, TargetValue, 2f * Time.deltaTime);
        if (Mathf.Approximately(m_light.intensity, TargetValue))
        {
            b_looped = true;
            TargetValue = 0f;
        }
	}
}
