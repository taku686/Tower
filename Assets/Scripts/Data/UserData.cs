using System;
using UnityEngine;

public class UserData : IDisposable
{
    public string Name;
    public int WinCount;
    public int ContinuityWinCount;

    public void Dispose()
    {
    }
}