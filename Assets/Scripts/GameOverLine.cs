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

        Debug.Log("接触");
        var blockSc = col.GetComponent<BlockGameObject>();
        Debug.Log(blockSc.gameObject.name);
        GameEnd.SetValueAndForceNotify(!blockSc.isOwn);
    }

    private void OnDestroy()
    {
        GameEnd.Dispose();
    }
}