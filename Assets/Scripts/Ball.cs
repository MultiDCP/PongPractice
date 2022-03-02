using Photon.Pun;
using UnityEngine;

// MonoBehaviourPun : Photon View 컴포넌트로 즉시 접근할 수 있는 shortcut을 제공함
public class Ball : MonoBehaviourPun
{
    // 이 스크립트를 실행하는 플레이어가 호스트인지, 이 오브젝트가 로컬 오브젝트인지를 봄
    // 방장이 아닌 플레이어는 볼을 직접 움직이지 못하며 동기화를 받아오고, 호스트가 직접 볼을 움직임
    public bool IsMasterClientLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;

    private Vector2 direction = Vector2.right;
    private readonly float speed = 10f;
    private readonly float randomRefectionIntensity = 0.1f;
    
    private void FixedUpdate()
    {
        if(!IsMasterClientLocal || PhotonNetwork.PlayerList.Length < 2) return;
        
        var distance = speed * Time.deltaTime;
        var hit = Physics2D.Raycast(transform.position, direction, distance);

        if(hit.collider != null){
            var goalPost = hit.collider.GetComponent<Goalpost>();

            if(goalPost != null){
                if(goalPost.playerNumber == 1)
                    GameManager.Instance.AddScore(2, score:1);
                else if(goalPost.playerNumber == 2)
                    GameManager.Instance.AddScore(1, score:1);
            }

            direction = Vector2.Reflect(direction, hit.normal);
            direction += Random.insideUnitCircle * randomRefectionIntensity;
        }

        transform.position = (Vector2)transform.position + direction * distance;
    }
}