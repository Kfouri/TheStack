using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RankingScript : MonoBehaviour {

	private int rank;
	private static string server = "http://kfouri.onlinewebshop.net/";
	private string TopScoresURL = server+"TopScoreStaking.php";
	private string RankURL = server+"GetRankStaking.php";
	private float waitTime = 2.5f;
	private float elapsedTime = 0.0f;

	//The score and username we submit
	private int highscore;
	private string username;

	//We use this to allow the user to start the game again.
	private bool pressspace;

	public GameObject BaseGUIText;

	GameObject MensajeEsperar;
	GameObject Scoresheader;
	GameObject Rankheader;
	GameObject Nameheader;
	GameObject Scoreheader;


	Vector2 CentreTextPosition = new Vector2(0.33f, 0.33f);

	WWW GetScoresAttempt;
	WWW RankGrabAttempt;

	// Use this for initialization
	void Start () {
	
		//Choose our text positions

		MensajeEsperar = Instantiate(BaseGUIText, CentreTextPosition, Quaternion.identity) as GameObject;
		MensajeEsperar.GetComponent<GUIText>().enabled = true;
		MensajeEsperar.GetComponent<GUIText>().text = "Please wait...";
		MensajeEsperar.GetComponent<GUIText>().fontSize = 35;
		MensajeEsperar.tag = "Esperar";

		StartCoroutine(GrabRank(SystemInfo.deviceUniqueIdentifier,3));


	}

	void OnEnable()
	{
		pressspace = false; // The user can't press space yet.
	}

	void Update()
	{

		elapsedTime += Time.deltaTime;

		if(elapsedTime >= waitTime)
		{
			Destroy(MensajeEsperar);

			if (!GetScoresAttempt.isDone)
			{
				StopCoroutine("GetTopScores");
				GetScoresAttempt.Dispose();
				Error();
			}

			if (!RankGrabAttempt.isDone)
			{
				StopCoroutine("GrabRank");
				RankGrabAttempt.Dispose();
				Error();
			}

		}		
	}

	///Our public access functions
	public void Setscore(int givenscore)
	{
		highscore = givenscore;
	}
	
	public void SetName(string givenname){
		username = givenname;
	}


	IEnumerator GrabRank(string name,int p_dificultad)
	{
		elapsedTime = 0.0f;
		RankGrabAttempt = new WWW(RankURL+"?p_imei=" + WWW.EscapeURL(name)+"&p_dificultad="+p_dificultad);
		string resultado = "";
		char[] delimiterChars = {';'};

		yield return RankGrabAttempt;
		
		if (RankGrabAttempt.error == null)
		{
			Debug.Log("RankGrabAttempt.text: "+RankGrabAttempt.text);
			resultado = RankGrabAttempt.text;

			string[] words = resultado.Split(delimiterChars);
			username = words[0];
			rank = int.Parse (words[1]);

			StartCoroutine(GetTopScores(p_dificultad));
			
		}
		else
		{
			Debug.Log("Error1");
			Error();
		}
	}


	IEnumerator GetTopScores(int p_dificultad)
	{
		elapsedTime = 0.0f;
		GetScoresAttempt = new WWW(TopScoresURL+"?p_dificultad="+p_dificultad);
		yield return GetScoresAttempt;
		
		if (GetScoresAttempt.error != null)
		{
			Debug.Log ("Error2");
			Error();
		}
		else
		{
			Destroy (MensajeEsperar);
			//Collect up all our data

			string[] textlist = GetScoresAttempt.text.Split(new string[] { "\n", "\t" }, System.StringSplitOptions.RemoveEmptyEntries);

			//Split it into two smaller arrays
			string[] Names = new string[Mathf.FloorToInt(textlist.Length/2)];
			string[] Scores = new string[Names.Length];

			for (int i = 0; i < textlist.Length; i++)
			{

				if (i % 2 == 0)
				{     
					Names[Mathf.FloorToInt(i / 2)] = textlist[i];
				}
				else if (i % 2 == 1)
				{
					Scores[Mathf.FloorToInt(i / 2)] = textlist[i];
				}
			}
			
			//Choose our text positions
			Vector2 RankPosition = new Vector2(0.25f,0.85f);
			Vector2 NamePosition = new Vector2(0.53f, 0.85f);
			Vector2 ScorePosition = new Vector2(0.80f, 0.85f);

			string strdif;

			if (p_dificultad == 1) 
			{
				strdif = "Easy";
			} 
			else if (p_dificultad == 2) 
			{
				strdif = "Medium";
			} 
			else 
			{
				strdif = "Hard";
			}

			///All our headers
		    Scoresheader = Instantiate(BaseGUIText, new Vector2(0.5f,0.94f), Quaternion.identity) as GameObject;
			Scoresheader.tag = "Score";
			Scoresheader.GetComponent<GUIText>().text = "High Scores "+strdif;
			Scoresheader.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;
			Scoresheader.GetComponent<GUIText>().fontSize = 40;

			//---------------------------------------------------------------
 			Rankheader = Instantiate(BaseGUIText, RankPosition, Quaternion.identity) as GameObject;
			Rankheader.tag = "Score";
			Rankheader.GetComponent<GUIText>().text = "Rank";
			Rankheader.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;
			Rankheader.GetComponent<GUIText>().fontSize = 35;

			Nameheader = Instantiate(BaseGUIText, NamePosition, Quaternion.identity) as GameObject;
			Nameheader.tag = "Score";
			Nameheader.GetComponent<GUIText>().text = "Name";
			Nameheader.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;
			Nameheader.GetComponent<GUIText>().fontSize = 35;

			Scoreheader = Instantiate(BaseGUIText, ScorePosition, Quaternion.identity) as GameObject;
			Scoreheader.tag = "Score";
			Scoreheader.GetComponent<GUIText>().text = "Score";
			Scoreheader.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;
			Scoreheader.GetComponent<GUIText>().fontSize = 35;


			//Increment the positions
			RankPosition -= new Vector2(0,0.04f);
			ScorePosition -= new Vector2(0,0.04f);
			NamePosition -= new Vector2(0,0.04f);


			///Our top 10 scores
			for(int i=0;i<Names.Length;i++)
			{

				GameObject Rank = Instantiate(BaseGUIText, RankPosition, Quaternion.identity) as GameObject;
				Rank.tag = "Score";
				Rank.GetComponent<GUIText>().text = "" + (i + 1);
				Rank.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;
				Rank.GetComponent<GUIText>().fontSize = 35;

				GameObject Score = Instantiate(BaseGUIText, ScorePosition, Quaternion.identity) as GameObject;
				Score.tag = "Score";
				Score.GetComponent<GUIText>().text = Scores[i];
				Score.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;
				Score.GetComponent<GUIText>().fontSize = 35;

				GameObject Name = Instantiate(BaseGUIText, NamePosition, Quaternion.identity) as GameObject;
				Name.tag = "Score";
				Name.GetComponent<GUIText>().text = Names[i];
				Name.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;
				Name.GetComponent<GUIText>().fontSize = 35;

				if (i + 1 == rank) //If the player is within the top 10 colour their score yellow.
				{
					Score.GetComponent<GUIText>().material.color = Color.yellow;
					Name.GetComponent<GUIText>().material.color = Color.yellow;
					Rank.GetComponent<GUIText>().material.color = Color.yellow;
				}
				
				//Increment the positions again
				RankPosition -= new Vector2(0,0.04f);
				ScorePosition -= new Vector2(0,0.04f);
				NamePosition -= new Vector2(0,0.04f);
			}

			//If our player isn't in the top 10, add them to the bottom.
			if (rank > 10)
			{

				GameObject Rank = Instantiate(BaseGUIText, RankPosition, Quaternion.identity) as GameObject;
				Rank.tag = "Score";
				Rank.GetComponent<GUIText>().text = "" + (rank);
				Rank.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;

				GameObject Name = Instantiate(BaseGUIText, NamePosition, Quaternion.identity) as GameObject;
				Name.tag = "Score";
				Name.GetComponent<GUIText>().text = username;

				GameObject Score = Instantiate(BaseGUIText, ScorePosition, Quaternion.identity) as GameObject;
				Score.tag = "Score";
				Score.GetComponent<GUIText>().text = ""+PlayerPrefs.GetInt("highscore", 0);
				Score.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;

				Score.GetComponent<GUIText>().material.color = Color.yellow;
				Name.GetComponent<GUIText>().material.color = Color.yellow;
				Rank.GetComponent<GUIText>().material.color = Color.yellow;
			}

			//Allows the user to restart the game
			pressspace = true;
		}
	}

	void Error()
	{
		Destroy (MensajeEsperar);
		//Choose our text positions
		Vector2 CentreTextPosition = new Vector2(0.33f, 0.33f);

		GameObject Scoreheader = Instantiate(BaseGUIText, CentreTextPosition, Quaternion.identity) as GameObject;
		Scoreheader.GetComponent<GUIText>().enabled = true;
		Scoreheader.GetComponent<GUIText>().text = "Connection error. Try later";
		Scoreheader.GetComponent<GUIText>().fontSize = 35;
		pressspace = true;
	}

	public void Botonera(int p_dificultad)
	{
		GameObject[] gameObjects;

		MensajeEsperar = Instantiate(BaseGUIText, CentreTextPosition, Quaternion.identity) as GameObject;
		MensajeEsperar.GetComponent<GUIText>().enabled = true;
		MensajeEsperar.tag = "Esperar";
		MensajeEsperar.GetComponent<GUIText>().text = "Please wait...";
		MensajeEsperar.GetComponent<GUIText>().fontSize = 35;


		gameObjects = GameObject.FindGameObjectsWithTag ("Score");

		for (int i = 0; i < gameObjects.Length; i++) 
		{
			Destroy (gameObjects [i]);
		}


		StartCoroutine(GrabRank(SystemInfo.deviceUniqueIdentifier,p_dificultad));
	}

	public void BotonBack()
	{
		SceneManager.LoadScene("menu");
	}
}
