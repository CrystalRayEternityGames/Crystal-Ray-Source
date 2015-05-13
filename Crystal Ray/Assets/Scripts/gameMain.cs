using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssemblyCSharp;

public class gameMain : MonoBehaviour
{
	
	#region Fields
	
	public AudioClip mouseOver;
	public Font gameFont;
	public Material textMaterial;
	//private static pathCreation instance = null;
	
	//Begin the ackwarding
	
	public crystal[,] field;
	public List<int> indexX = new List<int>();
	public List<int> indexY = new List<int>();
	protected List<crystal> generatedPath = new List<crystal>();
	protected List<crystal> playerPath = new List<crystal>();
	
	//End such shinanigens
	
	//protected GameObject crystal = null;
	protected GameObject globalData = null;
	//protected List<GameObject> generatedPath = new List<GameObject>();
	//protected List<GameObject> playerPath = new List<GameObject>();
	protected GameObject current = null;
	int playerProgress = 0;
	//protected Color[] visitColors = new Color[] {Color.green, Color.red, Color.magenta, Color.yellow, orange};
	protected bool started = false;
	
	protected int fieldWidth; 
	protected int fieldHeight;
	protected float scaleWidth;
	protected Vector2 fieldOffset;
	protected int pass = 0;
	protected int crystalCount = 0;
	protected float timer = 1.0f;
	
	protected bool ableToMove = false;
	protected bool playing = true;
	#endregion
	
	#region Private Methods
	
	/// <summary>
	/// Resets the index lists
	/// </summary>
	private void indexReset(List<int> index, int fSize)
	{
		//Empty index, refill with 0 to fSize
		index.Clear();
		for(int i = 0; i < fSize; i++)
		{
			index.Add (i);
		}
	}
	
	/// <summary>
	/// Creates the field.
	/// </summary>
	#if NET_VERSION_4_5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	#endif
	protected void CreateField ()
	{
		float screenWidth = (float)Screen.width;
		float screenHeight = (float)Screen.height;
		
		//Clear the x and y indexes
		indexReset(indexX, fieldWidth);
		indexReset(indexY, fieldHeight);
		field = new crystal[fieldWidth, fieldHeight];
		
		//Todo, impliment max values
		//field = new GameObject[fieldWidth, fieldHeight];
		
		//Enforce aspect ratio of 16/9
		//int widerSide = screenWidth/16.0f < screenHeight/9.0f ? 0 : 1;
		
		//Create the grid
		//Doing <= so we can use fieldWidth and such without -1
		for (int i = 0; i < fieldWidth; i++) {
			for (int j = 0; j < fieldHeight; j++) {
				
				//Arguments: name, position, scale
				
				//Name
				pass++;
				
				//Position just uses i and j indexes, adjusting position will be handled by each crystal
				
				//Handle scaling
				var dimensions = new Vector2((float)fieldWidth,(float)fieldHeight);
				
				field[indexX[i], indexY[j]] = new crystal(pass.ToString(), new Vector2(indexX[i], indexY[j]), dimensions, gameObject);

				/*
				float scaleWidth = 9f / fieldWidth * crystalScale;// * screenWidth / 1360f;
				float scaleHeight =  9f / fieldHeight * crystalScale;// * screenHeight / 740f;
				field[i, j].transform.position = new Vector3(((i - (fieldWidth / 1f) + 1f) * scaleWidth) + fieldOffset.x, ((j - (fieldHeight / 2f) + 1f) * scaleHeight) + fieldOffset.y, 0);
				field[i, j].transform.localScale = new Vector3(scaleWidth / 6.0f, scaleHeight / 6.0f, (scaleWidth + scaleHeight)/3f / 6.0f);
				*/
			}
		}
		
		//Generate Path
		while(!generatePath())
			continue;
	}
	
