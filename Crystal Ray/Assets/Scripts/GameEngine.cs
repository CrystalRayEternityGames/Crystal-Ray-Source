using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class GameEngine
    {
        //Sound
        public AudioClip MouseOver;

        //Field
        public Dictionary<Guid[], Crystal> Field;
        protected object FieldLock = new object();
        public List<List<Guid>> Indexes = new List<List<Guid>>();
        public Vector2 MousePos = new Vector2(-1f, -1f);
        protected Vector2 LastPos = new Vector2(-1f, -1f);
        protected Vector2 FieldSize;
        protected Vector2 FieldOffset;
        protected int Pass;

        //Global stuff
        protected GameObject GlobalData;
        protected gameMain Parent;

        //Playing
        protected List<Crystal> GeneratedPath = new List<Crystal>();
        protected List<Crystal> PlayerPath = new List<Crystal>();
        protected Crystal Current;
        protected int PlayerProgress;
        protected bool Started = false;
        protected bool AbleToMove;
        protected bool Playing = true;

        //Debug stuff
        protected bool AdditionMade;
        protected float Timer = 1.0f;
        protected float TheZEffect = 0.0f;

        public GameEngine(gameMain parent, GameObject globals)
        {
            Parent = parent;
            GlobalData = globals;
            SetVariables();
			CreateField ();
            //ThreadPool.QueueUserWorkItem(new WaitCallback(ChangeTexture));
        }

        /// <summary>
        /// Resets the index lists
        /// </summary>
        private static void IndexReset(ICollection<Guid> index, float fSize)
        {
            index.Clear();
            for (var i = 0; i < (int)fSize; i++)
            {
                var t = Guid.NewGuid();
                while (index.Contains(t))
                    t = Guid.NewGuid();

                index.Add(t);
            }
        }

        /// <summary>
        /// Creates the field.
        /// </summary>
#if NET_VERSION_4_5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public virtual void CreateField()
        {
            lock (FieldLock)
            {
                if (Field == null)
                    Field = new Dictionary<Guid[], Crystal>(new GuidComparer());

                while (Indexes.Count < 2)
                    Indexes.Add(new List<Guid>());

                //Clear the x and y indexes
                IndexReset(Indexes[0], FieldSize.x);
                IndexReset(Indexes[1], FieldSize.y);

                //Create the grid
                //Doing <= so we can use fieldWidth and such without -1
                for (var i = 0; i < FieldSize.x; i++)
                {
                    for (var j = 0; j < FieldSize.y; j++)
                    {
                        //Name
                        Pass++;

                        //Add the crystal
                        Field[new[] { Indexes[0][i], Indexes[1][j] }] = new Crystal(Pass.ToString(), new Vector2(i, j), FieldSize, Parent);
                    }
                }

                //Generate Path
				GeneratePath();
            }
        }

        //If x, make it a column, else make it a row
        public void AddCrystals(int axis, int index, int numb)
        {
            lock (FieldLock)
            {
                for (var i = 0; i < numb; i++)
                {
                    var t = Guid.NewGuid();
                    while (Indexes[axis].Contains(t))
                        t = Guid.NewGuid();
                    //Insert if index given
                    if (index >= 0)
                    {
                        Indexes[axis].Insert(index, t);
                    }
                    //Otherwise add to the end
                    else
                    {
                        Indexes[axis].Add(t);
                    }
                    if (axis < 2)
                        FieldSize[axis]++;

                    var newIndex = Indexes[axis].IndexOf(t);

                    var newItemsCount = 0f;
                    if (axis == 0)
                        newItemsCount = FieldSize[1];
                    if (axis == 1)
                        newItemsCount = FieldSize[0];

                    for (var j = 0; j < newItemsCount; j++)
                    {
                        Guid[] ind;
                        Vector2 vec;
                        switch (axis)
                        {
                            case 0:
                                ind = new[] { t, Indexes[1][j] };
                                vec = new Vector2(newIndex, j);
                                break;
                            case 1:
                                ind = new[] { Indexes[0][j], t };
                                vec = new Vector2(j, newIndex);
                                break;
                            default:
                                continue;
                        }

                        Field[ind] = new Crystal("N", vec, FieldSize, Parent);
                    }
                }

                //Update what is on the field
                foreach (var cell in Field)
                    cell.Value.FixPosition(new Vector2(Indexes[0].IndexOf(cell.Key[0]), Indexes[1].IndexOf(cell.Key[1])), FieldSize);
            }
        }

        private void RemoveCrystals(int axis, int index, int numb)
        {
            lock (FieldLock)
            {
                //Cleanup time

                for (var i = 0; i < numb; i++)
                {
                    if (!Indexes.All(ind => ind.Any()))
                        break;

                    //Removal minimum size
                    if (Indexes[axis].Count() <= 1)
                        break;

                    var removeItemsCount = 0;
                    var debugCount = Field.Count();
                    try
                    {
                        if (axis == 0)
                            removeItemsCount = (int)FieldSize[1];
                        if (axis == 1)
                            removeItemsCount = (int)FieldSize[0];
                        for (var j = 0; j < removeItemsCount; j++)
                        {
                            //The index to find in the field dictionary
                            Guid[] toRemove;
                            if (axis == 0) //
                                toRemove = new[] { Indexes[0][index >= 0 ? (index >= Indexes[0].Count() ? Indexes[0].Count() - 1 : index) : Indexes[0].Count - 1], Indexes[1][j] };
                            else// if(axis == 1)
                                toRemove = new[] { Indexes[0][j], Indexes[1][index >= 0 ? (index >= Indexes[1].Count() ? Indexes[1].Count - 1 : index) : Indexes[1].Count - 1] };

                            Object.Destroy(Field[toRemove].Tesseract);
                            Field.Remove(toRemove);
                        }
                    }
                    catch
                    {
                        Debug.LogError(debugCount.ToString() + " minus " + removeItemsCount.ToString() + " should equal " + Field.Count().ToString());
                    }

                    try
                    {
                        if (index >= 0)
                        {
                            Indexes[axis].RemoveAt(index);
                        }
                        //Otherwise add to the end
                        else
                        {
                            Indexes[axis].RemoveAt(Indexes[axis].Count - 1);
                        }
                    }
                    catch
                    {
                        Debug.LogError("Failed to remove " + index.ToString() + " from indexes");
                    }

                    try
                    {
                        if (axis < 2)
                            FieldSize[axis]--;
                    }
                    catch
                    {
                        Debug.LogError("Failed to decrement fieldSize");
                    }
                }

                //Update what is left on the field
                try
                {
                    foreach (var cell in Field)
                        cell.Value.FixPosition(new Vector2(Indexes[0].IndexOf(cell.Key[0]), Indexes[1].IndexOf(cell.Key[1])), FieldSize);
                }
                catch
                {
                    Debug.LogError("Failed to update something");
                }
            }
        }

        public virtual bool GeneratePath()
        {
            //Let's make a path!
            //Length of path to travel
            GeneratedPath = new List<Crystal>();

            var currentLevel = (int)Mathf.Round(GlobalData.GetComponent<gameVariables>().GetSetLevelsCompleted * 0.5f) + 1;

            //Generate until not on a void, if infinite loop, all ledges are void...
            {
                //starting edge
                var startSide = Random.Range(0, 3);
                //Get start point for the side, % 2 to see if we go height or width for odd or even
                var startPoint = (int)Random.Range(0, startSide % 2 == 0 ? FieldSize.y : FieldSize.x);
                //0 = right, 1 = top, 2 = left, 3 = up
                var x = startSide % 2 == 0 ? startSide == 0 ? (int)FieldSize.x - 1 : 0 : startPoint;
                var y = startSide % 2 == 0 ? startPoint : startSide == 1 ? 0 : (int)FieldSize.y - 1;
                //Start the path on the correct point
                if (Field[new[] { Indexes[0][x], Indexes[1][y] }].Type != 0)
                    GeneratedPath.Add(Field[new[] { Indexes[0][x], Indexes[1][y] }]);
            }

            //Pick starting direction
            var currentDirection = Random.Range(0, 3);
            var triedDirections = new[] { 0, 0, 0, 0 };

            //Start traveling
            while (currentLevel > 0 && FieldSize.y > 1 && FieldSize.x > 1 && GeneratedPath.Any())
            {
                var currentPosition = GeneratedPath[GeneratedPath.Count - 1].Position;
                var posMove = new Vector2(0, 0);

                //Find if picked direction is good
                switch (currentDirection)
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

                //If no issues with picked direction, move forward, get new direction
                if (CheckMove(currentPosition))
                {
                    GeneratedPath.Add(Field[new[] { Indexes[0][(int)currentPosition.x], Indexes[1][(int)currentPosition.y] }]);
                    currentDirection = GetDirection(currentDirection, triedDirections);
                    triedDirections = new[] { 0, 0, 0, 0 };
                    currentLevel--;
                }
                else
                {
                    //Grab direction, if stuck crash it?
                    currentDirection = GetDirection(currentDirection, triedDirections);
                    if (currentDirection == -1)
                        return false;
                }
            }

            //Finished path
            return true;
        }

        public bool CheckMove(Vector2 posCheck)
        {
            if (posCheck.x < 0 || posCheck.y < 0)
                return false;
            if ((int)posCheck.x >= FieldSize.x || (int)posCheck.y >= FieldSize.y)
                return false;
            if (Field[new[] { Indexes[0][(int)posCheck.x], Indexes[1][(int)posCheck.y] }].Type == 0)
                return false;
            return true;
        }

        //TODO: Implement better system that tries to keep track of direction used over time
        public int GetDirection(int currentDirection, int[] triedDirections)
        {
            //Get list of directions to try
            var dirList = new List<int>();
            for (var i = 0; i < 4; i++)
                if (triedDirections[i] == 0)
                    dirList.Add(i);

            //All directions tried, infinite loop
            if (dirList.Count == 0)
                return -1;

            var newDirection = Random.Range(0, 100);
            //Set directions, 15% backwards, 15% forward, %35 each side
            var picked = -1;
            //while(picked < 0)
            //{
            picked = newDirection < 15 ? 2 : newDirection < 30 ? 0 : newDirection < 65 ? 1 : 3;
            //	if(!dirList.Contains(picked))
            //		picked = -1;
            //}

            return (currentDirection + picked) % 4;
        }

        //Player has moved
        public void DoMove(Crystal usedCrystal)
        {
            //Only move forward only if on a different crystal
            if (Current != usedCrystal && Started)
            {
                //Give player path to follow later
                PlayerPath.Add(usedCrystal);

                //Set color to traveled
                usedCrystal.Traveled();

                //Move color index forward, account for fixed length and roll over
                //usedCrystal.gameObject.GetComponent<crystal>().colorIndex = usedCrystal.gameObject.GetComponent<crystal>().colorIndex + 1 % visitColors.Length;

                //Correct move
                if (GeneratedPath[PlayerProgress] == usedCrystal)
                {
                    Parent.GetComponent<AudioSource>().PlayOneShot(MouseOver);
                    //Particles, temp removed?
                    //generatedPath[playerProgress].renderer.particleSystem.Play();
                    if (PlayerProgress == GeneratedPath.Count - 1)
                    {
                        GlobalData.GetComponent<gameVariables>().GetSetLevelsCompleted += 1;
                        Application.LoadLevel("gameWorld");
                    }
                    Current = usedCrystal;
                    PlayerProgress++;
                    //Bad move
                }
                else
                {
                    //Do gameover, make it fancy later
                    //Application.LoadLevel("mainMenu");
                    AbleToMove = false;
                    Playing = false;
                    //PopUp();
                    GlobalData.GetComponent<gameVariables>().GetSetFailed = true;
                }
            }
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        public virtual void Update()
        {
            //scaleWidth = (float)Screen.width / (float)Screen.height * 0.8f; //0.8 is reverse of 5:4 ratio, 60f is camera default fieldofview
            foreach (var cam in Camera.allCameras)
                cam.aspect = 1;

            foreach (var cell in Field)
                cell.Value.Update();

            if (Input.GetKey(KeyCode.Escape))
            {
                GlobalData.GetComponent<gameVariables>().SaveScore();
                Application.LoadLevel("mainMenu");
                GlobalData.GetComponent<gameVariables>().GetSetLevelsCompleted = 0;
            }

            MousePos.x = 0;
            MousePos.y = 0;
            int mX;
            int mY;
            lock (FieldLock)
            {
                //Handle mouse stuff
                if (Field.Any() && Indexes.All(ind => ind.Any()))
                {

                    Vector2 min = Field[new[] { Indexes[0].FirstOrDefault(), Indexes[1].FirstOrDefault() }].Tesseract.transform.position;
                    Vector2 max = Field[new[] { Indexes[0].LastOrDefault(), Indexes[1].LastOrDefault() }].Tesseract.transform.position;

                    MousePos.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - min.x;
                    MousePos.x = Math.Abs(max.x - min.x) < 0.0001 ? 0 : Mathf.Floor((MousePos.x / (max.x - min.x) * (FieldSize.x - 1.0f)) + 0.5f);

                    MousePos.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - min.y;
                    MousePos.y = Math.Abs(max.y - min.y) < 0.0001 ? 0 : Mathf.Floor((MousePos.y / (max.y - min.y) * (FieldSize.y - 1.0f)) + 0.5f);

                    if (MousePos.x >= 0 && MousePos.x < FieldSize.x && MousePos.y >= 0 && MousePos.y < FieldSize.y)
                        if (Math.Abs(MousePos.x - LastPos.x) > 0.0001 || Math.Abs(MousePos.y - LastPos.y) > 0.0001)
                            Field[new[] { Indexes[0][(int)MousePos.x], Indexes[1][(int)MousePos.y] }].Traveled();
                }

                LastPos.x = MousePos.x + 0.0f;
                LastPos.y = MousePos.y + 0.0f;

                mX = MousePos.x < 0 ? 0 : (int)MousePos.x >= (int)FieldSize.x ? -1 : (int)MousePos.x;
                mY = MousePos.y < 0 ? 0 : (int)MousePos.y >= (int)FieldSize.y ? -1 : (int)MousePos.y;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                if (!AdditionMade)
                {
                    AddCrystals(1, mY, 1);
                    AdditionMade = true;
                }
            }
            else if (Input.GetKey(KeyCode.W))
            {
                if (!AdditionMade)
                {
					AddCrystals(0, mX, 1);
                    AdditionMade = true;
                }
            }
            else if (Input.GetKey(KeyCode.O))
            {
                if (!AdditionMade)
                {
					RemoveCrystals(1, mX, 1);
                    AdditionMade = true;
                }
            }
            else if (Input.GetKey(KeyCode.P))
            {
                if (!AdditionMade)
                {
					RemoveCrystals(0, mX, 1);
                    AdditionMade = true;
                }
            }
            else
            {
                AdditionMade = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                LastPos = new Vector2(-1, -1);
            }

            if (Input.GetKey(KeyCode.A))
            {
				AddCrystals(1, mY, 1);
            }
            else if (Input.GetKey(KeyCode.S))
            {
				AddCrystals(0, mX, 1);
            }
            else if (Input.GetKey(KeyCode.K))
            {
				RemoveCrystals(1, mY, 1);
            }
            else if (Input.GetKey(KeyCode.L))
            {
				RemoveCrystals(0, mX, 1);
            }
        }

        /// <summary>
        /// Changes the texture of the Crystal.
        /// </summary>
        /// <returns>The texture.</returns>
        protected void ChangeTexture(object dunno)
        {
            lock (FieldLock)
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
                if (GeneratedPath.Any())
                {
                    foreach (Crystal crystal in GeneratedPath)
                    {
                        var tempColor = crystal.Tesseract.GetComponent<Renderer>().material.color;
                        crystal.Tesseract.gameObject.GetComponent<Renderer>().material.color = GlobalData.GetComponent<gameVariables>().GetSetAIPathColor;
                        Thread.Sleep((int)Timer);
                        crystal.Tesseract.gameObject.GetComponent<Renderer>().material.color = tempColor;
                    }
                }
                //}


                AbleToMove = true;
            }
        }

        /// <summary>
        /// Sets the variables for difficulties.
        /// </summary>
        protected virtual void SetVariables()
        {
            float timeDecrease = 0;
            var increaseWidth = 0;
            var increaseHeight = 0;
            FieldOffset = new Vector2(3.0f, -0.3f);
            var levels = GlobalData.GetComponent<gameVariables>().GetSetLevelsCompleted;

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
            var lazymansNumber = 6;
            FieldSize.y = lazymansNumber + increaseHeight;
            FieldSize.x = lazymansNumber + increaseWidth;
            Timer = Random.Range(0.5f - timeDecrease, 0.7f - timeDecrease);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="GeneratedPath"/> get able to move.
        /// </summary>
        /// <value><c>true</c> if get able to move; otherwise, <c>false</c>.</value>
        public bool GetAbleToMove
        {
            get { return AbleToMove; }
        }

        public bool GamePlaying
        {
            get { return Playing; }
        }
    }

    public class GuidComparer : IEqualityComparer<Guid[]>
    {
        public bool Equals(Guid[] t1, Guid[] t2)
        {
            try
            {
                //Either one null, don't bother trying
                if (t1 == null || t2 == null)
                    return false;

                //Count difference, done
                if (t1.Count() != t2.Count())
                    return false;

                for (var i = 0; i < t1.Count(); i++)
                {
                    if (!t1[i].Equals(t2[i]))
                        return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public int GetHashCode(Guid[] t1)
        {
            var result = t1[0].GetHashCode();
            for (var i = 1; i < t1.Count(); i++)
                result &= t1[i].GetHashCode();

            return result;
        }
    }
}
