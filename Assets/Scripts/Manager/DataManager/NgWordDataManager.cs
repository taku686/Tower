using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Manager.DataManager
{
    public class NgWordDataManager : MonoBehaviour
    {
        private readonly List<NGWord> _words = new();

        public void AddWord(NGWord word)
        {
            if (word == null || _words.Contains(word))
            {
                return;
            }

            _words.Add(word);
        }

        public bool Validate(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                Debug.LogError("名前が空です。");
                return false;
            }

            if (userName.Length > GameCommonData.MaxNameCount)
            {
                Debug.LogError("名前が8文字以上あります。");
                return false;
            }

            userName = userName.Trim();
            userName = userName.Replace(" ", "").Replace("　", "");
            Debug.Log(userName);
            foreach (var word in _words)
            {
                if (word.Name == userName)
                {
                    Debug.LogError("不適切な用語が含まれています。");
                    return false;
                }
            }

            return true;
        }
    }
}