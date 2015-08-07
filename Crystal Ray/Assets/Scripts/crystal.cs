using UnityEngine;

namespace Assets.Scripts
{
	public class Crystal
	{
		//public crystal
		//{
		#region Fields
		
		//bool pressed = false;
		public Vector2 Position;
		public Vector3 Scale;
		public int Type;
		public int ColorIndex = 0;
		public Vector3 CrystalRotation;
		public string NameId;
		public GameObject Model;
		public Material Mat;
		public GameObject Tesseract;
		public int ColorInt = 1;
		public Vector3 RotationAngle;
		//Colors
		protected Color[] VisitColors = {
			Color.gray, //Dark grey
			Color.cyan, Color.green, Color.red, Color.magenta, Color.yellow,
			Color.white, Color.blue, Color.black};
			//new Color(255,165,0)}; //Orange
		#endregion
		
		#region Private Methods
		
		public Crystal(string id, Vector2 nPos, Vector2 fieldDimensions, gameMain parent)
		{
			//Type: 7% chance of being a void crystal, change later for smarter generation and conflict resolution?
			var crystalTypes = Random.Range(0,100);
			Type = crystalTypes < 7 ? 0 : 1;
			NameId = id;

			//Grants model manager
			Model = AssetManager.GetModels[0];//Resources.Load("Crystals/Prefabs/01_crystal") as GameObject;
			//Do we have a GetTextures part for it?
			Mat = Resources.Load("Crystals/Materials/Crystal01-Grey", typeof(Material)) as Material;

			//Set get it ready for the world
			Model.GetComponent<Renderer>().material = Mat;

            //Want in, but need it to be changed.
            CrystalRotation = new Vector3(Random.Range(0.2f, 0.4f), Random.Range(0.1f, 0.3f), Random.Range(0.05f, 0.2f));

            Tesseract = Object.Instantiate(Model);
			//tesseract.transform.SetParent(Camera.current.transform);
			Tesseract.transform.SetParent (parent.gameObject.transform);
			Tesseract.layer = parent.gameObject.layer;
			Tesseract.name = "crystal:"+NameId;

			//rotationAngle = new Vector3(Random.Range(0.5f,2.0f)-1.0f, Random.Range(0.5f,2.0f)-1.0f, Random.Range(0.5f,2.0f)-1.0f);
			RotationAngle = new Vector3(0.0f, 1.0f, 0.0f);

			FixPosition (nPos, fieldDimensions);

			Tesseract.GetComponent<Renderer>().material.color = VisitColors[Type];
			Tesseract.transform.Rotate(new Vector3(-45.0f, 0.0f, 0.0f));
		}
		
		public void Traveled()
		{
			ColorInt = (ColorInt + 1) % VisitColors.Length;
			Tesseract.GetComponent<Renderer>().material.color = VisitColors[ColorInt];
		}

		public void FixPosition(Vector2 pos, Vector2 fieldDimensions)
		{
			//Array positions
			Position = pos;
			
			const float multiplier = 1.0f;
			
			//Bad naming, visual location
			var visualPosition = new Vector3 (0.0f, 0.0f, 10.0f * multiplier);
			var positionOffset = new Vector3(Position.x + 0.5f - (fieldDimensions.x / 2.0f),
			                                     Position.y + 0.5f - (fieldDimensions.y / 2.0f),
			                                     0.0f);
			positionOffset.x *= 8.0f / fieldDimensions.x * multiplier;
			positionOffset.y *= 11.0f / fieldDimensions.y * multiplier;
			positionOffset += new Vector3(1.5f, 0.0f, 0.0f) * multiplier;
			visualPosition += positionOffset;
			//Since crystals are "tall" objects, width and depth will match
			//Decisions and math to be made later if we want other creative shapes/ratios
			Scale = new Vector3 (1.5f / fieldDimensions.x, 1.5f / fieldDimensions.y, 1.5f / fieldDimensions.x);
			Scale *= multiplier;
			Tesseract.transform.localPosition = visualPosition;
			Tesseract.transform.localScale = Scale;
		}
		
		/// <summary>
		/// Raises the mouse enter event.
		/// TODO: Replace with system that calculates positions
		/// </summary>
		/*void OnMouseOver()
		{
	        if (endlessModeScript.Instance().GamePlaying)
	            {
	                if (pressed && endlessModeScript.Instance().GetAbleToMove)
	                {
	                    endlessModeScript.Instance().DoStart();
	                }
	                endlessModeScript.Instance().doMove(this);
	            }

			//Placeholer thing
			int doIt = 1;

			if (globalData.GetComponent<gameVariables>().gamePlaying)
				if(pressed && globalData.GetComponent<gameVariables>().getAbleToMove)
					doIt = 0; //Tell the game to start somehow

			//Remove the warning
			if(doIt == 0)
				doIt = 1;
		}*/
		
		/// <summary>
		/// Update this instance.
		/// </summary>
		public void Update () 
		{
			Tesseract.transform.Rotate(RotationAngle * Time.deltaTime * 30.0f);
			
			/*if(Input.GetMouseButtonDown(0))
			{
				pressed = true;
			}
			if(Input.GetMouseButtonUp(0))
			{
				pressed = false;
			}*/
			//transform.Rotate(crystalRotation);
		}
		
		#endregion
	}
}
