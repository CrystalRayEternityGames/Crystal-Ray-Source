using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class crystal : MonoBehaviour
	{
		//public crystal
		//{
		#region Fields
		
		GameObject globalData = null;
		//bool pressed = false;
		public Vector2 position;
		public Vector2 positionOffset;
		public Vector3 scale;
		public int type;
		public int colorIndex = 0;
		public Vector3 crystalRotation;
		public string nameId;
		public GameObject model;
		public Material mat;
		public GameObject tesseract;
		public int colorInt = 0;
		//Colors
		protected Color[] visitColors = new Color[] {
			Color.gray, //Dark grey
			Color.cyan, Color.green, Color.red, Color.magenta, Color.yellow,
			new Color(255,165,0)}; //Orange
		#endregion
		
		#region Private Methods
		
		/// <summary>
		/// Start this instance.
		/// </summary>
		void Start()
		{
			globalData = GameObject.FindGameObjectWithTag("Global");
			
			//Want in, but need it to be changed.
			crystalRotation = new Vector3(Random.Range (0.2f, 0.4f), Random.Range (0.1f, 0.3f), Random.Range (0.05f, 0.2f));
			//type = 1;
		}
		
		public crystal(string id, Vector2 nPos, Vector3 nScale)
		{
			//Type: 7% chance of being a void crystal, change later
			int crystalTypes = Random.Range(0,100);
			type = crystalTypes < 7 ? 0 : 1;
			
			nameId = id;
			position = nPos;
			positionOffset = position * 3.0f;
            positionOffset += new Vector2(-2.5f, -4.5f);
			scale = new Vector3(1.5f / nScale.x, 1.5f / nScale.y, 1.5f / nScale.z);
			
			//Grants Change here
			model = AssetManager.GetModels[0];//Resources.Load("Crystals/Prefabs/01_crystal") as GameObject;
			mat = Resources.Load("Crystals/Materials/Crystal01-Grey", typeof(Material)) as Material;
			
			model.GetComponent<Renderer>().material = mat;
			tesseract = Instantiate(model) as GameObject;
			tesseract.transform.SetParent (Camera.main.transform);
			tesseract.name = "crystal:"+nameId;
			
			tesseract.transform.localPosition = new Vector3(positionOffset.x, positionOffset.y, 1f);
			tesseract.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
			tesseract.GetComponent<Renderer>().material.color = visitColors[type];
		}
		
		public void traveled()
		{
			colorInt = (colorInt + 1) % visitColors.Length;
			tesseract.GetComponent<Renderer>().material.color = visitColors[colorInt];
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
		void Update () 
		{
			//Todo: Draw the tesseract
			
			
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
