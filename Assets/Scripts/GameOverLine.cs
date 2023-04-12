using Block;
using Data;
using UniRx;
using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    public readonly ReactiveProperty<bool> GameEnd = new();

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(GameCommonData.BlockTag))
        {
            return;
        }

        var blockSc = col.GetComponent<BlockGameObject>();
        GameEnd.SetValueAndForceNotify(!blockSc.isOwn);
    }

    private void OnDestroy()
    {
        GameEnd.Dispose();
    }
}