	public bool generatePath()
	{
		//Let's make a path!
		//Length of path to travel
		generatedPath = new List<crystal>();
		
		int currentLevel = (int)Mathf.Round(globalData.GetComponent<gameVariables>().GetSetLevelsCompleted * 0.5f) + 1;
		
		//Generate until not on a void, if infinite loop, all ledges are void...
		while(generatedPath.Count < 1)
		{
			//starting edge
			int startSide = Random.Range(0,3);
			//Get start point for the side, % 2 to see if we go height or width for odd or even
			int startPoint = Random.Range(0, startSide % 2 == 0 ? fieldHeight : fieldWidth);
			//0 = right, 1 = top, 2 = left, 3 = up
			int x = startSide % 2 == 0 ? startSide == 0 ? fieldWidth - 1 : 0 : startPoint;
			int y = startSide % 2 == 0 ? startPoint : startSide == 1 ? 0 : fieldHeight - 1;
			//Start the path on the correct point
			if(field[indexX[x],indexY[y]].type != 0)
				generatedPath.Add(field[indexX[x],indexY[y]]);
		}
		
		//Pick starting direction
		int currentDirection = Random.Range(0,3);
		int[] triedDirections = new int[] {0,0,0,0};
		
		//Start traveling
		while(currentLevel > 0)
		{
			bool good = true;
			Vector2 currentPosition = generatedPath[generatedPath.Count-1].position;
			Vector2 posMove = new Vector2(0,0);

			//Find if picked direction is good
			switch(currentDirection)
			{
				//Right
			case (0):
				posMove.x += 1;
				break;
				//Up
			case (1):
				posMove.y += -1;
				break;
				//Left
			case (2):
				posMove.x += -1;
				break;
				//Down
			case (3):
				posMove.y += 1;
				break;
			}

			triedDirections[currentDirection] = 1;
			currentPosition += posMove;
			good = checkMove(currentPosition);
			
			//If no issues with picked direction, move forward, get new direction
			if(good)
			{
				generatedPath.Add(field[indexX[(int)currentPosition.x], indexY[(int)currentPosition.y]]);
				currentDirection = getDirection(currentDirection, triedDirections);
				triedDirections = new int[] {0,0,0,0};
				currentLevel--;
			} else {
				//Grab direction, if stuck crash it?
				currentDirection = getDirection(currentDirection, triedDirections);
				if(currentDirection == -1)
					return false;
			}
		}
		
		//Finished path
		return true;
	}
	
	public bool checkMove(Vector2 posCheck)
	{
		if (posCheck.x < 0 || posCheck.y < 0)
			return false;
		if ((int)posCheck.x >= fieldWidth || (int)posCheck.y >= fieldHeight)
			return false;
		if (field [(int)posCheck.x, (int)posCheck.y].type == 0)
			return false;
		return true;
	}
	
	//TODO: Implement better system that tries to keep track of direction used over time
	public int getDirection(int currentDirection, int[] triedDirections)
	{
		//Get list of directions to try
		List<int> dirList = new List<int>();
		for(int i = 0; i < 4; i++)
			if(triedDirections[i] == 0)
				dirList.Add(i);
		
		//All directions tried, infinite loop
		if(dirList.Count == 0)
			return -1;
		
		int newDirection = Random.Range(0,100);
		//Set directions, 15% backwards, 15% forward, %35 each side
		int picked = -1;
		//while(picked < 0)
		//{
		picked = newDirection < 15 ? 2 : newDirection < 30 ? 0 : newDirection < 65 ? 1 : 3;
		//	if(!dirList.Contains(picked))
		//		picked = -1;
		//}
		
		return (currentDirection + picked) % 4;
	}
	
	//Player has moved
	public void doMove(crystal usedCrystal)
	{
		//Only move forward only if on a different crystal
		if(current != usedCrystal && started)
		{
			//Give player path to follow later
			playerPath.Add(usedCrystal);
			
			//Set color to traveled
			usedCrystal.traveled();
			
			//Move color index forward, account for fixed length and roll over
			//usedCrystal.gameObject.GetComponent<crystal>().colorIndex = usedCrystal.gameObject.GetComponent<crystal>().colorIndex + 1 % visitColors.Length;
			
			//Correct move
			if(generatedPath[playerProgress] == usedCrystal)
			{
				GetComponent<AudioSource>().PlayOneShot(mouseOver);
				//Particles, temp removed?
				//generatedPath[playerProgress].renderer.particleSystem.Play();
				crystalCount++;
				if(playerProgress == generatedPath.Count - 1)
				{
					globalData.GetComponent<gameVariables>().GetSetLevelsCompleted += 1;
					Application.LoadLevel("gameWorld");
				}
				current = usedCrystal.gameObject;
				playerProgress++;
				//Bad move
			} else {
				//Do gameover, make it fancy later
				//Application.LoadLevel("mainMenu");
				ableToMove = false;
				playing = false;
				//PopUp();
				globalData.GetComponent<gameVariables>().GetSetFailed = true;
			}
		}
	}
	
