using UnityEngine;
using System.Collections;

public class mainMenu : MonoBehaviour {

	#region Fields

	public bool startGame = false;
	public bool tutorial = false;
    public bool endlessMode = false;
	public bool end = false;
	public bool credits = false;
	public AudioClip menuSelect;

	#endregion

	#region Private Methods

	//Mouse Enters a Box Collider
	void OnMouseEnter()
	{
		renderer.material.color = Color.cyan;
	}
	//Mouse Leaves a Box Collider
	void OnMouseExit()
	{
		renderer.material.color = Color.white;
	}
	//Mouse Clicks a Box Collider
	void OnMouseDown()
	{
		//Start Game
		if(startGame)
		{
			audio.PlayOneShot(menuSelect);
			Application.LoadLevel("gameWorld");
		}
        else if (endlessMode)
        {
            audio.PlayOneShot(menuSelect);
            Application.LoadLevel("endlessMode");
        }
        //Tutorial
        else if (tutorial)
        {
            audio.PlayOneShot(menuSelect);
            Application.LoadLevel("tutorial");
        }
        //Credits
        else if (credits)
        {
            audio.PlayOneShot(menuSelect);
            Application.LoadLevel("credits");
        }
        //End
        else if (end)
        {
            audio.PlayOneShot(menuSelect);
            Application.Quit();
        }
	}

	#endregion
}
