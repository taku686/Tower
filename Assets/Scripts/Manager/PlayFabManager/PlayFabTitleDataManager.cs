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

    public void Initialize(BlockDataManager blockDataManager, StageDataManager stageDataManager)
    {
        _blockDataManager = blockDataManager;
        _stageDataManager = stageDataManager;
    }

    public async UniTask SetTitleData(Dictionary<string, string> titleDatum)
    {
        var blockDatum = JsonConvert.DeserializeObject<BlockData[]>(titleDatum[GameCommonData.BlockMasterDataKey]);
        await SetBlockDataManager(blockDatum);
        var stageDatum = JsonConvert.DeserializeObject<StageData[]>(titleDatum[GameCommonData.StageMasterDataKey]);
        await SetStageDataManager(stageDatum);
    }

    private async UniTask SetBlockDataManager(BlockData[] blockDatum)
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
}