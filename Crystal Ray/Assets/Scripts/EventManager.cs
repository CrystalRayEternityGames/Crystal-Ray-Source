using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

    public delegate void ChangeMusic(int levelName);
    public static event ChangeMusic OnChange;

    void LateUpdate()
    {
        MusicChangeNow();
    }

    void MusicChangeNow()
    {
        if (Application.isLoadingLevel)
        {
            if (OnChange != null)
            {
                OnChange(Application.loadedLevel);
            }
        }
    }
}
