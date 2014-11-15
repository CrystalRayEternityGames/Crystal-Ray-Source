using UnityEngine;
using System.Collections;

public class background : MonoBehaviour {
	
	GameObject plane = null;
	GameObject globalData = null;

	// Use this for initialization
	void Start() 
	{
		float screenWidth = (float)Screen.width;
		float screenHeight = (float)Screen.height;

		globalData = GameObject.FindGameObjectWithTag("Global");

		Texture backgroundImage = Resources.Load("Images/background")as Texture;
		plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane.renderer.transform.position = new Vector3(0f, 1f, 4);
		plane.renderer.transform.Rotate(270f,0f,0f);//If you want it flipped to be something normal: (90f, 180f,0f);
		plane.renderer.transform.localScale = new Vector3(screenWidth / screenHeight * 16 / 9, 1f, screenWidth / screenHeight);//.65f);
		//plane.renderer.transform.localScale = new Vector3(3.5f * 9.0f / 16.0f * screenWidth / screenHeight, 3f * 9.0f / 16.0f * screenWidth / screenHeight, 1.65f);
		plane.renderer.material.mainTexture = backgroundImage;
	}
}
