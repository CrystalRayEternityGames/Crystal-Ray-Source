using UnityEngine;
using System.Collections;

public class backToMain : MonoBehaviour {

	public AudioClip menuSelection;

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
		Application.LoadLevel("mainMenu");
		GetComponent<AudioSource>().PlayOneShot(menuSelection);
	}
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.LoadLevel("mainMenu");
			GetComponent<AudioSource>().PlayOneShot(menuSelection);
		}
	}
}
