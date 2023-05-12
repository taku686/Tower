using System;
using UnityEngine;

public class UserData : IDisposable
{
    public string Name;
    public int WinCount;
    public int LoseCount;
    public int Rate;
    public int IconIndex;
    public Sprite IconImage;
    public int CurrentContinuityWinCount;
    public int MaxContinuityWinCount;

    public void Dispose()
    {
    }
}