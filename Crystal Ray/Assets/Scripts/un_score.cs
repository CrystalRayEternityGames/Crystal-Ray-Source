using UnityEngine;
using System.Collections;

public class un_score : MonoBehaviour {

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
		globalData = GameObject.FindGameObjectWithTag("Global");
		if (globalData == null) {
			globalData = new GameObject();
			globalData.AddComponent<gameVariables>();
			globalData.tag = "Global";
		}
		current = globalData.GetComponent<gameVariables>().GetSetLevelsCompleted;
		high = PlayerPrefs.GetInt("Score");

		var p = new Vector3(-4.5f, 4.5f, 1f);
		var height = 0.7f;
		GenerateScore(playerScore, "Current: " + current.ToString(), "Current score string", p);
		GenerateScore(highScore, "High Score: " + high.ToString(), "High score string", new Vector3(p.x,p.y-height,p.z));
	}

	//Easy way to get something to wait
	IEnumerator wait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		}
	
	/// <summary>
	/// Generates the score.
	/// </summary>
	void GenerateScore(GameObject obj, string str, string name, Vector3 pos)
	{
		obj.GetComponent<TextMesh>().text = str;
		obj.GetComponent<TextMesh>().font = gameFont;
		obj.GetComponent<TextMesh>().fontSize = 100;
		obj.GetComponent<TextMesh>().alignment = TextAlignment.Left;
		obj.GetComponent<TextMesh>().anchor = TextAnchor.UpperLeft;
		obj.GetComponent<Renderer>().material = textMaterial;
		obj.transform.SetParent (transform);
		obj.layer = gameObject.layer;
		obj.name = name;
		obj.transform.localPosition = pos;//(Screen.width/16/9) * 2f,(Screen.height/16/9) * 2.7f,0f);
		obj.transform.localScale = generalSizing;
	}
	
	#endregion
}
