using System;
using Block;
using Cysharp.Threading.Tasks;
using Data;
using Photon.Pun;
using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    [SerializeField] private Transform blockParent;
    [SerializeField] private PhysicsMaterial2D material;
    private int _count;
    private Vector3 _initPos;

    public void ResetBlockParent()
    {
        if (_initPos == Vector3.zero)
        {
            _initPos = blockParent.localPosition;
        }

        blockParent.localPosition = _initPos;
    }

    public async UniTask<GameObject> GenerateBlock(BlockData data)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        var block = PhotonNetwork.Instantiate(GameCommonData.BlockPrefabPass + data.Name,
            blockParent.position, blockParent.rotation);
        var blockSc = block.GetComponent<BlockGameObject>();
        _count++;
        blockSc.Initialize(BlockSate.Generating, _count);
        var rigid = block.GetComponent<Rigidbody2D>();
        rigid.gravityScale = 0;
        return block;
    }
}