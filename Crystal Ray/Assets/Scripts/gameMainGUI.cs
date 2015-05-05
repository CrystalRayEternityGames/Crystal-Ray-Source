using UnityEngine;
using System.Collections;

public class gameMainGUI : MonoBehaviour {

	#region Fields

	public GameObject scoreGUI;
	public GameObject gameOverGUI;
	public GameObject completedGUI;
	GameObject globalData;

	#endregion

	// Use this for initialization
	void Start () 
	{
		globalData = GameObject.FindGameObjectWithTag("Global");
		gameOverGUI.SetActive(false);
		completedGUI.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(globalData.GetComponent<gameVariables>().GetSetFailed == true)
		{
			gameOverGUI.SetActive(true);
		}
	}
}
