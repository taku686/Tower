using System;
using UnityEngine;

public class StageData : IDisposable
{
    public readonly string Name;
    public readonly int Id;
    public readonly int Category;
    public Sprite BlockSprite;
    
    public StageData(string name, int id, int category)
    {
        Name = name;
        Id = id;
        Category = category;
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