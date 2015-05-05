using UnityEngine;
using System.Collections;

public class mainMenu : MonoBehaviour {

	#region Fields

	public GameObject introGUI;
	public GameObject menuGUI;
	public GameObject tutorialGUI;
	public GameObject creditGUI;

	#endregion

	#region Public Methods

	/// <summary>
	/// Changes the level.
	/// </summary>
	/// <param name="name">Name.</param>
	public void ChangeLevel(string name)
	{
		//GetComponent<AudioSource>().PlayOneShot(menuSelect);
		if(name == "gameWorld" || name == "endlessMode" || name == "tutorial")
		{
			Application.LoadLevel(name);
		}
		else
		{
			menuGUI.SetActive(false);
			if(name == "credits")
			{
				creditGUI.SetActive(true);
			}
			//For new tutotrial
			/*else if(name == "tutorial")
			{
				tutorialGUI.SetActive(true);
			}*/
		}
	}

	/// <summary>
	/// Back this instance.
	/// </summary>
	public void Back()
	{
		if(tutorialGUI.activeSelf == true)
		{
			tutorialGUI.SetActive(false);
		}
		else
		{
			creditGUI.SetActive(false);
		}

		menuGUI.SetActive(true);
	}

	#endregion
	
	#region Private Methods

	/// <summary>
	/// Threads the start.
	/// </summary>
	/// <returns>The start.</returns>
	IEnumerator ThreadStart()
	{
		yield return StartCoroutine(ChangeScene());
		menuGUI.SetActive(true);
		introGUI.SetActive(false);
	}

	/// <summary>
	/// Changes the scene.
	/// </summary>
	/// <returns>The scene.</returns>
	IEnumerator ChangeScene()
	{
		yield return new WaitForSeconds(3f);
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		StartCoroutine(ThreadStart());
	}

	#endregion
}
