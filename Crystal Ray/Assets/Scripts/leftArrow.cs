﻿using UnityEngine;
using System.Collections;

public class leftArrow : MonoBehaviour {
	
	Vector3 generalSizing;
	GameObject tutorialObject = null;
	
	// Use this for initialization
	void Start() 
	{
		tutorialObject = GameObject.FindGameObjectWithTag("MainCamera");
		transform.position = new Vector3(-2.5f,-3.75f,0f);
		generalSizing = new Vector3(0.15f,0.15f,0.15f);
		transform.localScale = generalSizing;
	}
	
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
	
	void OnMouseDown()
	{
		tutorialObject.GetComponent<tutorial>().GetSetTutorialIndex -= 1;
	}

	void Update()
	{
		if(tutorialObject.GetComponent<tutorial>().GetSetTutorialIndex == 0)
		{
			GetComponent<Renderer>().enabled = false;
			GetComponent<Collider>().enabled = false;
		} else {
			GetComponent<Renderer>().enabled = true;
			GetComponent<Collider>().enabled = true;
		}
	}
}
