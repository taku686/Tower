using Block;
using Data;
using UniRx;
using UnityEngine;

public class BlockParent : MonoBehaviour
{
    public readonly ReactiveProperty<float> AmountOfRise = new();

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag(GameCommonData.BlockTag))
        {
            return;
        }

        var blockSc = col.GetComponent<BlockGameObject>();
        if (blockSc.BlockStateReactiveProperty.Value != BlockSate.Stop)
        {
            return;
        }

        transform.position += new Vector3(0, GameCommonData.AmountOfRise, 0);
        AmountOfRise.SetValueAndForceNotify(GameCommonData.AmountOfRise);
    }
}