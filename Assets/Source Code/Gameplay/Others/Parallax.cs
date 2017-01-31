using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

    public Renderer[] ParallaxTextures;
    public float[] ParallaxLayerScrollSpeed;
    public float OffsetModifier;

    Camera m_MainCamera;
    Vector3 m_vPreviousCameraPostion;
    float[] addValue;
	// Use this for initialization
	void Start () {
        m_MainCamera = Camera.main;
        m_vPreviousCameraPostion = m_MainCamera.transform.position;
        addValue = new float[ParallaxTextures.Length];
	}
	
	// Update is called once per frame
	void LateUpdate () {
        //Debug.LogError(m_MainCamera.transform.position.x - m_vPreviousCameraPostion.x);
        if((m_MainCamera.transform.position.x - m_vPreviousCameraPostion.x) >= 0.02f)
        {
            for(int i=0;i<ParallaxTextures.Length;++i)
            {
                addValue[i] += (ParallaxLayerScrollSpeed[i]/OffsetModifier);
                ParallaxTextures[i].material.mainTextureOffset = new Vector2(addValue[i], 0f);
            }
        }
        else if ((m_MainCamera.transform.position.x - m_vPreviousCameraPostion.x) < -0.02f)
        {
            //Debug.LogError();
            for (int i = 0; i < ParallaxTextures.Length; ++i)
            {
                addValue[i] -= (ParallaxLayerScrollSpeed[i]/OffsetModifier);
                ParallaxTextures[i].material.mainTextureOffset = new Vector2(addValue[i], 0f);
            }
        }
        m_vPreviousCameraPostion = m_MainCamera.transform.position;
	}
}
