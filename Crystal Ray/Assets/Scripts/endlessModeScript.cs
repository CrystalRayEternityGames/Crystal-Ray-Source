using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

namespace Assets.Scripts
{
    public class EndlessModeScript : GameEngine
    {
        #region Fields

        //private static endlessModeScript instance = null;

        #endregion

        protected override void SetVariables()
        {
            base.SetVariables();
			float timeDecrease = Random.Range(0.07f, 0.1f);
			FieldSize.y = 10;
			FieldSize.x = 10;
			Timer = Random.Range(0.5f - timeDecrease, 0.7f - timeDecrease);
        }

        private void ClearColor()
        {
            for (int i = 0; i < GeneratedPath.Count - 1; i++)
            {
                GeneratedPath[i].Type = 0;
                //generatedPath[i].renderer.material.color = Color.cyan;
                Object.Destroy(GeneratedPath[i].Tesseract.GetComponent<ParticleSystem>());
            }

            PlayerPath.ForEach(crst =>
            {

                crst.Type = 1;
                //crst.renderer.material.color = Color.cyan;
                Object.Destroy(crst.Tesseract.GetComponent<ParticleSystem>());
            });

			GeneratePath ();
        }

        public void DoStart()
        {
            Started = true;
        }

        public EndlessModeScript(gameMain parent, GameObject globals) : base(parent, globals)
        {
        }
    }
}
