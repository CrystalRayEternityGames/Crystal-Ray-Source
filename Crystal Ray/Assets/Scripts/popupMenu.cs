using UnityEngine;
using System.Collections;

public class popupMenu : MonoBehaviour {

	public Font gameFont;
	public Material textMaterial;

	public void PopUp()
	{
		float screenWidth = (float)Screen.width;
		float screenHeight = (float)Screen.height;
		GameObject plane;
		Vector3 generalSizing = new Vector3(0.05f,0.05f,0f);
		GameObject menu = new GameObject();
		menu.AddComponent<TextMesh>();
		menu.AddComponent<BoxCollider>();
		GameObject restart = new GameObject();
		restart.AddComponent<TextMesh>();
		
		plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane.renderer.transform.position = new Vector3(0f, 1f, -4f);
		plane.renderer.transform.Rotate(270f,0f,0f);
		plane.renderer.transform.localScale = new Vector3((screenWidth / screenHeight * 16 / 9) * .25f, 1f, (screenWidth / screenHeight) * .25f);
		plane.renderer.material.color = Color.white;
		
		menu.GetComponent<TextMesh>().text = "Main Menu";
		menu.GetComponent<TextMesh>().font = gameFont;
		menu.GetComponent<TextMesh>().color = Color.black;
		menu.GetComponent<TextMesh>().fontSize = 100;
		menu.GetComponent<TextMesh>().alignment = TextAlignment.Left;
		menu.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
		menu.renderer.material = textMaterial;
		menu.transform.position = new Vector3(0f, 1f, -4f);
		menu.transform.localScale = generalSizing;
	}
}
