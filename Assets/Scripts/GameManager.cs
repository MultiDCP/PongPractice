using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();

            return instance;
        }
    }

    private static GameManager instance;

    public Text scoreText;
    public Transform[] spawnPositions;
    public GameObject playerPrefab;
    public GameObject ballPrefab;

    private int[] playerScores;

    // 각각의 플레이어가 이를 즉각적으로 실행함
    private void Start()
    {
        playerScores = new[] {0, 0};
        SpawnPlayer();

        // 오직 호스트만이 공을 생성할 수 있음
        if(PhotonNetwork.IsMasterClient){
            SpawnBall();
        }
    }

    private void SpawnPlayer()
    {
        var localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        var spawnPosition = spawnPositions[localPlayerIndex % spawnPositions.Length];
        
        // 로컬의 세상에 먼저 생성하고 그 다음에 리모트로 다른 세상에 생성함
        // Resources 폴더에 있는 프리팹의 '이름'을 받아옴
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition.position, spawnPosition.rotation);
    }

    private void SpawnBall()
    {
        PhotonNetwork.Instantiate(ballPrefab.name, Vector2.zero, Quaternion.identity);
    }

    // 이미 접속했던 플레이어가 나가는 순간에 나간 플레이어 측에서 실행
    public override void OnLeftRoom()
    {
        // 본인 혼자 넘어가는 것이기 때문에 이걸 씀
        SceneManager.LoadScene("Lobby");
        //PhotonNetwork.LeaveRoom();
    }

    public void AddScore(int playerNumber, int score)
    {
        if(!PhotonNetwork.IsMasterClient) return;

        playerScores[playerNumber - 1] += score;

        photonView.RPC("RPCUpdateScoreText", RpcTarget.All, playerScores[0].ToString(), playerScores[1].ToString());
    }

    
    [PunRPC]
    private void RPCUpdateScoreText(string player1ScoreText, string player2ScoreText)
    {
        scoreText.text = $"{player1ScoreText} : {player2ScoreText}";
    }
    
}