using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine;

// MonoBehaviourPunCallbacks : Pun 서비스의 이벤트를 감지할 수 있는 형태의 MonoBehaviour
public class LobbyManager : MonoBehaviourPunCallbacks
{
    // 게임 버전이 같을 때 매칭을 하기 위함
    private readonly string gameVersion = "1";

    public Text connectionInfoText;
    public Button joinButton;
    
    // 로비 진입과 동시에 마스터 서버에 접속 시도
    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        // 설정 정보를 가지고 마스터 서버에 접속(여기서는 게임 버전)
        PhotonNetwork.ConnectUsingSettings();

        joinButton.interactable = false;
        connectionInfoText.text = "Connecting to Master Server...";
    }
    
    // 마스터 서버에 접속한 순간 자동으로 실행
    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
        connectionInfoText.text = "Online : Connected to Master Server";
    }
    
    // 접속을 시도했으나 실패한 경우나 이미 접속한 상태에서 네트워크 연결이 끊어질 경우 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = $"Offline : Connection Disabled {cause.ToString()}";

        // 재접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public void Connect()
    {
        joinButton.interactable = false;

        if(PhotonNetwork.IsConnected){
            connectionInfoText.text = "Connecting to Random Room...";
            PhotonNetwork.JoinRandomRoom();
        }
        else{
            connectionInfoText.text = "Offline : Connection Disabled - Try Reconnecting...";
            // 재접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    
    // JoinRandomRoom 실행 후 빈 방이 없을 때 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "There is no empty room, Creating new Room";
        // 방 생성
        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = 2});
    }
    
    // 방에 참가 완료 되었을 때 자동 실행(방 접속에 성공했거나 자기가 만들었거나)
    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "Connected with Room.";
        // 호스트가 Main 씬을 로드하면 나머지 플레이어들도 따라서 로드를 함
        PhotonNetwork.LoadLevel("Main");
        /* 
         * SceneManager.LoadScene()을 쓰면 안 되는 이유?
         * 이건 '나의 세상'에서만 일어나는 일이기 때문에 다른 사람들은 같이 안 넘어감
         * 동시에 들어간다고 해도 각각 따로 들어가는 거라 동기화가 전혀 되지 않고 제각각 독자적으로 메인씬을 굴림
         * 따라서 한 사람만 이동해도 다른 사람들이 똑같은 레벨로 들어가게 하기 위해,
         * 각각의 레벨이 동기화가 될 수 있도록 하기 위해 LoadLevel을 사용
         */
    }
    
}