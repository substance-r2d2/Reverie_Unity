
#define VFADebug

using UnityEngine;
using System.Collections;

public class SpriteAnim : MonoBehaviour 
{
	public Sprite[] frames;    // Set this from the inspector
	float frameTime;
	public bool oneTime = false;

	private float lastFrameTime;
	public float totalTime;
	float startTime;

	int currFrame = 0;
	SpriteRenderer spriteRen = null;
	public int sortingOrder;
	public int frameRate;

	// Use this for initialization
	void Start ()
	{
		lastFrameTime = Time.time;
		spriteRen = this.gameObject.GetComponent<SpriteRenderer>();
		startTime = Time.time;
		spriteRen.sortingOrder = sortingOrder;
		frameTime = 1f/frameRate;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ((Time.time - lastFrameTime) >= frameTime)
		{
//			Debug.Log("ChangeFrame");
			currFrame = (currFrame + 1) % frames.Length;
			//renderer.material.mainTexture = frames[currFrame];
			spriteRen.sprite = frames[currFrame];
			lastFrameTime = Time.time;

			if(oneTime == true)
			{
				if(currFrame == frames.Length-1)
				{
					Destroy(this.gameObject);
				}
			}
			else
			{
				if((Time.time - startTime) > totalTime)
				{
					Destroy(this.gameObject);
				}
			}
		}
	}

}//End of SpriteAnim 

#region IMPLEMENTATION DETAILS AND NOTES
/*
 * var frames: Texture2D[];    // Set this from the inspector
var frameTime: float;
 
private var lastFrameTime: float;
 
function Start() {
    lastFrameTime = Time.time;
}
 
 
var currFrame: integer;
 
function Update() {
    if ((Time.time - lastFrameTime) >= frameTime) {
        currFrame = (currFrame + 1) % frames.Length;
        renderer.material.mainTexture = frames[currFrame];
    }
}
*
*/
#endregion

