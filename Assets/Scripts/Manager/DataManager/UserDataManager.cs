using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private UserData _userData;
    private PlayFabUserDataManager _playFabUserDataManager;
    private int _enemyRate;

    public void Initialize(PlayFabUserDataManager playFabUserDataManager)
    {
        _playFabUserDataManager = playFabUserDataManager;
    }

    public void SetUserData(UserData userData)
    {
        _userData = userData;
    }

    public UserData GetUserData()
    {
        return _userData;
    }

    public async UniTask<string> GetUserName()
    {
        var userName = await _playFabUserDataManager.GetUserDisplayName();
        return userName;
    }

    public async UniTask<bool> SetUserName(string userName)
    {
        var result = await _playFabUserDataManager.UpdateUserDisplayName(userName);
        if (!result)
        {
            return false;
        }

        PlayerPrefs.SetString(GameCommonData.UserKey, userName);
        _userData.Name = userName;
        return true;
    }

    public void SetWinCount()
    {
        _userData.WinCount++;
        if (_userData.WinCount >= GameCommonData.MaxWinCount)
        {
            _userData.WinCount = GameCommonData.MaxWinCount;
        }
    }

    public int GetWinCount()
    {
        return _userData.WinCount;
    }

    public void SetLoseCount()
    {
        _userData.LoseCount++;
        if (_userData.LoseCount >= GameCommonData.MaxWinCount)
        {
            _userData.LoseCount = GameCommonData.MaxWinCount;
        }
    }

    public int GetLoseCount()
    {
        return _userData.LoseCount;
    }

    public void SetRate(bool isWin, int blockCount)
    {
        _userData.Rate += CalculateAddRate(isWin, blockCount);
        if (_userData.Rate >= GameCommonData.MaxRateCount)
        {
            _userData.Rate = GameCommonData.MaxRateCount;
        }
    }

    public int CalculateAddRate(bool isWin, int blockCount)
    {
        var enemyRate = GetEnemyRate();
        var myRate = GetRate();
        var bonusPoint = Mathf.CeilToInt(blockCount / 5f);
        var addRate = 0;
        if (isWin)
        {
            if (myRate < enemyRate)
            {
                addRate += 5;
            }
            else if (myRate == enemyRate)
            {
                addRate += 3;
            }
            else
            {
                addRate += 1;
            }

            addRate += bonusPoint;
        }
        else
        {
            if (myRate < enemyRate)
            {
                addRate -= 1;
            }
            else if (myRate == enemyRate)
            {
                addRate -= 3;
            }
            else
            {
                addRate -= 5;
            }
        }

        return addRate;
    }

    public int GetRate()
    {
        return _userData.Rate;
    }

    public void SetEnemyRate(int rate)
    {
        _enemyRate = rate;
    }

    public int GetEnemyRate()
    {
        return _enemyRate;
    }

    public void SetIconIndex(int index)
    {
        _userData.IconIndex = index;
    }

    public int GetIconIndex()
    {
        return _userData.IconIndex;
    }

    public Sprite GetIconImage()
    {
        return _userData.IconImage;
    }

    public void SetCurrentContinuityWinCount(bool isWin)
    {
        if (isWin)
        {
            _userData.CurrentContinuityWinCount++;
            if (_userData.CurrentContinuityWinCount >= GameCommonData.MaxWinCount)
            {
                _userData.CurrentContinuityWinCount = GameCommonData.MaxWinCount;
            }
        }
        else
        {
            _userData.CurrentContinuityWinCount = 0;
        }
    }

    public int GetCurrentContinuityWinCount()
    {
        return _userData.CurrentContinuityWinCount;
    }

    public void SetMaxContinuityWinCount()
    {
        if (GetCurrentContinuityWinCount() < GetMaxContinuityWinCount())
        {
            return;
        }

        _userData.MaxContinuityWinCount = GetCurrentContinuityWinCount();
        if (_userData.MaxContinuityWinCount >= GameCommonData.MaxWinCount)
        {
            _userData.MaxContinuityWinCount = GameCommonData.MaxWinCount;
        }
    }

    public int GetMaxContinuityWinCount()
    {
        return _userData.MaxContinuityWinCount;
    }

    public async UniTask UpdateUserData(UserData userData)
    {
        _userData = userData;
        await _playFabUserDataManager.TryUpdateUserDataAsync(_userData);
    }

    public async UniTask UpdateUserData()
    {
        await _playFabUserDataManager.TryUpdateUserDataAsync(_userData);
    }


    public UserData CreateUserData()
    {
        var userData = new UserData
        {
            Name = "",
            WinCount = 0,
            LoseCount = 0,
            Rate = 0,
            IconIndex = 0,
            MaxContinuityWinCount = 0,
            CurrentContinuityWinCount = 0
        };
        PlayerPrefs.SetString(GameCommonData.UserKey, "");
        _userData = userData;
        return _userData;
    }
}