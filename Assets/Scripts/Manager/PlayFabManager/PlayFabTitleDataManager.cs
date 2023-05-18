using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Manager.DataManager;
using Newtonsoft.Json;
using UnityEngine;

public class PlayFabTitleDataManager : MonoBehaviour
{
    private BlockDataManager _blockDataManager;
    private StageDataManager _stageDataManager;
    private IconDataManager _iconDataManager;

    public void Initialize(BlockDataManager blockDataManager, StageDataManager stageDataManager,
        IconDataManager iconDataManager)
    {
        _blockDataManager = blockDataManager;
        _stageDataManager = stageDataManager;
        _iconDataManager = iconDataManager;
    }

    public async UniTask SetTitleData(Dictionary<string, string> titleDatum)
    {
        var blockDatum = JsonConvert.DeserializeObject<BlockData[]>(titleDatum[GameCommonData.BlockMasterDataKey]);
        SetBlockDataManager(blockDatum);
        var stageDatum = JsonConvert.DeserializeObject<StageData[]>(titleDatum[GameCommonData.StageMasterDataKey]);
        await SetStageDataManager(stageDatum);
        var iconDatum = JsonConvert.DeserializeObject<IconData[]>(titleDatum[GameCommonData.IconMasterDataKey]);
        await SetIconDataManager(iconDatum);
    }

    private void SetBlockDataManager(BlockData[] blockDatum)
    {
        foreach (var data in blockDatum)
        {
            _blockDataManager.AddBlockData(data);
        }
    }

    private async UniTask SetStageDataManager(StageData[] stageDatum)
    {
        foreach (var data in stageDatum)
        {
            var obj =
                await Resources.LoadAsync<GameObject>(GameCommonData.StagePrefabPass + data.Stage + "/" + data.Name);
            if (obj == null)
            {
                continue;
            }

            data.StageObj = (GameObject)obj;
            _stageDataManager.AddStageData(data);
        }
    }

    private async UniTask SetIconDataManager(IconData[] iconDatum)
    {
        foreach (var data in iconDatum)
        {
            var obj =
                await Resources.LoadAsync<Sprite>(GameCommonData.IconSpritePass + data.Name);
            if (obj == null)
            {
                continue;
            }

            data.Sprite = (Sprite)obj;
            _iconDataManager.AddIconData(data);
        }
    }
}