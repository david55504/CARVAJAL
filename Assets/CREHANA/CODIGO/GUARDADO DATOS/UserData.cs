
using System;
using System.Collections.Generic;

[System.Serializable]
public class UserData
{
    public string nombre;
    public string correo;
    public string telefono;
}

[System.Serializable]
public class Decision
{
    public string escena;
    public string tipoDecision;
    public string valorSeleccionado;
}

[Serializable]
public class DecisionRequest
{
    public string userId;
    public Decision decision;
}

[Serializable]
public class RegistroResponse
{
    public bool success;
    public string userId;
    public string mensaje;
}

[Serializable]
public class DecisionResponse
{
    public bool success;
    public string mensaje;
}