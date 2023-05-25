using System;

public class BlockData : IDisposable
{
    public readonly string Name;
    public readonly int Id;
    public readonly int Stage;
    public readonly float ScaleX;
    public readonly float ScaleY;

    public BlockData(string name, int id, int stage,float scaleX,float scaleY)
    {
        Name = name;
        Id = id;
        Stage = stage;
        ScaleX = scaleX;
        ScaleY = scaleY;
    }

    public void Dispose()
    {
    }
}