using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Photon 네임스페이스 추가
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks //상속 클래스 변경
{
    //변수 선언
    private readonly string version = "1.0";//게임 버전 체크. 유저가 건드리지 못하게 private, readonly
    private string userId = "Victor"; //아무거나 userId 생성

    //플레이어 범위 랜덤 생성을 위한 콜라이더 변수 선언
    public Collider spawnArea;

    //네트워크 접속은 Start()보다 먼저 실행되어야한다. Awake() 함수 사용
    private void Awake()
    {
        //씬 동기화. 맨 처음 접속한 사람이 방장이 된다.
        PhotonNetwork.AutomaticallySyncScene = true;
        //버전 할당. 위에 string으로 만들었던 version을 쓴다.
        PhotonNetwork.GameVersion = version;
        //App ID 할당. 위에 userId로 만들었던 userId를 쓴다.
        PhotonNetwork.NickName = userId;
        //포톤 서버와의 통신 횟수를 로그로 찍기. 기본값 : 30
        Debug.Log(PhotonNetwork.SendRate); //제대로 통신이 되었다면 30이 출력된다.
        //포톤 서버에 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    //CallBack 함수
    public override void OnConnectedToMaster() //정상적으로 마스터 서버에 접속이 되면 호출된다.
    {
        //마스터 서버에 접속이 되었는지 디버깅 한다.
        Debug.Log("Connected to Master");
        Debug.Log($"In Lobby = {PhotonNetwork.InLobby}"); //로비에 들어와 있으면 True, 아니면 False 반환. Master 서버에는 접속했지만 로비에는 아니므로 False 반환된다.
        //로비 접속
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() //로비에 접속이 제대로 되었다면 해당 콜백함수 호출
    {
        Debug.Log($"In Lobby = {PhotonNetwork.InLobby}"); //로비에 접속이 되었다면 True가 반환 될 것이다.
        //방 접속 방법은 두 가지. 1.랜덤 매치메이킹, 2.선택된 방 접속
        PhotonNetwork.JoinRandomRoom();
    }

    //방 생성이 되지 않았으면 오류 콜백 함수 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}: {message}");

        //룸 속성 설정
        RoomOptions roomOptions = new RoomOptions();
        //룸의 접속할 수 있는 최대 접속자 수 최대 제한을 해놔야 CCU를 제한할 수 있다.
        roomOptions.MaxPlayers = 20;
        //룸 오픈 여부
        roomOptions.IsOpen = true;
        //로비에서 룸의 목록에 노출시킬지 여부. 공개방 생성
        roomOptions.IsVisible = true;
        //룸 생성
        PhotonNetwork.CreateRoom("Room1", roomOptions); //룸 이름과 룸 설정. 우리는 roomOptions에 설정을 이미 해놓았다.
    }

    //제대로 룸이 있다면 다음의 콜백 함수를 호출한다.
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name: {PhotonNetwork.CurrentRoom.Name}");
    }

    //룸에 들어왔을 때 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"In Room = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        //접속한 사용자 닉네임 확인
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            //플레이어 닉네임, 유저의 고유값 가져오기
            Debug.Log($"플레이어 닉네임: {player.Value.NickName}, 유저 고유값: {player.Value.ActorNumber}");
        }

        if(spawnArea != null) //만약 콜라이더가 비어있지 않다면,
        {
            Vector3 randomPoint = GetRandomPointInCollider(spawnArea);
            PhotonNetwork.Instantiate("Player", randomPoint, Quaternion.identity, 0);
        }
        else
        {
            Debug.Log("Spawn Area not assigned");
        }

        /*
        //플레이어 생성 포인트 그룹 배열을 받아오기. 포인트 그룹의 자식 오브젝트의 Transform 받아오기.
        Transform[] points = GameObject.Find("PointGroup").GetComponentsInChildren<Transform>();
        //1부터 배열의 길이까지의 숫자 중 Random한 값을 추출
        int idx = Random.Range(1, points.Length);
        //플레이어 프리팹을 추출한 idx 위치와 회전 값에 생성. 네트워크를 통해서.
        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);
        */
    }

    Vector3 GetRandomPointInCollider(Collider collider)
    {
        Bounds bounds = collider.bounds; //콜라이더의 좌표 가져오기

        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, 3.044f, randomZ); //플레이어의 높이는 고정
    }
}