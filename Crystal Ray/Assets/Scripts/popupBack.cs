using UnityEngine;
using System.Collections;

public class popupBack : MonoBehaviour {

	GameObject globalData = GameObject.FindGameObjectWithTag("Global");

	AudioClip menuSelection = Resources.Load("Sounds/crystal_highlight") as AudioClip;

	//Mouse Enters a Box Collider
	void OnMouseEnter()
	{
		GetComponent<TextMesh>().color = Color.cyan;
	}
	//Mouse Leaves a Box Collider
	void OnMouseExit()
	{
		GetComponent<TextMesh>().color = Color.white;
	}
	//Mouse Clicks a Box Collider
	void OnMouseDown()
	{
		Application.LoadLevel("mainMenu");
        
		globalData.GetComponent<gameVariables>().SaveScore();

		/*if (pathCreation.Instance() != null)
        {
            pathCreation.Instance().SaveScore();
        }
        else
        {
            endlessModeScript.Instance();
        }*/
		
        audio.PlayOneShot(menuSelection);
	}
}
