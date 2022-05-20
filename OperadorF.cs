using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase para los axiomas de la fuzzificacione
/// </summary>
public class OperadorF {
    public static float AND(float A, float B)
    {

        return Mathf.Min(A, B);

    }

    public static float OR(float A, float B)
    {

        return Mathf.Max(A, B);

    }

    public static float FuzzyNOT(float A)
    {

        return 1.0f - A;

    }
}
