using Cysharp.Threading.Tasks;
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

    public string GetUserName()
    {
        return _userData.Name;
    }

    public void SetUserName(string userName)
    {
        _userData.Name = userName;
    }

    public async UniTask UpdateUserData(UserData userData)
    {
        _userData = userData;
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
        _userData = userData;
        return _userData;
    }
}