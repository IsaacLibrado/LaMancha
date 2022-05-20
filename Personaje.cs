using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase para controlar a los personajes
/// </summary>
public class Personaje : Participante {

    //Para el nuevo color de los manchados y cuando se desmanchan
    public Material color;
    public Material colorN;

    //referencia a los otros personajes 
    private Participante[] personajes;
    private Participante personaje;
    private Participante jugador;
    
    //Referencia al game controller
    public GameController GameController;

    //Valores para los conjuntos difusos
    private float distancia = 0;
    public float tiempo = 0;
    public float mancha=0;

    // Conjunto difuso de la distancia
    private float cerca = 0;
    private float media = 0;
    private float lejos = 0;

    // Conjunto difuso del tiempo
    private float poco = 0;
    private float medio = 0;
    private float mucho = 0;

    //Para la mancha
    private float manchado = 0;

    // Conjunto de las reglas difusas
    private float gradoPerseguir = 0;
    private float gradoHuir = 0;

    //Para la desfuzzificacion
    private int accion = 0;
    private float salida = 0;

 
	// Use this for initialization
	void Start () {
        personajes = FindObjectsOfType<Participante>();
    }
	
	// Update is called once per frame
	void Update () {

        if(!GameController.acabado)
        {
            //raycast para validar lo que vamos a interactuar
            RaycastHit hit;

            //lanzamos el raycast para detectar los objetos
            if (Physics.Raycast(transform.position, transform.forward, out hit, 2.5f))
            {
                if (hit.collider.tag != "Untagged")
                {
                    GameObject personaje = null;
                    bool isManchado = true;

                    if (hit.collider.tag == "Personaje")
                    {
                        //Obtenemos al personaje y su estado de manchado
                        personaje = hit.collider.gameObject;
                        isManchado = personaje.GetComponent<Personaje>().IsManchado();

                        //Si no esta manchado y nosotros si le asignamos el valor
                        if (!isManchado && Manchado)
                        {
                            personaje.GetComponent<MeshRenderer>().material = color;
                            personaje.GetComponent<Personaje>().Manchar(color);
                            Desmanchar();
                        }
                        jugador = null;

                    }
                    if (hit.collider.tag == "Player")
                    {
                        //Obtenemos al personaje y su estado de manchado
                        personaje = hit.collider.gameObject;
                        isManchado = personaje.GetComponent<MovJugador>().IsManchado();

                        //Si no esta manchado y nosotros si le asignamos el valor
                        if (!isManchado && Manchado)
                        {
                            personaje.GetComponent<MeshRenderer>().material = color;
                            personaje.GetComponent<MovJugador>().Manchar(color);
                            Desmanchar();
                        }
                        jugador = null;

                    }
                }
            }

            Fuzzy();
        }
        
    }

    /// <summary>
    /// Metodo para manchar al personaje
    /// </summary>
    /// <param name="pColor">El color de la mancha</param>
    public void Manchar(Material pColor)
    {
        //Indicamos que estamos manchados
        Manchado = true; 
        mancha = 1f;

        //Le colocamos el color
        gameObject.GetComponent<MeshRenderer>().material = pColor;
       
        //Para que tome un nuevo objetivo
        jugador = null;
    }

    /// <summary>
    /// Metodo para desmanchar al personaje
    /// </summary>
    public void Desmanchar()
    {
        //Desmanchamos
        Manchado = false;
        mancha = 0f;

        gameObject.GetComponent<MeshRenderer>().material = colorN;
        
        jugador = null;
    }

    // Para el algoritmo de fuzzificacion
    void Fuzzy()
    {
        //Si no tenemos jugador objetivo realizamos el proceso
        if(jugador==null)
        {
            do
            {
                //Obtenemos un personaje random
                personaje = personajes[Random.Range(0, personajes.Length)];

                //Si el personaje está activo
                if (personaje.gameObject.activeInHierarchy)
                {
                    //Si estamos manchados buscamos a alguien no manchado, y visceversa
                    if (Manchado)
                    {
                        if (!personaje.IsManchado())
                            jugador = personaje;
                    }
                    else
                    {
                        if (personaje.IsManchado())
                            jugador = personaje;
                    }

                }
            } while (jugador == null); //Se realiza el proceso hasta que tengamos un jugador objetivo
        }
        


        // Obtenemos la distancia al jugador objetivo
        distancia = Vector3.Magnitude(transform.position - jugador.gameObject.transform.position);

        // Obtenemos el tiempo
        tiempo = GameController.timer;

        // Fuzzificamos las entradas

        // Relacionadas con distancia
        cerca = FMembresia.GradoInversa(distancia, 0, 30);
        media = FMembresia.Trapezoide(distancia, 20, 50, 80, 100);
        lejos = FMembresia.Grado(distancia, 90, 120);


        // Relacionadas con tiempo
        poco = FMembresia.GradoInversa(tiempo, 0, 60);
        medio = FMembresia.Trapezoide(tiempo, 30, 90, 100, 120);
        mucho = FMembresia.Grado(tiempo, 100, 150);

        manchado = FMembresia.Booleana(mancha, 1f);

        // Reglas difusas
        gradoPerseguir = 0;
        gradoHuir = 0;

        // Persigue cuando (poco || medio)  && (manchado)
        gradoPerseguir = OperadorF.AND(OperadorF.OR(poco, medio), manchado);

        // Huye cuando (media || cerca) && (!manchado)
        gradoHuir = OperadorF.AND(OperadorF.OR(media, cerca),
                                   OperadorF.FuzzyNOT(manchado));


        // Decidimos que accion tomar
        if (gradoPerseguir > gradoHuir)
            accion = 0;
        else
            accion = 1;


        // Defuzzificamos

        salida = (gradoPerseguir * 10 + gradoHuir * -20) / (gradoPerseguir + gradoHuir+1);

        if (salida < 0)
            salida = -1;

        if (accion == 0)// Perseguir
        {
            transform.LookAt(jugador.gameObject.transform.position);
            transform.Translate(Vector3.forward * salida * 2 * Time.deltaTime);
        }
        if (accion == 1)// huir
        {
            transform.LookAt(jugador.gameObject.transform.position);
            transform.Translate(Vector3.forward * salida * Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, 1.02f, transform.position.z);
    }

}
