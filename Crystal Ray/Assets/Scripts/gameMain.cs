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

        //Code here to pick alternate game modes besides GameEngine
        TheGameEngine = new GameEngine(this, GlobalData);
	}

    protected virtual void Update()
    {
		TheGameEngine.Update ();
    }
}