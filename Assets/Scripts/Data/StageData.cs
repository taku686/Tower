using System;
using UnityEngine;

public class StageData : IDisposable
{
    public readonly string Name;
    public readonly int Id;
    public readonly int Stage;
    public GameObject StageObj;
    
    public StageData(string name, int id, int stage)
    {
        Name = name;
        Id = id;
        Stage = stage;
    }

    public void Dispose()
    {
    }
}

public enum Stage
{
    Utsu,
    Byan,
    Bonnnou,
    Others
}