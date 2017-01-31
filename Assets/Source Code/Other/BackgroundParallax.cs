using UnityEngine;
using System.Collections;

public class BackgroundParallax : MonoBehaviour
{
		public Transform[] backgrounds;				// Array of all the backgrounds to be parallaxed.
		public GameObject foreground;
		public GameObject foregroundCamera;
		float oldOrthoSize = 4;
		private Transform cam;						// Shorter reference to the main camera's transform.
		private Vector3 previousCamPos;				// The postion of the camera in the previous frame.
		public GameObject target;
		public float bg1 = 1,bg2 = 2,bg3 = 3, bg4 = 4, fg = 5;
		float offsetx, offsety;

		void Awake ()
		{
				// Setting up the reference shortcut.
				cam = Camera.main.transform;
		}

		void Start ()
		{
				// The 'previous frame' had the current frame's camera position.
				previousCamPos = cam.position;
			if(target == null)
			{
				backgrounds [0].GetComponent<Renderer>().sortingLayerName = "Foreground";
			}

		}

		void ParallaxMovement ()
		{
		offsetx =-( 0.001f * (previousCamPos.x - cam.position.x)) ;
		if(cam.position.y >-55)
			offsety =-( 0.001f * (previousCamPos.y - cam.position.y)) ;
		else 
			offsety =0;
		if(target !=null)
		{
				for (int i = 0; i < backgrounds.Length; i++) {
						Vector2 texOffset = backgrounds [i].GetComponent<Renderer>().material.mainTextureOffset;
						float tempOffsetx = offsetx;
						float tempOffsety = offsety;
				if(i == 0)
					tempOffsetx *= bg1*0.5f;
				else if(i == 1)
					tempOffsetx *= bg2;
				else if(i ==2)
				{
					tempOffsetx *= bg3;
					tempOffsety *= 2;
				}
				else if(i == 3)
				{
					tempOffsetx *= bg3;
					tempOffsety *= 2;
				}

//						if (target.GetComponent<Player> ().currentDirection == Direction.Left) {
//							texOffset.x -= tempOffset;
//						} else if (target.GetComponent<Player> ().currentDirection == Direction.Right) {
							texOffset.x += tempOffsetx;
							texOffset.y += tempOffsety;
//						}	
							
						backgrounds [i].GetComponent<Renderer>().material.mainTextureOffset = texOffset;
				}
		}
		else
		{
			Vector2 texOffset = backgrounds [0].GetComponent<Renderer>().material.mainTextureOffset;
			float tempOffset = offsetx;
			tempOffset *=  (fg * 6.0f) ;
			//						if (target.GetComponent<Player> ().currentDirection == Direction.Left) {
			//							texOffset.x -= tempOffset;
			//						} else if (target.GetComponent<Player> ().currentDirection == Direction.Right) {

	

			texOffset.x += tempOffset;
			//						}
			
			backgrounds [0].GetComponent<Renderer>().material.mainTextureOffset = texOffset;
		}
		previousCamPos = cam.position;
		}

		void Update ()
		{
				ParallaxMovement ();
		}

	void LateUpdate()
	{

		if((foreground!=null) && (oldOrthoSize != foregroundCamera.GetComponent<Camera>().orthographicSize))
		{
			if(foregroundCamera.GetComponent<Camera>().orthographicSize != 4.0f)
			{
				foreground.GetComponent<Renderer>().enabled = false; 
			}
			else
			{
				foreground.GetComponent<Renderer>().enabled = true; 
			}
			oldOrthoSize = foregroundCamera.GetComponent<Camera>().orthographicSize;
		}

	}

}
