using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Photon
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
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

        private void OnGUI()
        {
            GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
        }
    }
}