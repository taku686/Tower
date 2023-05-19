using System;
using UnityEngine;

public class IconData : IDisposable
{
    public string Name;
    public int Index;
    public Sprite Sprite;

    public void Dispose()
    {
    }
}