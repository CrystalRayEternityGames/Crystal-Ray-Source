using UnityEngine;
using System.Collections;

public class cameraShinanigans : MonoBehaviour {

	private float _theZEffect;
	private readonly float distance = 20.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		var pos = transform.localPosition;
		transform.localPosition = new Vector3 (pos.x, pos.y, pos.z - Mathf.Abs (_theZEffect - distance));
		_theZEffect += (distance / 100.0f);
		_theZEffect %= (distance * 2.0f);
		pos = transform.localPosition;
		transform.localPosition = new Vector3 (pos.x, pos.y, pos.z + Mathf.Abs (_theZEffect - distance));
	}
}
