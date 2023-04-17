using Block;
using Data;
using UniRx;
using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    public readonly ReactiveProperty<bool> GameEnd = new();

    public void Setup()
    {
        transform.position = new Vector3(0, -12.88f, 0);
    }

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