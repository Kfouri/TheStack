using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Choose : MonoBehaviour {

   public void Back()
	{
        SceneManager.LoadScene("menu");
	}

	public void OnPressButton(string nivel)
	{
        ApplicationModel.level = nivel;
        SceneManager.LoadScene("TheStack");
	}
}
