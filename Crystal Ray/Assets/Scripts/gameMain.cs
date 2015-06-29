using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssemblyCSharp;
using System;
using Random = UnityEngine.Random;

public class gameMain : MonoBehaviour
{
	
	#region Fields
	
	public AudioClip mouseOver;
	public Font gameFont;
	public Material textMaterial;
	//private static pathCreation instance = null;
	
	//Begin the ackwarding

	//Columns then rows
	public Dictionary<Guid, Dictionary<Guid, crystal>> field;
	object fieldLock = new object ();
	public List<Guid> indexX = new List<Guid>();
	public List<Guid> indexY = new List<Guid>();
	public Vector2 mousePos = new Vector2(-1f, -1f);
	protected Vector2 lastPos = new Vector2 (-1f, -1f);
	protected List<crystal> generatedPath = new List<crystal>();
	protected List<crystal> playerPath = new List<crystal>();
	
	//End such shinanigens

	protected GameObject globalData = null;
	//protected List<GameObject> generatedPath = new List<GameObject>();
	//protected List<GameObject> playerPath = new List<GameObject>();
	protected crystal current = null;
	int playerProgress = 0;
	protected bool started = false;
	protected bool additionMade = false;
	
	protected Vector2 fieldSize;
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
	private void indexReset(List<Guid> index, float fSize)
	{
		index.Clear();
		for(int i = 0; i < fSize; i++)
		{
			Guid t = Guid.NewGuid();
			while(index.Contains(t))
				t = Guid.NewGuid();

			index.Add (t);
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
		lock (fieldLock) {
			//Clear the x and y indexes
			indexReset (indexX, fieldSize.x);
			indexReset (indexY, fieldSize.y);

			//Create the grid
			//Doing <= so we can use fieldWidth and such without -1
			for (int i = 0; i < fieldSize.x; i++) {
				for (int j = 0; j < fieldSize.y; j++) {
					//Name
					pass++;

					//Clear nulls
					if (field == null)
						field = new Dictionary<Guid, Dictionary<Guid, crystal>> ();
					if (!field.ContainsKey (indexX [i]))
						field [indexX [i]] = new Dictionary<Guid, crystal> ();
					//Add the crystal
					field [indexX [i]] [indexY [j]] = new crystal (pass.ToString (), new Vector2 (i, j), fieldSize, gameObject);
				}
			}
		
			//Generate Path
			while (!generatePath())
				continue;
		}
	}

	protected void addRows(int index = -1, int numb = 1)
	{
		lock (fieldLock) {
			for(int i = 0; i < numb; i++)
			{
				Guid t = Guid.NewGuid();
				while(indexY.Contains(t))
					t = Guid.NewGuid();
				//Insert if index given
				if(index >= 0)
				{
					indexY.Insert(index+i, t);
				}
				//Otherwise add to the end
				else
				{
					indexY.Add(t);
				}
				fieldSize.y++;
				field.Keys.ToList().ForEach(ind=>
				                            field[ind][t] = new crystal("Newly made",
				                                    new Vector2(indexX.IndexOf(ind),indexY.IndexOf(t)),
				                                    fieldSize,
				                                    gameObject));
			}

			foreach (var p in field)
				foreach (var crys in p.Value)
					crys.Value.fixPosition(new Vector2(indexX.IndexOf(p.Key), indexY.IndexOf(crys.Key)), fieldSize);
		}
	}

	protected void addColumns(int index = -1, int numb = 1)
	{
		lock (fieldLock) {
			for(int i = 0; i < numb; i++)
			{
				Guid t = Guid.NewGuid();
				while(indexX.Contains(t))
					t = Guid.NewGuid();
				//Insert if index given
				if(index >= 0)
				{
					indexX.Insert(index+i, t);
				}
				//Otherwise add to the end
				else
				{
					indexX.Add(t);
				}
				fieldSize.x++;
				field[t] = new Dictionary<Guid, crystal>();
				field[indexX[index > 0 ? 0 : 1]].Keys.ToList().ForEach(ind=>field[t][ind] = new crystal("Newly made", new Vector2(indexX.IndexOf(t),indexY.IndexOf(ind)), fieldSize, gameObject));
			}
			
			foreach (var p in field)
				foreach (var crys in p.Value)
					crys.Value.fixPosition(new Vector2(indexX.IndexOf(p.Key), indexY.IndexOf(crys.Key)), fieldSize);
		}
	}
	
	public bool generatePath()
	{
		//Let's make a path!
		//Length of path to travel
		generatedPath = new List<crystal>();
		
		int currentLevel = (int)Mathf.Round(globalData.GetComponent<gameVariables>().GetSetLevelsCompleted * 0.5f) + 1;
		
		//Generate until not on a void, if infinite loop, all ledges are void...
		{
			//starting edge
			int startSide = Random.Range(0,3);
			//Get start point for the side, % 2 to see if we go height or width for odd or even
			int startPoint = (int)Random.Range(0, startSide % 2 == 0 ? fieldSize.y : fieldSize.x);
			//0 = right, 1 = top, 2 = left, 3 = up
			int x = startSide % 2 == 0 ? startSide == 0 ? (int)fieldSize.x - 1 : 0 : startPoint;
			int y = startSide % 2 == 0 ? startPoint : startSide == 1 ? 0 : (int)fieldSize.y - 1;
			//Start the path on the correct point
			if(field[indexX[x]][indexY[y]].type != 0)
				generatedPath.Add(field[indexX[x]][indexY[y]]);
		}
		
		//Pick starting direction
		int currentDirection = Random.Range(0,3);
		int[] triedDirections = new int[] {0,0,0,0};
		
		//Start traveling
		while(currentLevel > 0 && fieldSize.y > 1 && fieldSize.x > 1)
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
				generatedPath.Add(field[indexX[(int)currentPosition.x]][indexY[(int)currentPosition.y]]);
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
		if ((int)posCheck.x >= fieldSize.x || (int)posCheck.y >= fieldSize.y)
			return false;
		if (field [indexX[(int)posCheck.x]][indexY[(int)posCheck.y]].type == 0)
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
				current = usedCrystal;
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
		int lazymansNumber = 6;
		fieldSize.y = lazymansNumber + increaseHeight;
		fieldSize.x = lazymansNumber + increaseWidth;
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
		foreach (Camera cam in Camera.allCameras)
						cam.aspect = 1;

		foreach (Dictionary<Guid, crystal> row in field.Values)
			foreach (crystal crys in row.Values)
				crys.Update ();

		//RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		Vector2 min = field [indexX.FirstOrDefault()][indexY.FirstOrDefault()].tesseract.transform.position;
		Vector2 max = field [indexX.LastOrDefault()][indexY.LastOrDefault()].tesseract.transform.position;

		mousePos.x = Camera.main.ScreenToWorldPoint (Input.mousePosition).x - min.x;
		mousePos.x = Mathf.Floor((mousePos.x / (max.x - min.x) * (fieldSize.x - 1.0f)) + 0.5f);
		mousePos.y = Camera.main.ScreenToWorldPoint (Input.mousePosition).y - min.y;
		mousePos.y = Mathf.Floor((mousePos.y / (max.y - min.y) * (fieldSize.y - 1.0f)) + 0.5f);

		if(mousePos.x >= 0 && mousePos.x < fieldSize.x && mousePos.y >= 0 && mousePos.y < fieldSize.y)
			if(mousePos.x != lastPos.x || mousePos.y != lastPos.y)
				field [indexX [(int)mousePos.x]][indexY[(int)mousePos.y]].traveled ();

		lastPos.x = mousePos.x + 0.0f;
		lastPos.y = mousePos.y + 0.0f;

		if (Input.GetKey (KeyCode.Q)) {
			if (!additionMade) {
				addRows(mousePos.y < 0 ? 0 : mousePos.y >= fieldSize.y ? -1 : (int)mousePos.y, 1);
				additionMade = true;
			}
		} else if (Input.GetKey (KeyCode.W)) {
			if (!additionMade) {
				addColumns(mousePos.x < 0 ? 0 : mousePos.x >= fieldSize.x ? -1 : (int)mousePos.x, 1);
				additionMade = true;
			}
		} else {
			additionMade = false;
		}

		if (Input.GetKey (KeyCode.A)) {
			addRows(mousePos.y < 0 ? 0 : mousePos.y >= fieldSize.y ? -1 : (int)mousePos.y, 1);
		} else if (Input.GetKey (KeyCode.S)) {
			addColumns(mousePos.x < 0 ? 0 : mousePos.x >= fieldSize.x ? -1 : (int)mousePos.x, 1);
		}

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