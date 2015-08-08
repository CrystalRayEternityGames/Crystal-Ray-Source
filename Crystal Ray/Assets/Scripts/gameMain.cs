using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using Assets.Scripts;

public class gameMain : MonoBehaviour
{
	
	#region Fields

    public GameEngine TheGameEngine;
	public enum GameMode {Standard, Arcade};
	public GameMode CurrentMode = GameMode.Standard;

	//public Font gameFont;
	//public Material textMaterial;

	protected GameObject GlobalData;
	#endregion
	
	/// <summary>
	/// Start the Game.
	/// </summary>
	protected virtual void Start ()
	{
		GlobalData = GameObject.FindGameObjectWithTag("Global");
		if (GlobalData == null) {
			GlobalData = new GameObject();
			GlobalData.AddComponent<gameVariables>();
			GlobalData.tag = "Global";
		}

		GetLevel ();
	}

    protected virtual void Update()
    {
		TheGameEngine.Update ();

		if (TheGameEngine.LevelComplete) {
			TheGameEngine.CleanupAssets();
			GlobalData.GetComponent<gameVariables>().GetSetLevelsCompleted += 1;
			GetLevel();
		}
    }

	protected void GetLevel()
	{
		switch (CurrentMode) {
		case GameMode.Standard:
			TheGameEngine = new GameEngine (this, GlobalData);
			CurrentMode = GameMode.Arcade;
			break;
		case GameMode.Arcade:
			TheGameEngine = new EndlessModeScript (this, GlobalData);
			CurrentMode = GameMode.Standard;
			break;
		}
	}
}