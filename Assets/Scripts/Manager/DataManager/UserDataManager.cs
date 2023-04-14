using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private UserData _userData;

    private PlayFabUserDataManager _playFabUserDataManager;

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
            ContinuityWinCount = 0
        };
        PlayerPrefs.SetString(GameCommonData.UserKey, "");
        _userData = userData;
        return _userData;
    }
}