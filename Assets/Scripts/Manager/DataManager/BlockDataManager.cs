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

        public BlockData GetBlockData(int index)
        {
            if (!_blockDatum.Select(x => x.Id).Contains(index))
            {
                return null;
            }

            return _blockDatum[index];
        }
    }
}