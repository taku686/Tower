using Cysharp.Threading.Tasks;
using Data;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayFabLoginManager : MonoBehaviour
    {
        private GetPlayerCombinedInfoRequestParams _info;


        public void Initialize()
        {
            PlayFabSettings.staticSettings.TitleId = GameCommonData.TitleId;
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

            Debug.Log("login");
            return true;
        }
    }
}