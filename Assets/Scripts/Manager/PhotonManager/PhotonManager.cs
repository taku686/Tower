using System;
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
        private UserDataManager _userDataManager;
        private readonly Subject<Unit> _changeTurn = new();
        private readonly Subject<Unit> _forcedTermination = new();
        private readonly Subject<int> _changeIndex = new();
        private readonly Subject<int> _generateBlock = new();
        private readonly Subject<int> _battleEnd = new();
        private readonly Subject<int> _enemyRate = new();

        public Subject<int> EnemyRate => _enemyRate;
        public Subject<int> GenerateBlock => _generateBlock;
        public Subject<int> ChangeIndex => _changeIndex;
        public Subject<int> BattleEnd => _battleEnd;
        public Subject<Unit> ChangeTurn => _changeTurn;
        public Subject<Unit> ForcedTermination => _forcedTermination;


        public void Initialize(UserDataManager userDataManager)
        {
            _userDataManager = userDataManager;
        }


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
            var rate = _userDataManager.GetRate();
            PhotonNetwork.LocalPlayer.SetEnemyRate(rate);
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            _forcedTermination.OnNext(Unit.Default);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            foreach (var prop in changedProps)
            {
                if ((string)prop.Key == GameCommonData.EnemyRateKey)
                {
                    if (targetPlayer.IsLocal)
                    {
                        return;
                    }

                    var enemyRate = (int)prop.Value;
                    _enemyRate.OnNext(enemyRate);
                }
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
                    //   Debug.Log("ブロック生成");
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

                if ((string)property.Key == GameCommonData.BattleEndKey)
                {
                    var index = (int)property.Value;
                    if (index == GameCommonData.ErrorCode)
                    {
                        return;
                    }

                    //  Debug.Log("Battle End");
                    _battleEnd.OnNext(index);
                }
            }
        }

        /*private void OnGUI()
        {
            GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
        }*/

        private void OnDestroy()
        {
            _changeTurn.Dispose();
            _changeIndex.Dispose();
        }
    }
}