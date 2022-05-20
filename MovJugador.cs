using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Clase para controlar el movimiento del jugador
/// </summary>
/// Version 1.0
/// Fecha de creacion 18/05/22
public class MovJugador : Participante {

    public float nivel = 100;

    public Vector3 posicion;

    //Referenciamos al componente de control del jugador
    public CharacterController controller;

    //Guardamos la velocidad que tendrá el jugador
    public float velocidad = 12f;

    //Guardamos la fuerza de gravedad que ejercerá sobre el jugador al caer
    public float gravedad = -9.81f;

    //Guardamos el impulso de salto del jugador
    public float salto = 3f;

    //Referenciamos la posición de una esfera que con su collider nos indicará cuando nos encontremos sobre el suelo
    public Transform detectarSuelo;

    //Guardamos la distancia del suelo a la que tiene que estar el jugador para su detección
    public float distanciaSuelo = 0.4f;

    //Guardamos la máscara del suelo
    //Necesaria para determinar si nos encontramos sobre una superficie de tipo suelo
    public LayerMask mascaraSuelo;

    //Vector que determinará el movimiento horizontal del jugador
    Vector3 vectorMoverse;

    //Vector que determinará la velocidad de caida del jugador
    Vector3 vectorCaida;

    //Validamos si el jugador está pisando el suelo o no
    private bool sobreSuelo;

    //Referenciamos al label que nos indica una interacción con algún elemento de la escena
    public TMP_Text lTomar;

    //Para el nuevo color de los manchados
    public Material color;

    //Referencia al game controller
    public GameController gc;

    void Update()
    {
        if(!gc.acabado)
        {
            MoverJugador();
            Interactuar();
            posicion = transform.position;
        }
        
    }

    /// <summary>
    /// Metodo para interactuar con otros personajes
    /// </summary>
    /// Version 1.0
    /// Fecha de creacion 18/05/22

    void Interactuar()
    {
        //raycast para validar lo que vamos a interactuar
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //lanzamos el raycast para detectar los objetos
        if (Physics.Raycast(ray, out hit, 2.5f))
        {
            if (hit.collider.tag != "Untagged")
            {
                //Activamos el texto
                lTomar.gameObject.SetActive(true);

                GameObject personaje;
                bool isManchado=true;

                if (hit.collider.tag == "Personaje")
                {
                    //Obtenemos al personaje y su estado de manchado
                    personaje = hit.collider.gameObject;
                    isManchado = personaje.GetComponent<Personaje>().IsManchado();

                    //Activamos o desactivamos el texto
                    if(!isManchado && Manchado)
                    {
                        lTomar.text = "Manchar";
                    }
                    else
                    {
                        lTomar.gameObject.SetActive(false);
                    }
                }
                else
                {
                    lTomar.gameObject.SetActive(false);
                    personaje = null;
                }
                    

                //validamos que el jugador presiona la tecla de interacción
                if (Input.GetButtonDown("Interact"))
                {
                    //validamos el tipo de objeto
                    if (personaje)
                    {
                        //Si no esta manchado y nosotros si le asignamos el valor
                        if (!isManchado && Manchado)
                        {
                            personaje.GetComponent<MeshRenderer>().material = color;
                            personaje.GetComponent<Personaje>().Manchar(color);
                            Manchado = false;
                        }
                    }
                }
            }
            else
                lTomar.gameObject.SetActive(false);
        }
        else
            lTomar.gameObject.SetActive(false);
    }

    //Movemos al jugador
    /// Version 1.0
    /// Fecha de creacion 18/05/22
    private void MoverJugador()
    {
        //Guardamos en una variable si el jugador está sobre el suelo o no
        sobreSuelo = Physics.CheckSphere(detectarSuelo.position, distanciaSuelo, mascaraSuelo);

        //Si estamos sobre el suelo y la velocidad de caida es menor a 0, mantenemos la velocidad de caida en -2f
        //Impedimos que la velocidad de caida crezca sin control
        if (sobreSuelo && vectorCaida.y < 0)
            vectorCaida.y = -2f;

        //Obtenemos la entrada de movimiento horizontal y vertical (Teclas WASD)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Establecemos el movimiento que se verá reflejado en el jugador
        vectorMoverse = transform.right * x + transform.forward * z;

        ////Al pulsar el botón "Jump" y nos encontramos sobre el suelo, agregamos una fuerza al salto del jugador
        //if (Input.GetButtonDown("Jump") && sobreSuelo)
        //{
        //    //Agregamos una fuerza de salto
        //    vectorCaida.y = Mathf.Sqrt(salto * -2f * gravedad);
        //}

        //Al pulsar el botón "Run" y nos encontramos en movimiento, incrementamos la velocidad de desplazamiento horizontal
        if (Input.GetButton("Run") && vectorMoverse != Vector3.zero)
        {
            //Incrementamos la velocidad de desplazamiento
            velocidad = 7f;
        }

        //Si no, mantenemos la velocidad de desplazamiento original
        else
        {
            //Mantenemos la velocidad de desplazamiento original
            velocidad = 4f;
        }

        //Aumentamos gradualmente la velocidad de caida del jugador
        vectorCaida.y += gravedad * Time.deltaTime;

        //Reflejamos el movimiento horizontal y de caida sobre el jugador
        controller.Move(vectorMoverse * velocidad * Time.deltaTime + vectorCaida * Time.deltaTime);
    }

    /// <summary>
    /// Metodo para manchar al jugador
    /// </summary>
    /// <param name="pColor">El material de manchado que se asignara al jugador</param>
    /// Version 1.0
    /// Fecha de creacion 18/05/22
    public void Manchar(Material pColor)
    {
        Manchado = true;
        gameObject.GetComponent<MeshRenderer>().material = pColor;
    }
}
