using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public Vector2 moveLeftScreenCoords;
    public Vector2 moveRightScreenCoords;

    public static GameManager Instance;
    int LevelToLoad;

#if UNITY_ANDROID
       InputController m_ptrInputController;
#elif UNITY_WEBGL
    KeyboardInputController m_ptrInputController;
#endif
    LevelManager m_ptrLevelManager;

    public GameUIController m_ptrUIController { get; set; }

    Camera mainCamera;
    AsyncOperation loading;

    public static bool b_takeInput;

    void Awake()
    {
        b_takeInput = false;
        DontDestroyOnLoad(this.gameObject);
        Instance = this;
        Application.targetFrameRate = 60;
    }

	void Start ()
    { 
        EventHandler.AddListener(EEventID.EVENT_LOAD_LEVEL, OnEventLoadLevel);
        Invoke("DelayMainMenu", Time.deltaTime);
#if UNITY_ANDROID
        m_ptrInputController = new InputController();
#elif UNITY_WEBGL
        m_ptrInputController = new KeyboardInputController();
#endif
        m_ptrLevelManager = new LevelManager();
        m_ptrUIController = GameObject.FindObjectOfType<GameUIController>();

        mainCamera = Camera.main;
        SetUpCameraEdgeColliders();
    }

    void Update()
    {
        if(b_takeInput)
            m_ptrInputController.Update();
    }

    void DelayMainMenu()
    {
        SceneManager.LoadSceneAsync((int)SCENEID.MAIN_MENU);
        //Application.LoadLevel((int)SCENEID.MAIN_MENU);
    }

    void OnEventLoadLevel(System.Object data)
    {
        b_takeInput = false;
        SoundManager.SharedInstance.StopAllSounds();
        Camera cam = Camera.main;
        int level = (int)data;
        LevelToLoad = level;
        //System.GC.Collect();
        EventHandler.CleanUpTable();
        m_ptrLevelManager.RegisterUpdateSaveEvent();
        //Invoke("DelayLoad", 1f);
        StartCoroutine(DelayLoad());
    }

    IEnumerator DelayLoad()
    {
        yield return new WaitForSeconds(1f);
        //Debug.LogError(EventHandler.Count());
        loading = SceneManager.LoadSceneAsync(LevelToLoad);
        loading.allowSceneActivation = false;
        Debug.Log(loading.progress);
        while (loading.progress < 0.9f)
        {
            yield return null;
        }
        loading.allowSceneActivation = true;
        //Application.LoadLevel(LevelToLoad);
        //AsynchronousLoad();
    }

    IEnumerator AsynchronousLoad()
    {
        yield return new WaitForSeconds(1f);
        yield return null;

        AsyncOperation ao = SceneManager.LoadSceneAsync(LevelToLoad);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            // [0, 0.9] > [0, 1]
            float progress = Mathf.Clamp01(ao.progress / 0.9f);
            //Debug.log("Loading progress: " + (progress * 100) + "%");
            Debug.LogError("HERE");
            // Loading completed
            if (ao.progress == 0.9f)
            {
                //Debug.Log("Press a key to start");
                //if (Input.AnyKey())
                ao.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void OnLevelWasLoaded(int currentLevel)
    {
        PlayBGAudio(currentLevel);
        if (m_ptrLevelManager != null && currentLevel > (int)SCENEID.MAIN_MENU)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            m_ptrLevelManager.UpdateSavePoint(player.position);
        }
    }

    void PlayBGAudio(int level)
    {
        //SoundManager.SharedInstance.StopAllSounds();
        switch(level)
        {
            case 1:
                SoundManager.SharedInstance.PlayClip("MainMenuBG", 0.7f, true);
                break;

            case 2:
                SoundManager.SharedInstance.PlayClip("LevelBG_2", 0.7f, true);
                break;

            case 3:
                SoundManager.SharedInstance.PlayClip("LevelBG_2", 0.7f, true);
                break;

            case 4:
                SoundManager.SharedInstance.PlayClip("LevelBG_4", 0.7f, true);
                break;

            case 5:
                SoundManager.SharedInstance.PlayClip("LevelBG_3", 0.7f, true);
                break;

            case 6:
                SoundManager.SharedInstance.PlayClip("LevelBG_3", 0.7f, true);
                break;

            default:
                SoundManager.SharedInstance.PlayClip("MainMenuBG", 0.7f, true);
                break;

        }
    }

    public Vector2 GetSavePoint()
    {
        return m_ptrLevelManager.SavePos;
    }

    void SetUpCameraEdgeColliders()
    {
        Vector2 TopLeft = new Vector2(0, 1);
        Vector2 BottomLeft = new Vector2(0, 0);
        Vector2 BottomRight = new Vector2(1, 0);
        Vector2 TopRight = new Vector2(1, 1);

        EdgeCollider2D collider = mainCamera.GetComponent<EdgeCollider2D>();
        Vector2 offset = new Vector2(0, 10);
        Vector2[] temp = new Vector2[4];
        temp[0] = (Vector2)Camera.main.ViewportToWorldPoint(TopLeft) + offset;
        temp[1] = (Vector2)Camera.main.ViewportToWorldPoint(BottomLeft) - offset;
        temp[2] = (Vector2)Camera.main.ViewportToWorldPoint(BottomRight) - offset;
        temp[3] = (Vector2)Camera.main.ViewportToWorldPoint(TopRight) + offset;

        collider.points = temp;
    }

    void OnDestroy()
    {
        EventHandler.RemoveListener(EEventID.EVENT_LOAD_LEVEL, OnEventLoadLevel);
    }


}

public enum SCENEID
{
    LOADER = 0,
    MAIN_MENU,
    LEVEL_1_1,
    LEVEL_1_2,
    LEVEL_2_1,
    LEVEL_2_2,
    LEVEL_2_3,
}
