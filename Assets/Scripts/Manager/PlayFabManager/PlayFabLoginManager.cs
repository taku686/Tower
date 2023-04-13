using Cysharp.Threading.Tasks;
using Data;
using Manager.DataManager;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayFabLoginManager : MonoBehaviour
    {
        private GetPlayerCombinedInfoRequestParams _info;
        private PlayFabTitleDataManager _playFabTitleDataManager;
        private UserDataManager _userDataManager;

        public void Initialize(PlayFabTitleDataManager playFabTitleDataManager, UserDataManager userDataManager)
        {
            PlayFabSettings.staticSettings.TitleId = GameCommonData.TitleId;
            _playFabTitleDataManager = playFabTitleDataManager;
            _userDataManager = userDataManager;
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
            /*var userData = JsonConvert.DeserializeObject<UserData>(response.Result.InfoResultPayload
                .UserData[GameCommonData.UserKey].Value);*/
            await _playFabTitleDataManager.SetTitleData(titleData);
            /*if (userData == null)
            {
                var newData = _userDataManager.CreateUserData();
                await _userDataManager.UpdateUserData(newData);
            }*/

            return true;
        }
    }
}