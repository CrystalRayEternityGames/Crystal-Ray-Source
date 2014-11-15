using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Tutorial at http://unity3d.com/learn/tutorials/modules/beginner/live-training-archive/scriptable-objects
/// Help store serializable arrays of materials and models to change on the fly
/// </summary>
[System.Serializable]
public class AssetManager : ScriptableObject
{

    static readonly GameObject[] MODELS = Resources.LoadAll<GameObject>(@"Crystals/Prefabs") as GameObject[];
    static readonly Material[] MATERIALS = Resources.LoadAll<Material>(@"Crystals/Materials") as Material[];

    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    //Hmm... http://www.dotnetperls.com/multiple-return-values
    /*public static KeyValuePair<GameObject[], Material[]> GetModelAndMaterial
    {
        get 
        {
            return new KeyValuePair<GameObject[], Material[]>();
        }
    }*/

    /// <summary>
    /// Obtain any specific model in array cache
    /// </summary>
    public static GameObject[] GetModels
    {
        get
        {
            return MODELS;
        }
    }

    /// <summary>
    /// Obtain any specific material in array Cache
    /// </summary>
    public static Material[] GetMaterials
    {
        get
        {
            return MATERIALS;
        }
    }

}
