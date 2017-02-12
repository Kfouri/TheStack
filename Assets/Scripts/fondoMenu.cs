using UnityEngine;
using System.Collections;

public class fondoMenu : MonoBehaviour {
	
	private void OnCollisionEnter(Collision col)
	{
		Destroy (col.gameObject);
	}
}