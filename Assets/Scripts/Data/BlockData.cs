using System;
using UnityEngine;

public class BlockData : IDisposable
{
    public readonly string Name;
    public readonly int Id;
    public readonly int Stage;
    public Sprite BlockSprite;

    public BlockData(string name, int id, int stage)
    {
        Name = name;
        Id = id;
        Stage = stage;
    }

    public void Dispose()
    {
    }
}