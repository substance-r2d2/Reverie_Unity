using UnityEngine;
using System.Collections;

public class LevelManager
{
    public Vector2 SavePos;
    
    public LevelManager()
    {
        
	}

    public void RegisterUpdateSaveEvent()
    {
        EventHandler.AddListener(EEventID.EVENT_UPDATE_SAVE_POSITION, OnEventUpdateSavePos);
    }

    void OnEventUpdateSavePos(System.Object data)
    {
        SavePos = (Vector2)data;
    }

    public void UpdateSavePoint(Vector2 pos)
    {
        SavePos = pos;
    }

    ~LevelManager()
    {
        EventHandler.RemoveListener(EEventID.EVENT_UPDATE_SAVE_POSITION, OnEventUpdateSavePos);
    }
}
