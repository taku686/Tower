using Cysharp.Threading.Tasks;
using Data;
using Manager.DataManager;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayFabLoginManager : MonoBehaviour
    {
        private GetPlayerCombinedInfoRequestParams _info;
        private PlayFabTitleDataManager _playFabTitleDataManager;

        public void Initialize(PlayFabTitleDataManager playFabTitleDataManager, BlockDataManager blockDataManager)
        {
            PlayFabSettings.staticSettings.TitleId = GameCommonData.TitleId;
            _playFabTitleDataManager = playFabTitleDataManager;
            _playFabTitleDataManager.Initialize(blockDataManager);
        }

        public async UniTask<bool> Login()
        {
            _info = new GetPlayerCombinedInfoRequestParams()
            {
                GetUserData = true,
                GetUserAccountInfo = true,
                GetTitleData = true,
                GetUserVirtualCurrency = true,
                GetUserInventory = true,
                GetPlayerProfile = true
            };
            var request = new LoginWithAndroidDeviceIDRequest()
            {
                CreateAccount = true,
                InfoRequestParameters = _info,
                AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
            };
            var response = await PlayFabClientAPI.LoginWithAndroidDeviceIDAsync(request);

            if (response.Error != null)
            {
                Debug.LogError(response.Error.GenerateErrorReport());
                return false;
            }


            var titleData = response.Result.InfoResultPayload.TitleData;
            await _playFabTitleDataManager.SetTitleData(titleData);
            return true;
        }
    }
}