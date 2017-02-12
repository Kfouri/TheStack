using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {


	public Text scoreText;
	public Text scoreEasy;
	public Text scoreMedium;
	public Text scoreHard;

	public void Start()
	{
		/*
		PlayerPrefs.SetInt ("scoreEasy", 0);
		PlayerPrefs.SetInt ("scoreMedium", 0);
		PlayerPrefs.SetInt ("scoreHard", 0);
        */
		//PlayerPrefs.SetString ("nombreArcade", "");

		scoreEasy.text = PlayerPrefs.GetInt ("scoreEasy").ToString();
		scoreMedium.text = PlayerPrefs.GetInt ("scoreMedium").ToString();
		scoreHard.text = PlayerPrefs.GetInt ("scoreHard").ToString();
	}

    public void ToGame()
	{

		if (PlayerPrefs.GetString ("nombreArcade").Equals ("")) 
		{
			SceneManager.LoadScene("nombreArcade");
		} 
		else
		{
			SceneManager.LoadScene("chooseLevel");	
		}
    }

	public void Ranking()
	{
		SceneManager.LoadScene("ranking");
	}
}
