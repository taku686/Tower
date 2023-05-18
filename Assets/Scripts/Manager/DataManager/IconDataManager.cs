using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manager.DataManager
{
    public class IconDataManager : MonoBehaviour
    {
        private readonly List<IconData> _iconDatum = new();

        public void AddIconData(IconData data)
        {
            if (data == null || _iconDatum.Contains(data))
            {
                return;
            }

            _iconDatum.Add(data);
        }

        public Sprite GetIconSprite(int index)
        {
            return _iconDatum.FirstOrDefault(x => x.Index == index)!.Sprite;
        }
    }
}