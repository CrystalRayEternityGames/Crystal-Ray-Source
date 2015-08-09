using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class score : MonoBehaviour {

	#region Fields

	public Text[] texts;
	public Canvas canvas;
	GameObject globalData;

	#endregion

	#region Private Methods

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		//Sets Global Data
		globalData = GameObject.FindGameObjectWithTag("Global");
		if (globalData == null) {
			globalData = new GameObject();
			globalData.AddComponent<gameVariables>();
			globalData.tag = "Global";
			globalData.name = "Global Data";
		}

	}

	void Update()
	{
		//Grabs the Children text under the GameObject
		texts = canvas.gameObject.GetComponentsInChildren <Text> ();
		
		//Sets text of Current Score
		texts[0].text = "Current: " + globalData.GetComponent<gameVariables>().GetSetLevelsCompleted.ToString();
		
		//Sets text of High Score
		texts[1].text = "High Score: " + PlayerPrefs.GetInt("Score").ToString();
	}

	#endregion
}