	//Start the game
	//Jade reminder: Handle this
	public void doStart()
	{
		started = true;
	}
	
	/// <summary>
	/// Sets the variables for difficulties.
	/// </summary>
	/// <param name="difficulty">Difficulty.</param>
	protected virtual void SetVariables()
	{
		float timeDecrease = 0;
		int increaseWidth = 0;
		int increaseHeight = 0;
		fieldOffset = new Vector2(3.0f, -0.3f);
		int levels = globalData.GetComponent<gameVariables>().GetSetLevelsCompleted;
		
		if (levels > 1 && levels <= 3)
		{
			timeDecrease = Random.Range(0.05f, 0.07f);
			
		}
		else if (levels > 3 && levels <= 8)
		{
			timeDecrease = Random.Range(0.06f, 0.08f);
			increaseWidth = 1;
		}
		else if (levels > 8 && levels <= 20)
		{
			timeDecrease = Random.Range(0.07f, 0.1f);
			increaseWidth = 1;
			increaseHeight = 1;
		}
		else if (levels > 20)
		{
			timeDecrease = Random.Range(0.1f, 0.15f);
			increaseWidth = 2;
			increaseHeight = 2;
		}
		
		fieldHeight = 4 + increaseHeight;
		fieldWidth = 4 + increaseWidth;
		timer = Random.Range(0.5f - timeDecrease, 0.7f - timeDecrease);
	}
	
