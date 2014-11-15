using UnityEngine;
using System.Collections;

public class transition : MonoBehaviour {

	int tutorialRun = 0;

	// Use this for initialization
	void Start () {
		StartCoroutine("ChangeScene");
		tutorialRun = PlayerPrefs.GetInt("Tutorial");
	}
	
	IEnumerator ChangeScene()
	{
		yield return new WaitForSeconds(3f);
		if(tutorialRun == 0)
		{
			Application.LoadLevel("tutorial");
			PlayerPrefs.SetInt("Tutorial", 1);
			PlayerPrefs.Save();
		} else {
			Application.LoadLevel("mainMenu");
		}
	}
}
