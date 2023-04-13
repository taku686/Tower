using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class PlayFabUserDataManager : MonoBehaviour
{
    /// <summary>
    /// playerデータの更新
    /// </summary>
    public async UniTask<bool> TryUpdateUserDataAsync(UserData value)
    {
        var userJson = JsonConvert.SerializeObject(value);
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {
                    GameCommonData.UserKey, userJson
                }
            }
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        if (response.Error != null)
        {
            Debug.Log(response.Error.GenerateErrorReport());
            return false;
        }

        return true;
    }

    public async UniTask<UserData> GetPlayerData(string key)
    {
        var request = new GetUserDataRequest
        {
            PlayFabId = GameCommonData.TitleId,
            Keys = new List<string> { key }
        };

        var response = await PlayFabClientAPI.GetUserDataAsync(request);
        if (response.Error != null)
        {
            Debug.Log(response.Error.GenerateErrorReport());
            return null;
        }

        var value = response.Result.Data[key].Value;
        var user = JsonConvert.DeserializeObject<UserData>(value);
        if (user == null)
        {
            return null;
        }

        return user;
    }

    public async UniTask DeletePlayerDataAsync()
    {
        var request = new UpdateUserDataRequest()
        {
            KeysToRemove = new List<string> { GameCommonData.UserKey }
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        if (response.Error != null)
        {
            Debug.Log(response.Error.GenerateErrorReport());
        }
    }

    public async UniTask<bool> UpdateUserDisplayName(string playerName, TextMeshProUGUI errorText)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            return false;
        }

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = playerName
        };

        var response = await PlayFabClientAPI.UpdateUserTitleDisplayNameAsync(request);
        if (response.Error != null)
        {
            switch (response.Error.Error)
            {
                case PlayFabErrorCode.InvalidParams:
                    Debug.Log("名前の文字数制限エラーです");
                    errorText.text = "名前は3~15文字以内で入力して下さい。";
                    return false;
                case PlayFabErrorCode.ProfaneDisplayName:
                    Debug.Log("この名前は使用できません");
                    errorText.text = "この名前は使用できません。";
                    return false;
                default:
                    errorText.text = "この名前は使用できません。";
                    Debug.Log(response.Error.GenerateErrorReport());
                    return false;
            }
        }

        return true;
    }

    public void Dispose()
    {
    }
}