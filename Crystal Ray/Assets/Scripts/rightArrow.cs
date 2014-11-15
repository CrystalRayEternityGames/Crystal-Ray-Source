using UnityEngine;
using System.Collections;

public class rightArrow : MonoBehaviour {

	Vector3 generalSizing;
	GameObject tutorialObject = null;

	// Use this for initialization
	void Start() 
	{
		tutorialObject = GameObject.FindGameObjectWithTag("MainCamera");
		transform.position = new Vector3(2f,-3.75f,0f);
		generalSizing = new Vector3(0.15f,0.15f,0.15f);
		transform.localScale = generalSizing;
	}

	/// <summary>
	/// Raises the mouse enter event.
	/// </summary>
	void OnMouseEnter()
	{
		renderer.material.color = Color.cyan;
	}

	/// <summary>
	/// Raises the mouse exit event.
	/// </summary>
	void OnMouseExit()
	{
		renderer.material.color = Color.white;
	}

	/// <summary>
	/// Raises the mouse down event.
	/// </summary>
	void OnMouseDown()
	{
		tutorialObject.GetComponent<tutorial>().GetSetTutorialIndex += 1;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		if(tutorialObject.GetComponent<tutorial>().GetSetTutorialIndex == 6)
		{
			renderer.enabled = false;
			collider.enabled = false;
		} else {
			renderer.enabled = true;
			collider.enabled = true;
		}
	}
}
