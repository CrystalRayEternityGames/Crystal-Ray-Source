using UnityEngine;
using System.Collections;

public class gameVariables : MonoBehaviour {

	#region Fields

	int highScore, levelsCompleted;
	Color playerColor, aiPathColor;
	bool easterEgg;
	public int height = 0;
	public int width = 0;
	//Color orange = new Color(1f, 0.647f, 0f);
	public bool gamePlaying = false;
	public bool getAbleToMove = false;
	public bool doStart = false;
	public bool failed = false;
	#endregion

	#region Private Methods

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		height = Screen.currentResolution.height;
		width = Screen.currentResolution.width;

		levelsCompleted = 0;
		playerColor = Color.green;
		aiPathColor = Color.yellow;
		easterEgg = false;
		
		if(!PlayerPrefs.HasKey("Score") || !PlayerPrefs.HasKey("Tutorial"))
		{
			PlayerPrefs.SetInt("Score", 0);
			PlayerPrefs.SetInt("Tutorial", 0);
		}
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		DontDestroyOnLoad(this);
	}

	/// <summary>
	/// Saves player score.
	/// </summary>
	/// <param name="difficulty">Difficulty.</param>
	public void SaveScore()
	{
		int currentScore = GetSetLevelsCompleted;
		if(PlayerPrefs.GetInt("Score") < currentScore)
		{
			PlayerPrefs.SetInt("Score", GetSetLevelsCompleted);
			PlayerPrefs.Save();
		}
		GetSetLevelsCompleted = 0;
	}

	#endregion

	#region Properties

	/// <summary>
	/// Gets or sets the color of the Player.
	/// </summary>
	/// <value>The color of the Player.</value>
	public Color GetSetPlayerColor
	{
		get{return playerColor;}
		set{playerColor = value;}
	}

	/// <summary>
	/// Gets or sets the color of the AI Path Color.
	/// </summary>
	/// <value>The color of the Player.</value>
	public Color GetSetAIPathColor
	{
		get{return aiPathColor;}
		set{aiPathColor = value;}
	}

	/// <summary>
	/// Gets or sets the get set levels completed.
	/// </summary>
	/// <value>The get set levels completed.</value>
	public int GetSetLevelsCompleted
	{
		get{return levelsCompleted;}
		set{levelsCompleted = value;}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="gameVariables"/> get set easter egg.
	/// </summary>
	/// <value><c>true</c> if get set easter egg; otherwise, <c>false</c>.</value>
	public bool GetSetEasterEgg
	{
		get{return easterEgg;}
		set{easterEgg = value;}
	}

	/// <summary>
	/// Gets or sets the height of the get set game.
	/// </summary>
	/// <value>The height of the get set game.</value>
	public int GetSetGameHeight
	{
		get{return height;}
		set{height = value;}
	}

	/// <summary>
	/// Gets or sets the width of the get set game.
	/// </summary>
	/// <value>The width of the get set game.</value>
	public int GetSetGameWidth
	{
		get{return width;}
		set{width = value;}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="gameVariables"/> get set failed.
	/// </summary>
	/// <value><c>true</c> if get set failed; otherwise, <c>false</c>.</value>
	public bool GetSetFailed
	{
		get {return failed;}
		set{GetSetFailed = value;}
	}
	#endregion
}
