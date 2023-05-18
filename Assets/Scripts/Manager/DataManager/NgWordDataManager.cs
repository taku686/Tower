using System.Collections.Generic;
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
                return false;
            }

            userName = userName.Trim();
            foreach (var word in _words)
            {
                if (word.Name == userName)
                {
                    return false;
                }
            }

            return true;
        }
    }
}