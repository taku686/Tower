using Data;
using ExitGames.Client.Photon;
using Photon.Realtime;

public static class RoomPropertiesExtensions
{
    private static readonly Hashtable PropsToSet = new();

    public static bool GetIsMyTurn(this Room room)
    {
        return room.CustomProperties[GameCommonData.IsMyTurnKey] is bool and true;
    }

    public static void SetIsMyTurn(this Room room, bool turn)
    {
        PropsToSet[GameCommonData.IsMyTurnKey] = turn;
        room.SetCustomProperties(PropsToSet);
        PropsToSet.Clear();
    }

    public static int GetBlockIndex(this Room room)
    {
        return room.CustomProperties[GameCommonData.BlockIndexKey] is int index ? index : 999;
    }

    public static void SetBlockIndex(this Room room, int index)
    {
        PropsToSet[GameCommonData.BlockIndexKey] = index;
        room.SetCustomProperties(PropsToSet);
        PropsToSet.Clear();
    }

    public static int GetGenerateBlock(this Room room)
    {
        return room.CustomProperties[GameCommonData.GenerateBlockKey] is int index ? index : 999;
    }

    public static void SetGenerateBlock(this Room room, int index)
    {
        PropsToSet[GameCommonData.GenerateBlockKey] = index;
        room.SetCustomProperties(PropsToSet);
        PropsToSet.Clear();
    }
}