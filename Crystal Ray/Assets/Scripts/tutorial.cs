using UnityEngine;
using System.Collections;

public class tutorial : MonoBehaviour {
	
	GameObject plane;
	public Texture[] tutorialImages;
	int index = 0;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() 
	{
		plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane.GetComponent<Renderer>().transform.position = new Vector3(0f, 0f, 0f);
		plane.GetComponent<Renderer>().transform.Rotate(90f, 180f,0f);
		plane.GetComponent<Renderer>().transform.localScale = new Vector3(1f, 3f, .5f);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		if(index < 0)
		{
			index = 0;
		} else if(index > 6) {
			index = 6;
		} else {
			plane.GetComponent<Renderer>().material.mainTexture = tutorialImages[index];
		}
	}

	/// <summary>
	/// Gets or sets the index of the get set tutorial.
	/// </summary>
	/// <value>The index of the get set tutorial.</value>
	public int GetSetTutorialIndex
	{
		get{return index;}
		set{index = value;}
	}
}