	/// <summary>
	/// Start the Game.
	/// </summary>
	protected virtual void Start ()
	{
		globalData = GameObject.FindGameObjectWithTag("Global");
		if (globalData == null) {
			globalData = new GameObject();
			globalData.AddComponent<gameVariables>();
			globalData.tag = "Global";
		}
		SetVariables();
		CreateField();
		StartCoroutine("ChangeTexture");
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	protected virtual void Update ()
	{
		scaleWidth = (float)Screen.width / (float)Screen.height * 0.8f; //0.8 is reverse of 5:4 ratio, 60f is camera default fieldofview
		var temp = Camera.main.gameObject.transform.localScale;//this.gameObject.transform.localScale;
		foreach (Camera cam in Camera.allCameras)
						cam.aspect = 1;

		if (Input.GetKey(KeyCode.Escape)) 
		{
			globalData.GetComponent<gameVariables>().SaveScore();
			Application.LoadLevel("mainMenu");
			globalData.GetComponent<gameVariables>().GetSetLevelsCompleted = 0;
		}
	}
	
	#endregion
	
	#region Other Methods
	
	/// <summary>
	/// Changes the texture of the Crystal.
	/// </summary>
	/// <returns>The texture.</returns>
	protected IEnumerator ChangeTexture ()
	{
		/*   The temporary color thing for showing the path to the player
		if(globalData.GetComponent<gameVariables>().GetSetLevelsCompleted < 3)
		{
			for(int i = 0; i < generatedPath.Count; i++)
			{
				if(generatedPath[i].gameObject.renderer.material.color == globalData.GetComponent<gameVariables>().GetSetAIPathColor)
				{
					generatedPath[i].gameObject.renderer.material.color = orange;
				} else {
					generatedPath[i].gameObject.renderer.material.color = globalData.GetComponent<gameVariables>().GetSetAIPathColor;
				}
				yield return new WaitForSeconds(timer);
			}

			for(int j = 0; j < generatedPath.Count; j++)
			{
				generatedPath[j].renderer.material.color = Color.cyan;
			}
		} else {*/
		for(int i = 0; i < generatedPath.Count; i++)
		{
			Color tempColor = generatedPath[i].tesseract.GetComponent<Renderer>().material.color;
			generatedPath[i].tesseract.gameObject.GetComponent<Renderer>().material.color = globalData.GetComponent<gameVariables>().GetSetAIPathColor;
			yield return new WaitForSeconds(timer);
			generatedPath[i].tesseract.gameObject.GetComponent<Renderer>().material.color = tempColor;
		}
		//}
		
		
		ableToMove = true;
	}
	
	/*protected void PopUp()
	{
		float screenWidth = (float)Screen.width;
		float screenHeight = (float)Screen.height;
		GameObject plane;
		Vector3 generalSizing = new Vector3(0.05f,0.05f,0f);
		GameObject menu = new GameObject();
		menu.AddComponent<TextMesh>();
		menu.AddComponent<BoxCollider>();
		menu.AddComponent<popupBack>();
		menu.AddComponent<AudioSource>();
		GameObject restartGame = new GameObject();
		restartGame.AddComponent<TextMesh>();		
		restartGame.AddComponent<BoxCollider>();
		restartGame.AddComponent<restart>();
		restartGame.AddComponent<AudioSource>();
		GameObject generalText = new GameObject();
		generalText.AddComponent<TextMesh>();	
		
		plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane.GetComponent<Renderer>().transform.position = new Vector3(0f, 1f, -3.8f);
		plane.GetComponent<Renderer>().transform.Rotate(270f,0f,0f);
		plane.GetComponent<Renderer>().transform.localScale = new Vector3((screenWidth / screenHeight * 16 / 9) * .15f, 1f, (screenWidth / screenHeight) * .15f);
		plane.GetComponent<Renderer>().material.color = Color.gray;
		
		menu.GetComponent<TextMesh>().text = "Main Menu";
		menu.GetComponent<TextMesh>().font = gameFont;
		menu.GetComponent<TextMesh>().color = Color.white;
		menu.GetComponent<TextMesh>().fontSize = 80;
		menu.GetComponent<TextMesh>().alignment = TextAlignment.Left;
		menu.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
		menu.GetComponent<BoxCollider>().size = new Vector3(50f, 10f, 0f);
		menu.GetComponent<BoxCollider>().center = new Vector3(0f,1f,2f);
		menu.GetComponent<Renderer>().material = textMaterial;
		menu.transform.position = new Vector3(-1f, 0f, -4f);
		menu.transform.localScale = generalSizing;
		
		restartGame.GetComponent<TextMesh>().text = "Restart";
		restartGame.GetComponent<TextMesh>().font = gameFont;
		restartGame.GetComponent<TextMesh>().color = Color.white;
		restartGame.GetComponent<TextMesh>().fontSize = 80;
		restartGame.GetComponent<TextMesh>().alignment = TextAlignment.Left;
		restartGame.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
		restartGame.GetComponent<BoxCollider>().size = new Vector3(50f, 10f, 0f);
		restartGame.GetComponent<BoxCollider>().center = new Vector3(0f,1f,2f);
		restartGame.GetComponent<Renderer>().material = textMaterial;
		restartGame.transform.position = new Vector3(1.5f, 0f, -4f);
		restartGame.transform.localScale = generalSizing;
		
		generalText.GetComponent<TextMesh>().text = "Wrong Path Crystal!";
		generalText.GetComponent<TextMesh>().font = gameFont;
		generalText.GetComponent<TextMesh>().color = Color.white;
		generalText.GetComponent<TextMesh>().fontSize = 70;
		generalText.GetComponent<TextMesh>().alignment = TextAlignment.Left;
		generalText.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
		generalText.GetComponent<Renderer>().material = textMaterial;
		generalText.transform.position = new Vector3(0.25f, 1.5f, -4f);
		generalText.transform.localScale = generalSizing;
	}*/
	#endregion
	
	#region Public Methods
	
	/// <summary>
	/// Instance this instance.
	/// </summary>
	/*public new static pathCreation Instance()
	{
		if(!instance)
		{
			instance = GameObject.FindObjectOfType(typeof(pathCreation)) as pathCreation;
		}
		return instance;
	}*/
	
	#endregion
	
	#region Properties
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="pathCreation"/> get able to move.
	/// </summary>
	/// <value><c>true</c> if get able to move; otherwise, <c>false</c>.</value>
	public bool GetAbleToMove
	{
		get{return ableToMove;}
	}
	
	public bool GamePlaying
	{
		get{return playing;}
	}
	
	#endregion
}