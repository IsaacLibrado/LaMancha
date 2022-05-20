using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Clase para controlar todo el juego
/// </summary>
/// Version 1.0
/// Fecha de creacion 19/05/22
public class GameController : MonoBehaviour {

	//Referencia a los participantes
	public Personaje[] personajes;
	public MovJugador jugador;

	//Para controlar el tiempo
	public float timer;

	//Para reproducir sonido
	public AudioSource musica;

	//Para controlar la cantidad de manchas a colocar
	private int manchas;
	private int ronda;

	//Referencia a la cantidad de participantes enemigos activos
	public int vs;

	//Color para las manchas
	public Material color;
	
	//Referencias a los UI
	public GameObject manchado;
	public GameObject canvasFinal;
	public GameObject canvasUI;

	//Para los textos del UI
	public TMP_Text restantes;
	public TMP_Text tiempo;
	
	//Para acabar con el juego
	public bool acabado;

	// Use this for initialization
	void Start () {
		//Obtenemos todos los personajes
		personajes = FindObjectsOfType<Personaje>();

		//Inicializamos las variables
		timer = 150f;

		manchas = 10;
		ronda = 0;
		
		vs = 0;
		
		//Iniciamos las manchas
		Reinicio();

		QualitySettings.vSyncCount = 1;
		Application.targetFrameRate = 60;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}

	// Update is called once per frame
	void Update() {
		if(!acabado)
        {
			Tiempo();
			
			ValidarMancha();

		}

	}
	
	/// <summary>
	/// Metodo para controlar el tiempo del juego
	/// </summary>
	/// Version 1.0
	/// Fecha de creacion 19/05/22
	void Tiempo()
    {
		//Colocamos el color negro al texto
		tiempo.faceColor = Color.black;

		//Reducimos el tiempo cada frame
		timer -= 1 * Time.deltaTime;

		
		//Divimos el tiempo entre minutos
		int minutos = (int)timer / 60;

		//Obtenemos los segundos de acuerdo a la cantidad de minutos
		int segundos;

		if (minutos == 2)
		{
			segundos = (int)(timer - 120f);
		}
		else if (minutos == 1)
		{
			segundos = (int)(timer - 60f);
		}
		else
        {
			//Colocamos el texto en rojo cuando queda menos de un minuto
			tiempo.faceColor = Color.red;
			segundos = (int)timer;
		}
			
		//Colocamos el texto del tiempo
		tiempo.text = "0" + minutos.ToString();

		if (segundos<10)
        {
			tiempo.text +=  ":0" + segundos;
		}
		else
        {
			tiempo.text += ":" + segundos;
		}

		//Si el tiempo se acaba se reinicia
		if (timer <= 0.9)
		{
			Reinicio();
		}

	}

	/// <summary>
	/// Metodo para mostrar el UI de manchado dependiendo el estado del jugador
	/// </summary>
	/// Version 1.0
	/// Fecha de creacion 19/05/22
	void ValidarMancha()
    {
		if(jugador.IsManchado())
        {
			manchado.SetActive(true);
        }
		else
			manchado.SetActive(false);
	}

	/// <summary>
	/// Metodo para reiniciar cada ronda
	/// </summary>
	/// Version 1.0
	/// Fecha de creacion 19/05/22
	void Reinicio()
    {
		//Colocamos la cantidad de enemigos en 0
		vs = 0;

		//Desactivamos los que estaban manchados y contamos los que no
		foreach (Personaje personaje in personajes)
		{
			if (personaje.IsManchado())
			{
				personaje.gameObject.SetActive(false);
			}
			else
			{
				vs++;
			}
		}

		restantes.text = vs + " restantes";

		//Condiciones para terminar el juego
		if (jugador.IsManchado() || vs <= 0)
		{
			Final();
		}

		//Reiniciamos el tiempo y la musica
		timer = 15f;
		musica.Play();

		//Aumentamos la ronda y obtenemos las nuevas manchas
		ronda++;
		manchas = 5+ronda;

		

		//Posibilidad de 25% para manchar al jugador al inicio y si las manchas no sobrepasan la cantidad de enemigos
		if (Random.Range(0,4)==1 && vs>=manchas)
        {
			manchas--;

			jugador.Manchar(color);
        }

		
		//Manchamos a los personajes que esten activos
		foreach (Personaje personaje in personajes)
        {
			if (manchas > 0 && personaje.gameObject.activeInHierarchy)
			{
				personaje.Manchar(color);
				manchas--;
			}
        }

		
	}

	/// <summary>
	/// Metodo para terminar el juego
	/// </summary>
	/// Version 1.0
	/// Fecha de creacion 19/05/22
	void Final()
    {
		//Acabamos el juego
		acabado = true;

		//Mostramos el UI correspondiente
		canvasFinal.SetActive(true);
		canvasUI.SetActive(false);
		manchado.SetActive(true);

        //Cerramos el juego
        Application.Quit();
    }
}
