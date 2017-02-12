using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheStack : MonoBehaviour {

	public Text scoreText; 
	public Text textLevel;
	public Text textCombo;

	public GameObject endPanel;

	public Color32[] gameColors = new Color32[4];
	public Material stackMat;
	//1:38:28
	private const float BOUND_SIZE = 3.5f;
	private const float STACK_MOVING_SPEED = 5.0f;
	private const float ERROR_MARGIN = 0.15f;
	private const float STACK_BOUNDS_GAIN = 0.25f;
	private const int COMBO_START_GAIN = 3;

	private Vector2 stackBounds = new Vector2(BOUND_SIZE,BOUND_SIZE);

	private int combo = 0;

	private GameObject[] theStack;

	private int stackIndex;
	private int scoreCount = 0;
	private bool gameOver = false;

	private float tileTransition = 0.0f;
	private float tileSpeed = 2.5f;

	private bool isMovingOnX = true;
	private float secondaryPosition;

	private Vector3 desiredPosition;

	private Vector3 lastTilePosition;

	// Use this for initialization
	void Start () 
	{
		
		textLevel.text = ApplicationModel.level;

		if (ApplicationModel.level == "Easy") 
		{
			tileSpeed = 3f;
		} 
		else if (ApplicationModel.level == "Medium") 
		{
			tileSpeed = 4f;
		} 
		else 
		{
			tileSpeed = 6f;
		}

		theStack = new GameObject[transform.childCount];
		for (int i=0; i<transform.childCount; i++) 
		{
			theStack [i] = transform.GetChild (i).gameObject;
			ColorMesh(theStack [i].GetComponent<MeshFilter>().mesh);
		}

		stackIndex = transform.childCount - 1;
	}

	void createRubble(Vector3 pos, Vector3 scale)
	{
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		go.AddComponent<Rigidbody> ();
		go.GetComponent<MeshRenderer> ().material = stackMat;
		ColorMesh(go.GetComponent<MeshFilter>().mesh);
	}


	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (PlaceTile())
			{
			   SpawnTile();
			   scoreCount++;
				scoreText.text = scoreCount.ToString();
			}
			else
			{
				EndGame();
			}
		}

		MoveTile ();

		//move the stack
		transform.position = Vector3.Lerp (transform.position, desiredPosition, STACK_MOVING_SPEED*Time.deltaTime);
	}

	void MoveTile()
	{
		if (gameOver) 
		{
			return;
		}

		tileTransition += Time.deltaTime * tileSpeed;

		if (isMovingOnX) {
			theStack [stackIndex].transform.localPosition = new Vector3 (Mathf.Sin (tileTransition) * BOUND_SIZE, scoreCount, secondaryPosition);
		} else {
			theStack [stackIndex].transform.localPosition = new Vector3 (secondaryPosition, scoreCount, Mathf.Sin (tileTransition) * BOUND_SIZE);

		}
	}

	void SpawnTile()
	{
		lastTilePosition = theStack [stackIndex].transform.localPosition;
		stackIndex--;
		if (stackIndex < 0)
			stackIndex = transform.childCount - 1;

		desiredPosition = (Vector3.down) * scoreCount;
		theStack[stackIndex].transform.localPosition = new Vector3(0,scoreCount,0);
		theStack[stackIndex].transform.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);

		ColorMesh(theStack[stackIndex].GetComponent<MeshFilter>().mesh);

	}

	void ColorMesh(Mesh mesh)
	{
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];

		float f = Mathf.Sin (scoreCount * 0.25f);

		for (int i = 0; i<vertices.Length; i++) 
		{
			colors[i] =  Lerp4(gameColors[0],gameColors[1],gameColors[2],gameColors[3],f);
		}

		mesh.colors32 = colors;
	}

	bool PlaceTile()
	{
		Transform t = theStack [stackIndex].transform;

		if (isMovingOnX) 
		{
			float deltaX = lastTilePosition.x - t.position.x;

			if (Mathf.Abs (deltaX) > ERROR_MARGIN) 
			{
				//cut the tile
				combo = 0;
				textCombo.text = "Combo 0";
				stackBounds.x -= Mathf.Abs (deltaX);

				if (stackBounds.x <= 0)
				{
					stackBounds.x += Mathf.Abs (deltaX);
					return false;
				}

				float middle = lastTilePosition.x + t.position.x / 2;
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);

				createRubble(new Vector3((t.position.x>0)
				                         ?t.position.x + (t.localScale.x/2)
				                         :t.position.x - (t.localScale.x/2),t.position.y,t.position.z),
				             new Vector3(Mathf.Abs(deltaX),1,t.localScale.z));

				t.localPosition = new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
			}
			else
			{
				if (combo>COMBO_START_GAIN)
				{
					stackBounds.x += STACK_BOUNDS_GAIN;
					if (stackBounds.x > BOUND_SIZE)
					{
						stackBounds.x = BOUND_SIZE;
					}
					float middle = lastTilePosition.x + t.position.x / 2;
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					t.localPosition = new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
				}
				combo++;
				textCombo.text = "Combo "+combo.ToString();
				t.localPosition = new Vector3(lastTilePosition.x,scoreCount,lastTilePosition.z);
			}
		} 
		else 
		{
			float deltaZ = lastTilePosition.z - t.position.z;
			if (Mathf.Abs (deltaZ) > ERROR_MARGIN) 
			{
				//cut the tile
				combo = 0;
				textCombo.text = "Combo 0";
				stackBounds.y -= Mathf.Abs (deltaZ);
				if (stackBounds.y <= 0)
				{
					stackBounds.y += Mathf.Abs (deltaZ);
					return false;
				}

				float middle = lastTilePosition.z + t.position.z / 2;
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);

				createRubble(new Vector3(t.position.x,
				                         t.position.y, 
				                         (t.position.z>0)
				                         ?t.position.z + (t.localScale.z/2)
				                         :t.position.z - (t.localScale.z/2)),
				             new Vector3(t.localScale.x,1,Mathf.Abs(deltaZ)));

				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, middle - (lastTilePosition.z/2));
			}
			else
			{
				if (combo>COMBO_START_GAIN)
				{
					stackBounds.y += STACK_BOUNDS_GAIN;

					if (stackBounds.y > BOUND_SIZE)
					{
						stackBounds.y = BOUND_SIZE;
					}

					float middle = lastTilePosition.z + t.position.z / 2;
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, middle - (lastTilePosition.z/2));
				}
				combo++;
				textCombo.text = "Combo "+combo.ToString();
				t.localPosition = new Vector3(lastTilePosition.x,scoreCount,lastTilePosition.z);
			}
		}

		secondaryPosition = (isMovingOnX)
			? t.localPosition.x
				: t.localPosition.z;

		isMovingOnX = !isMovingOnX;

		return true;
	}

	Color32 Lerp4(Color32 a,Color32 b,Color32 c,Color32 d,float t)
	{
		if (t < 0.33f) 
		{
			return Color.Lerp (a, b, t / 0.33f);
		} 
		else if (t < 0.66f) 
		{
			return Color.Lerp (b, c, (t - 0.33f) / 0.33f);
		} 
		else 
		{
			return Color.Lerp (c,d,(t-0.66f)/0.66f);
		}
	}

	void EndGame()
	{
		int nivel;

		if (ApplicationModel.level == "Easy") 
		{
			nivel = 0;
			if (PlayerPrefs.GetInt ("scoreEasy") < scoreCount) 
			{
				PlayerPrefs.SetInt("scoreEasy",scoreCount);
				StartCoroutine (subirInternet (nivel,scoreCount));
			}		
		} 
		else if (ApplicationModel.level == "Medium") 
		{
			nivel = 1;
			if (PlayerPrefs.GetInt ("scoreMedium") < scoreCount) 
			{
				PlayerPrefs.SetInt("scoreMedium",scoreCount);
				StartCoroutine (subirInternet (nivel,scoreCount));
			}
		} 
		else 
		{
			nivel = 2;
			if (PlayerPrefs.GetInt ("scoreHard") < scoreCount) 
			{
				PlayerPrefs.SetInt("scoreHard",scoreCount);
				StartCoroutine (subirInternet (nivel,scoreCount));
			}
		}

		gameOver = true;
		endPanel.SetActive (true);
		//theStack [stackIndex].AddComponent<Rigidbody> ();
	}

	public void OnButtonClick(string sceneName)
	{

		Debug.Log (sceneName);
        if (sceneName == "Game")
        {
            SceneManager.LoadScene("TheStack");

        }
        else if (sceneName == "Menu")
        {
            SceneManager.LoadScene("menu");
        }
        else
        {
            Debug.Log("Boton Continuar con Ads");
            gameOver = false;
            endPanel.SetActive(false);
            mostrarAd();
        }
    }

    private void mostrarAd()
    {
        if (Advertisement.IsReady())
        {
            Debug.Log("showAds() despues");
            Advertisement.Show("rewardedVideo", new ShowOptions()
            {
                resultCallback = HandleAdResult
            });
        }
    }

    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Finish ...");
                gameOver = false;
                endPanel.SetActive(false);
                break;
            case ShowResult.Failed:
                Debug.Log("Failed ...");
                break;
            case ShowResult.Skipped:
                Debug.Log("Skipped ...");
                break;

        }
    }

	IEnumerator  subirInternet(int p_nivel, int p_puntos)
	{

		Debug.Log ("SubirInternet");
		string url = "http://kfouri.onlinewebshop.net/nuevo_record_staking.php";


		WWWForm form = new WWWForm();
		form.AddField ( "p_imei", SystemInfo.deviceUniqueIdentifier);
		form.AddField ( "p_puntos", p_puntos);
		form.AddField ( "p_nombre", PlayerPrefs.GetString("nombreArcade"));
		form.AddField ( "p_dificultad", p_nivel); 
		form.AddField ( "p_tiempo", 0);


		if (ApplicationModel.level == "Easy") 
		{
			form.AddField ("p_dificultad", 1);
		} 
		else if (ApplicationModel.level == "Medium") 
		{
			form.AddField ("p_dificultad", 2);
		} 
		else 
		{
			form.AddField ("p_dificultad", 3);
		}



		WWW download = new WWW( url, form );
		yield return download;

		if((!string.IsNullOrEmpty(download.error)))
		{
			//print( "Error downloading: " + download.error );
			Debug.Log ("Error: "+download.error);
		} 
		else 
		{
			//formText = download.text;
			Debug.Log ("OK");
		}

	}
}