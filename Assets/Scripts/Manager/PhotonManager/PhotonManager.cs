using Data;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;

namespace Photon
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        private readonly Subject<Unit> _changeTurn = new();
        private readonly Subject<int> _changeIndex = new();
        private readonly Subject<int> _generateBlock = new();
        public Subject<int> GenerateBlock => _generateBlock;

        public Subject<int> ChangeIndex => _changeIndex;

        public Subject<Unit> ChangeTurn => _changeTurn;


        public void OnStartConnectNetwork()
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions()
            {
                MaxPlayers = 2,
                IsOpen = true,
                IsVisible = true,
                EmptyRoomTtl = 0
            }, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            foreach (var prop in changedProps)
            {
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            foreach (var property in propertiesThatChanged)
            {
                if ((string)property.Key == GameCommonData.IsMyTurnKey)
                {
                    _changeTurn.OnNext(Unit.Default);
                    if ((bool)property.Value && PhotonNetwork.IsMasterClient)
                    {
                    }

                    if (!(bool)property.Value && !PhotonNetwork.IsMasterClient)
                    {
                        _changeTurn.OnNext(Unit.Default);
                    }
                }

                if ((string)property.Key == GameCommonData.BlockIndexKey)
                {
                    var index = (int)property.Value;
                    if (index == GameCommonData.ErrorCode)
                    {
                        _changeIndex.OnNext(0);
                        return;
                    }

                    _changeIndex.OnNext(index);
                }

                if ((string)property.Key == GameCommonData.GenerateBlockKey)
                {
                    var index = (int)property.Value;
                    if (index == GameCommonData.ErrorCode)
                    {
                        return;
                    }

                    _generateBlock.OnNext(index);
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
        }

        private void OnDestroy()
        {
            _changeTurn.Dispose();
            _changeIndex.Dispose();
        }
    }
}