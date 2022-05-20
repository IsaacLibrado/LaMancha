using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase padre para los personajes en el juego
/// </summary>
public class Participante : MonoBehaviour {

	//Para identificar que estamos manchados
	[SerializeField]
	public bool Manchado;


	/// <summary>
	/// Metodo para revisar si estamos manchados o no
	/// </summary>
	/// <returns>El estado de manchado</returns>
	/// Version 1.0
	/// Fecha de creacion 18/05/22
	public bool IsManchado()
	{
		return Manchado;
	}
}
