using UnityEngine;
using System.Collections;

public class restart : MonoBehaviour {

	AudioClip menuSelection = Resources.Load("Sounds/crystal_highlight") as AudioClip;
	GameObject globalData = GameObject.FindGameObjectWithTag("Global");

    const string NAME = "endlessMode";
	//Mouse Enters a Box Collider
	void OnMouseEnter()
	{
		GetComponent<Renderer>().material.color = Color.cyan;
	}
	//Mouse Leaves a Box Collider
	void OnMouseExit()
	{
		GetComponent<Renderer>().material.color = Color.white;
	}
	//Mouse Clicks a Box Collider
	void OnMouseDown()
	{
		globalData.GetComponent<gameVariables>().SaveScore();
        
		//Grant, Jade, if you see this/use this again, look into it more to fix it up

		Application.LoadLevel(Application.loadedLevelName == NAME ? NAME : "gameWorld");
		GetComponent<AudioSource>().PlayOneShot(menuSelection);
	}
}
