using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class endlessModeScript : gameMain
{
    #region Fields
    //private static endlessModeScript instance = null;
    #endregion

	protected override void SetVariables ()
	{
		float timeDecrease = Random.Range (0.07f, 0.1f);
		fieldHeight = 10;
		fieldWidth = 10;
		timer = Random.Range (0.5f - timeDecrease, 0.7f - timeDecrease);
		base.SetVariables ();
	}

    private void ClearColor()
    {
        for (int i = 0; i < generatedPath.Count - 1; i++)
        {
            generatedPath[i].type = 0;
            //generatedPath[i].renderer.material.color = Color.cyan;
            Destroy(generatedPath[i].tesseract.GetComponent<ParticleSystem>());
        }

        playerPath.ForEach(crst => 
        { 
            
            crst.type = 1;
            //crst.renderer.material.color = Color.cyan;
            Destroy(crst.tesseract.GetComponent<ParticleSystem>());
        });

        while (!generatePath())
        {
            continue;
        }
        StartCoroutine("ChangeTexture");
    }

    
    /*public new static endlessModeScript Instance()
    {
        if (!instance)
        {
            instance = GameObject.FindObjectOfType(typeof(endlessModeScript)) as endlessModeScript;
        }
        return instance;
    }*/

    

    public void DoStart()
    {
        started = true;
    }
	
    // Use this for initialization
	protected override void Start ()
    {
	    //Score setting player pref
        globalData = GameObject.FindGameObjectWithTag("Global");
		//crystal = Resources.Load("Crystals/Prefabs/01_crystal") as GameObject;
        SetVariables();
        CreateField();
        StartCoroutine("ChangeTexture");
	}

	// Update is called once per frame
	protected override void Update ()
	{
		base.Update ();
	}

    public new bool GetAbleToMove
    {
        get { return ableToMove; }
    }

    public new bool GamePlaying
    {
        get { return playing;  }
    }
}
