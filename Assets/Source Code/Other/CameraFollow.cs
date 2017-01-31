using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	private float xMargin = 0.0f;		// Distance in the x axis the player can move before the camera follows.
	private float yMargin = 0.0f;		// Distance in the y axis the player can move before the camera follows.
	public float xSmooth = 3.5f;		// How smoothly the camera catches up with it's target movement in the x axis.
	public float ySmooth = 3.5f;        // How smoothly the camera catches up with it's target movement in the y axis.
    public Vector3 Offset;

    Vector2 maxXAndY;		// The maximum x and y coordinates the camera can have.
	Vector2 minXAndY;		// The minimum x and y coordinates the camera can have.

    public CameraFollowData[] cameraData;

	 public Transform player;		// Reference to the player's transform.

    EdgeCollider2D m_ptrEdgeCollider;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

	void Start ()
	{
        m_ptrEdgeCollider = GetComponent<EdgeCollider2D>();
     //   EventHandler.AddListener(EEventID.EVENT_LOAD_LEVEL, OnEventLoadLevel);
	}

    void OnLevelWasLoaded(int currentLevel)
    {
        m_ptrEdgeCollider.enabled = false;
        int level = currentLevel;
        for (int i=0;i<cameraData.Length;++i)
        {
            if((int)cameraData[i].id == level)
            {
                maxXAndY = cameraData[i].maxXAndY;
                minXAndY = cameraData[i].minXAndY;
                break;
            }
        }
    }

	bool CheckXMargin()
	{
		// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
		return Mathf.Abs(transform.position.x - player.position.x) > xMargin;
	}


	bool CheckYMargin()
	{
		// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
		return Mathf.Abs(transform.position.y - player.position.y) > yMargin;
	}

    void SetPlayerReference()
    {
        PlayerFSM playerFSM = GameObject.FindObjectOfType<PlayerFSM>();
        if (playerFSM != null)
            player = playerFSM.transform;
    }

    void FixedUpdate ()
	{
        if (player != null)
        {
            ToggleEdgeColliders();
            AdjustOffeset();
            TrackPlayer();
        }
        else
            SetPlayerReference();
	}
	
    void AdjustOffeset()
    {
        if(player.localScale.x < 0)
            Offset = new Vector3(-1f * Mathf.Abs(Offset.x), Offset.y, Offset.z);
        else
            Offset = new Vector3(Mathf.Abs(Offset.x), Offset.y, Offset.z);
    }

    void ToggleEdgeColliders()
    {
        if (Mathf.Abs(transform.position.x - minXAndY.x) <= Mathf.Epsilon)
            m_ptrEdgeCollider.enabled = true;
        else if (Mathf.Abs(transform.position.x - maxXAndY.x) <= Mathf.Epsilon)
            m_ptrEdgeCollider.enabled = true;
        else
            m_ptrEdgeCollider.enabled = false;
    }
	
	void TrackPlayer ()
	{
		// By default the target x and y coordinates of the camera are it's current x and y coordinates.
		float targetX = transform.position.x;
		float targetY = transform.position.y;

		// If the player has moved beyond the x margin...
		if(CheckXMargin())
			// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
			targetX = Mathf.Lerp(transform.position.x, player.position.x + Offset.x, xSmooth * Time.deltaTime);

		// If the player has moved beyond the y margin...
		if(CheckYMargin())
			// ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
			targetY = Mathf.Lerp(transform.position.y, player.position.y + Offset.y, ySmooth * Time.deltaTime);

		// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
		targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
		targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

		// Set the camera's position to the target position with the same z component.
		transform.position = new Vector3(targetX, targetY, transform.position.z);
	}

    void OnDestroy()
    {

    }
}

[System.Serializable]
public class CameraFollowData
{
    public SCENEID id;
    public Vector2 maxXAndY;
    public Vector2 minXAndY;
}
