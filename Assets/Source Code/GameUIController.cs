using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameUIController : MonoBehaviour
{
    Transform gestureUITrans;
    public Text m_PromptText;
    int index;
    public static int maxCount;

    public Transform MainMenuRoot;
    public Transform GameMenuRoot;
    public Image SplashImage;

    public Transform ScreenControls;
    public Transform RightButton;
    public Transform LeftButton;
    public Transform JumpButton;

    public GameObject m_LoadingIndicator;
    public GameObject m_ContinueButton;
    public GameObject m_NewGameButton;
    public GameObject m_RestartGameButton;
    public GameObject m_ExitGameButton;

    Dictionary<Transform, Vector3> originalRotation;

    public static GameUIController Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        originalRotation = new Dictionary<Transform, Vector3>();
        originalRotation.Add(RightButton, RightButton.eulerAngles);
        originalRotation.Add(LeftButton, LeftButton.eulerAngles);
        originalRotation.Add(JumpButton, JumpButton.eulerAngles);

        //EventHandler.AddListener(EEventID.EVENT_UPDATE_GESTURE_UI, OnEventUpdateGestureUI);
        EventHandler.AddListener(EEventID.EVENT_LOAD_LEVEL, OnEventLoadLevel);
        gestureUITrans = GameMenuRoot.FindChild("GesturesUI");
        index = gestureUITrans.childCount;
        maxCount = gestureUITrans.childCount;
        m_PromptText = GameMenuRoot.FindChild("TextPrompt").GetComponent<Text>();
        gestureUITrans.gameObject.SetActive(false);
    }

    void OnEventLoadLevel(System.Object data)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeSplashImageColor(Color.white, true));
        ToggleScreenButtons(false);
    }

    public void OnStartButtonClick()
    {
        MainMenuRoot.gameObject.SetActive(false);
        SoundManager.SharedInstance.StopSound("MainMenuBG");
        SoundManager.SharedInstance.PlayClip("ButtonSelect");
        int levelToLoad = PlayerPrefs.GetInt("SAVED_PROGRESS", 2);
        EventHandler.TriggerEvent(EEventID.EVENT_LOAD_LEVEL, SCENEID.LEVEL_1_1);
    }

    void ResetGestureIcons()
    {
        index = gestureUITrans.childCount;
        maxCount = gestureUITrans.childCount;
        for (int i = 0; i < maxCount; i++)
        {
            Transform t = gestureUITrans.GetChild(i);
            iTween.ScaleTo(t.gameObject, Vector3.one, 1f);
        }
    }

    void OnEventUpdateGestureUI(System.Object data)
    {
        int i = (int)data;
        if (i == 1)
        {
            SoundManager.SharedInstance.PlayClip("GestureCreate");
            --maxCount;
            --index;
            Transform t = gestureUITrans.GetChild(index);
            iTween.ScaleTo(t.gameObject, Vector3.zero, 1f);
        }
        else
        {
            SoundManager.SharedInstance.PlayClip("GestureDestroy");
            ++maxCount;
            Transform t = gestureUITrans.GetChild(index);
            iTween.ScaleTo(t.gameObject, Vector3.one, 1f);
            ++index;
        }
    }

    public void OnRestartButtonClick()
    {
        SoundManager.SharedInstance.PlayClip("ButtonSelect");
        Time.timeScale = 1f;
        Transform PausePanel = GameMenuRoot.FindChild("OptionsPanel");
        iTween.ScaleTo(PausePanel.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.2f, "ignoretimescale", true));
        EventHandler.TriggerEvent(EEventID.EVENT_LOAD_LEVEL, SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuitBtnClick()
    {
        SoundManager.SharedInstance.PlayClip("ButtonSelect");
        Time.timeScale = 1f;
        Transform PausePanel = GameMenuRoot.FindChild("OptionsPanel");
        iTween.ScaleTo(PausePanel.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.2f, "ignoretimescale", true));
        EventHandler.TriggerEvent(EEventID.EVENT_LOAD_LEVEL, SCENEID.MAIN_MENU);
    }

    public void OnPauseBtnClick()
    {
        SoundManager.SharedInstance.PlayClip("ButtonSelect");
        Transform PausePanel = GameMenuRoot.FindChild("OptionsPanel");
        if (Mathf.Approximately(1f, PausePanel.localScale.x) && Mathf.Approximately(1f, PausePanel.localScale.y))
        {
            ToggleScreenButtons(true);
            //SoundManager.SharedInstance.ResumeSound("LevelBG");
            iTween.ScaleTo(PausePanel.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.75f, "ignoretimescale", true));
            Time.timeScale = 1f;
        }
        else if (Mathf.Approximately(0f, PausePanel.localScale.x) && Mathf.Approximately(0f, PausePanel.localScale.y))
        {
            //SoundManager.SharedInstance.PauseSound("LevelBG");
            iTween.ScaleTo(PausePanel.gameObject, iTween.Hash("scale", Vector3.one, "time", 0.75f, "ignoretimescale", true));
            ToggleScreenButtons(false);
            Time.timeScale = 0f;
        }
    }

    public void OnEscapeButton(System.Object data)
    {
        OnPauseBtnClick();
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    void OnLevelWasLoaded(int level)
    {
        if((level > 2) && (PlayerPrefs.GetInt("SAVED_PROGRESS",2) < level))
            PlayerPrefs.SetInt("SAVED_PROGRESS", level);

        m_PromptText.text = "";
        EventHandler.AddListener(EEventID.EVENT_UPDATE_GESTURE_UI, OnEventUpdateGestureUI);
        EventHandler.AddListener(EEventID.EVENT_SHOW_PROMPT_MSG, OnEventShowText);
        if (level > (int)SCENEID.MAIN_MENU)
        {
#if UNITY_ANDROID
            ScreenControls.gameObject.SetActive(true);
#elif UNITY_WEBGL
            ScreenControls.gameObject.SetActive(false);
            EventHandler.AddListener(EEventID.EVENT_KEY_ESCAPE, OnEscapeButton);
#endif
            MainMenuRoot.gameObject.SetActive(false);
            GameMenuRoot.gameObject.SetActive(true);
            ResetGestureIcons();
        }
        else
        {
            ScreenControls.gameObject.SetActive(false);
            MainMenuRoot.gameObject.SetActive(true);
            int currentLevel = PlayerPrefs.GetInt("SAVED_PROGRESS", 2);
            if(currentLevel > 2)
            {
                m_RestartGameButton.SetActive(true);
                m_ContinueButton.SetActive(true);
                m_NewGameButton.SetActive(false);
                m_ExitGameButton.SetActive(false);
            }
            else
            {
                m_RestartGameButton.SetActive(false);
                m_ContinueButton.SetActive(false);
                m_ExitGameButton.SetActive(true);
                m_NewGameButton.SetActive(true);
            }
            GameMenuRoot.gameObject.SetActive(false);
            ToggleScreenButtons(true);
        }
        StopAllCoroutines();
        StartCoroutine(ChangeSplashImageColor(new Color(1f, 1f, 1f, 0f),false));
    }

    IEnumerator ChangeSplashImageColor(Color targetColor,bool b_loadingIndicator)
    {
        m_LoadingIndicator.SetActive(b_loadingIndicator);

        while (!Mathf.Approximately(targetColor.a, SplashImage.color.a))
        {
            SplashImage.color = Color.Lerp(SplashImage.color, targetColor, Time.deltaTime * 5f);
            yield return new WaitForEndOfFrame();
        }

        SplashImage.color = targetColor;
    }

    public void OnPlayerStateChange(PLAYERSTATE state,bool b_left)
    {
        ResetScreenControlRotation();
        if(state == PLAYERSTATE.IDLE)
        {
            LeftButton.gameObject.SetActive(true);
            RightButton.gameObject.SetActive(true);
            JumpButton.gameObject.SetActive(true);

            iTween.ScaleTo(LeftButton.gameObject, Vector3.one, 0.5f);
            iTween.ScaleTo(RightButton.gameObject, Vector3.one, 0.5f);
            iTween.ScaleTo(JumpButton.gameObject, Vector3.one, 0.5f);
        }
        if (state == PLAYERSTATE.MOVEMENT)
        {
            iTween.ScaleTo(JumpButton.gameObject, Vector3.zero, 0.5f);
            if(b_left)
            {
                RightButton.gameObject.SetActive(true);
                iTween.RotateTo(RightButton.gameObject, new Vector3(0f, 0f, 180f), 0.5f);
            }
            else
            {
                LeftButton.gameObject.SetActive(true);
                iTween.RotateTo(LeftButton.gameObject, new Vector3(0f, 0f, 180f), 0.5f);
            }
        }
        if(state == PLAYERSTATE.GESTURE_DRAW)
        {
            //iTween.ScaleTo(ScreenControls.gameObject, Vector3.zero, 0.5f);
            iTween.ScaleTo(LeftButton.gameObject, Vector3.zero, 0.5f);
            iTween.ScaleTo(RightButton.gameObject, Vector3.zero, 0.5f);
            iTween.ScaleTo(JumpButton.gameObject, Vector3.zero, 0.5f);
        }
        if(state == PLAYERSTATE.DEAD)
        {
            iTween.ScaleTo(LeftButton.gameObject, Vector3.zero, 0.5f);
            iTween.ScaleTo(RightButton.gameObject, Vector3.zero, 0.5f);
            iTween.ScaleTo(JumpButton.gameObject, Vector3.zero, 0.5f);
            //iTween.ScaleTo(ScreenControls.gameObject, Vector3.zero, 0.5f);
        }
        if(state == PLAYERSTATE.JUMP)
        {
            iTween.ScaleTo(JumpButton.gameObject, Vector3.zero, 0.5f);
        }
    }

    void ResetScreenControlRotation()
    {
        iTween.Stop(ScreenControls.gameObject);
        iTween.Stop(LeftButton.gameObject);
        iTween.Stop(RightButton.gameObject);
        iTween.Stop(JumpButton.gameObject);

        iTween.RotateTo(LeftButton.gameObject, originalRotation[LeftButton], 0.5f);
        //LeftButton.eulerAngles = originalRotation[LeftButton];
        iTween.RotateTo(RightButton.gameObject, originalRotation[RightButton], 0.5f);
        //RightButton.eulerAngles = originalRotation[RightButton];
        iTween.RotateTo(JumpButton.gameObject, originalRotation[JumpButton], 0.5f);

        //iTween.ScaleTo(JumpButton.gameObject, Vector3.one, 0.5f);
    }

    void ToggleScreenButtons(bool b_visible)
    {
        if (b_visible)
        {
            ResetScreenControlRotation();
            iTween.ScaleTo(LeftButton.gameObject, Vector3.one, 0.5f);
            iTween.ScaleTo(RightButton.gameObject, Vector3.one, 0.5f);
        }
        else
        {
            iTween.ScaleTo(LeftButton.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "ignoretimescale", true));
            iTween.ScaleTo(RightButton.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "ignoretimescale", true));
            iTween.ScaleTo(JumpButton.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "ignoretimescale", true));
            //iTween.ScaleTo(LeftButton.gameObject, Vector3.zero, 0.5f);
            //iTween.ScaleTo(RightButton.gameObject, Vector3.zero, 0.5f);
            //iTween.ScaleTo(JumpButton.gameObject, Vector3.zero, 0.5f);
        }
    }

    public void OnJumpButtonClick()
    {
        EventHandler.TriggerEvent(EEventID.EVENT_JUMP, null);
    }

    public void ToggleJumpButton(bool active)
    {
        JumpButton.gameObject.SetActive(active);
    }

    public void OnRestartGameClick()
    {
        PlayerPrefs.SetInt("SAVED_PROGRESS", 2);
        int levelToLoad = PlayerPrefs.GetInt("SAVED_PROGRESS", 2);
        EventHandler.TriggerEvent(EEventID.EVENT_LOAD_LEVEL, levelToLoad);
    }

    void OnEventShowText(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        bool show = (bool)table["show"];
        string text = (string)table["message"];

        if(show)
        {

        }
        else
        {

        }
    }

    void OnDestroy()
    {
        EventHandler.RemoveListener(EEventID.EVENT_UPDATE_GESTURE_UI, OnEventUpdateGestureUI);
        EventHandler.RemoveListener(EEventID.EVENT_LOAD_LEVEL, OnEventLoadLevel);
    }

}
