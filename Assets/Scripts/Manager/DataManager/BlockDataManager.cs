using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manager.DataManager
{
    public class BlockDataManager : MonoBehaviour
    {
        private readonly List<BlockData> _blockDatum = new();

        public void AddBlockData(BlockData data)
        {
            if (data == null || _blockDatum.Contains(data))
            {
                return;
            }

            _blockDatum.Add(data);
        }

        public BlockData GetBlockData(int index, int stageIndex)
        {
            return _blockDatum.FirstOrDefault(x => x.Id == index && x.Stage == stageIndex);
        }

        public BlockData GetRandomBlockData()
        {
            var stageIndex = 3;
            var blockDatum = _blockDatum.Where(x => x.Stage == stageIndex).ToList();
            var index = Random.Range(0, blockDatum.Count);
            return blockDatum[index];
        }
    }
}