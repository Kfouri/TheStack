using UnityEngine;
using System.Collections;

public class Remove : MonoBehaviour {
	
	private void OnCollisionEnter(Collision col)
	{
		Destroy (col.gameObject);
	}
}