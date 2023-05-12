using ExitGames.Client.Photon;
using Data;
using Photon.Realtime;

public static class PlayerPropertiesExtensions
{
    private static readonly Hashtable PropsToSet = new();

    public static int GetEnemyRate(this Player player)
    {
        return player.CustomProperties[GameCommonData.EnemyRateKey] is int index ? index : 0;
    }

    public static void SetEnemyRate(this Player player, int index)
    {
        PropsToSet[GameCommonData.EnemyRateKey] = index;
        player.SetCustomProperties(PropsToSet);
        PropsToSet.Clear();
    }
}