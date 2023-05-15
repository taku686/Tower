using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manager.DataManager
{
    public class StageDataManager : MonoBehaviour
    {
        public readonly List<StageData> _stageDatum = new();

        public void AddStageData(StageData data)
        {
            if (data == null || _stageDatum.Contains(data))
            {
                return;
            }

            _stageDatum.Add(data);
        }

        public StageData GetRandomStageData()
        {
            var stageDatum = _stageDatum.Where(x => x.Stage == 3).ToList();
            Debug.Log(stageDatum.Count);
            var index = Random.Range(0, stageDatum.Count);
            Debug.Log("index" + index);
            return _stageDatum.Find(x => x.Id == index && x.Stage == 3);
        }
    }
}