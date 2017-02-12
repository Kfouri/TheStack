using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ControlBotones : MonoBehaviour {

	private char[] letras;
	public Text PrimerLetra; 
	public Text SegundaLetra; 
	public Text TercerLetra; 
	private int i = 0;
	private int j = 0;
	private int k = 0;

	// Use this for initialization
	void Start () {
		letras = new char[36];

		letras [0] = 'A';
		letras [1] = 'B';
		letras [2] = 'C';
		letras [3] = 'D';
		letras [4] = 'E';
		letras [5] = 'F';
		letras [6] = 'G';
		letras [7] = 'H';
		letras [8] = 'I';
		letras [9] = 'J';
		letras [10] = 'K';
		letras [11] = 'L';
		letras [12] = 'M';
		letras [13] = 'N';
		letras [14] = 'O';
		letras [15] = 'P';
		letras [16] = 'Q';
		letras [17] = 'R';
		letras [18] = 'S';
		letras [19] = 'T';
		letras [20] = 'U';
		letras [21] = 'V';
		letras [22] = 'W';
		letras [23] = 'X';
		letras [24] = 'Y';
		letras [25] = 'Z';
		letras [26] = '0';
		letras [27] = '1';
		letras [28] = '2';
		letras [29] = '3';
		letras [30] = '4';
		letras [31] = '5';
		letras [32] = '6';
		letras [33] = '7';
		letras [34] = '8';
		letras [35] = '9';
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	public void SubirLetra(int pos)
	{
		if (pos == 0) 
		{
			i = i + 1;
			if (i > 35) 
			{
				i = 0;
			}

			PrimerLetra.text = letras [i].ToString();
		}

		if (pos == 1) 
		{
			j = j + 1;
			if (j > 35) 
			{
				j = 0;
			}

			SegundaLetra.text = letras [j].ToString();
		}


		if (pos == 2) 
		{
			k = k + 1;
			if (k > 35) 
			{
				k = 0;
			}

			TercerLetra.text = letras [k].ToString();
		}




	}

	public void BajarLetra(int pos)
	{

		if (pos == 0) 
		{
			i = i - 1;

			if (i < 0) 
			{
				i = 35;
			}

			PrimerLetra.text = letras [i].ToString ();
		}

		if (pos == 1) 
		{
			j = j - 1;
			if (j < 0) 
			{
				j = 35;
			}

			SegundaLetra.text = letras [j].ToString();
		}


		if (pos == 2) 
		{
			k = k - 1;
			if (k < 0) 
			{
				k = 35;
			}

			TercerLetra.text = letras [k].ToString();
		}

	}

	public void ready()
	{
		PlayerPrefs.SetString("nombreArcade",PrimerLetra.text+SegundaLetra.text+TercerLetra.text);
		SceneManager.LoadScene("chooseLevel");	
	}


}
