using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase para controlar el movimiento de camara
/// </summary>
/// Version 1.0
/// Fecha de creacion 18/05/22
public class MovCamara : MonoBehaviour {

    //Establecemos la sensibilidad del mouse para mover la visión del jugador
    public float sensibilidad = 1.6f;

    //Referenciamos a la posición del jugador
    public Transform jugador;

    //Declaramos la variable que nos permitirá subir y bajar la visión del jugador
    float xRotacion = 0f;

    //Referencia al game controller;
    public GameController gc;

    void Start()
    {
        //Bloqueamos el cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if(!gc.acabado)
        {
            //Si el jugador se encuentra bloqueado, impedimos que este pueda mover su visión
            MoverCamara();
        }
    }

    private void MoverCamara()
    {
        //Obtenemos los valores de movimiento en X y Y del mouse
        float mouseX = Input.GetAxis("Mouse X") * sensibilidad;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidad;

        //Guardamos el valor de movimiento en Y del mouse
        xRotacion -= mouseY;

        //Impedimos que el jugador pueda mover la visión del jugador mas allá de los 90 grados
        xRotacion = Mathf.Clamp(xRotacion, -70f, 90f);

        //Movemos la visión del jugador verticalmente
        transform.localRotation = Quaternion.Euler(xRotacion, 0f, 0f);

        //Rotamos al jugador de acuerdo al movimiento en X del mouse
        jugador.Rotate(Vector3.up * mouseX);
    }
}
