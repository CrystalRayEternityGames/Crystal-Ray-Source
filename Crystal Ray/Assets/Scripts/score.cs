using UnityEngine;
using System.Collections;

public class score : MonoBehaviour {

	#region Fields
	
	public Font gameFont;
	public Material textMaterial;
	Vector3 generalSizing;
	GameObject playerScore = null;
	GameObject highScore = null;
	GameObject globalData = null;
	int current, high;
	
	#endregion
	
	#region Private Methods
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		playerScore = new GameObject();
		playerScore.AddComponent<TextMesh>();
		highScore = new GameObject();
		highScore.AddComponent<TextMesh>();
		generalSizing = new Vector3(0.05f,0.05f,0f);
		wait (2); //If needed, this makes score wait for a second if needed to grab the object
		globalData = GameObject.FindGameObjectWithTag("Global");
		current = globalData.GetComponent<gameVariables>().GetSetLevelsCompleted;
		high = PlayerPrefs.GetInt("Score");
		
		GenerateScore();
	}

	//Easy way to get something to wait
	IEnumerator wait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		}
	
	/// <summary>
	/// Generates the score.
	/// </summary>
	void GenerateScore()
	{
		playerScore.GetComponent<TextMesh>().text = "Current: " + current.ToString();
		playerScore.GetComponent<TextMesh>().font = gameFont;
		playerScore.GetComponent<TextMesh>().fontSize = 100;
		playerScore.GetComponent<TextMesh>().alignment = TextAlignment.Left;
		playerScore.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
		playerScore.GetComponent<Renderer>().material = textMaterial;
		playerScore.transform.position = new Vector3(Screen.width / Screen.height * 16 / 9 - 6.5f, (Screen.width / Screen.height) + 4.7f, 0);//(Screen.width/16/9) * 2f,(Screen.height/16/9) * 2.7f,0f);
		playerScore.transform.localScale = generalSizing;

		highScore.GetComponent<TextMesh>().text = "High Score: " + high.ToString();
		highScore.GetComponent<TextMesh>().font = gameFont;
		highScore.GetComponent<TextMesh>().fontSize = 100;
		highScore.GetComponent<TextMesh>().alignment = TextAlignment.Left;
		highScore.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
		highScore.GetComponent<Renderer>().material = textMaterial;
		highScore.transform.position = new Vector3(Screen.width / Screen.height * 16 / 9 - 6.5f, (Screen.width / Screen.height) + 5.2f, 0);//(Screen.width/16/9) * 2f,(Screen.height/16/9) * 3f,0f);
		highScore.transform.localScale = generalSizing;
	}
	
	#endregion
}
