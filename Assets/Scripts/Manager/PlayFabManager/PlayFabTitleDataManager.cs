using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Manager.DataManager;
using Newtonsoft.Json;
using UnityEngine;

public class PlayFabTitleDataManager : MonoBehaviour
{
    private BlockDataManager _blockDataManager;

    public void Initialize(BlockDataManager blockDataManager)
    {
        _blockDataManager = blockDataManager;
    }

    public async UniTask SetTitleData(Dictionary<string, string> titleDatum)
    {
        var blockDatum = JsonConvert.DeserializeObject<BlockData[]>(titleDatum[GameCommonData.BlockMasterDataKey]);
        await SetBlockDataManager(blockDatum);
    }

    private async UniTask SetBlockDataManager(BlockData[] blockDatum)
    {
        foreach (var data in blockDatum)
        {
            var sprite = await Resources.LoadAsync<Sprite>(GameCommonData.BlockSpritePass + data.Name);
            if (sprite == null)
            {
                continue;
            }

            data.BlockSprite = (Sprite)sprite;
            _blockDataManager.AddBlockData(data);
        }
    }
}