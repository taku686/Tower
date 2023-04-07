using System;
using UnityEngine;

public class BlockData : IDisposable
{
    public readonly string Name;
    public readonly int Id;
    public readonly int Category;
    public Sprite BlockSprite;

    public BlockData(string name, int id, int category)
    {
        Name = name;
        Id = id;
        Category = category;
    }

    public void Dispose()
    {
    }
}

public enum Category
{
    Tohoku,
    Kanto,
    Chubu,
    Kinki,
    Chugoku,
    Shikoku,
    Kyushu,
}