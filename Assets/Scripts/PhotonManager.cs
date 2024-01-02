using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Photon ���ӽ����̽� �߰�
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks //��� Ŭ���� ����
{
    //���� ����
    private readonly string version = "1.0";//���� ���� üũ. ������ �ǵ帮�� ���ϰ� private, readonly
    private string userId = "Victor"; //�ƹ��ų� userId ����

    //�÷��̾� ���� ���� ������ ���� �ݶ��̴� ���� ����
    public Collider spawnArea;

    //��Ʈ��ũ ������ Start()���� ���� ����Ǿ���Ѵ�. Awake() �Լ� ���
    private void Awake()
    {
        //�� ����ȭ. �� ó�� ������ ����� ������ �ȴ�.
        PhotonNetwork.AutomaticallySyncScene = true;
        //���� �Ҵ�. ���� string���� ������� version�� ����.
        PhotonNetwork.GameVersion = version;
        //App ID �Ҵ�. ���� userId�� ������� userId�� ����.
        PhotonNetwork.NickName = userId;
        //���� �������� ��� Ƚ���� �α׷� ���. �⺻�� : 30
        Debug.Log(PhotonNetwork.SendRate); //����� ����� �Ǿ��ٸ� 30�� ��µȴ�.
        //���� ������ ����
        PhotonNetwork.ConnectUsingSettings();
    }

    //CallBack �Լ�
    public override void OnConnectedToMaster() //���������� ������ ������ ������ �Ǹ� ȣ��ȴ�.
    {
        //������ ������ ������ �Ǿ����� ����� �Ѵ�.
        Debug.Log("Connected to Master");
        Debug.Log($"In Lobby = {PhotonNetwork.InLobby}"); //�κ� ���� ������ True, �ƴϸ� False ��ȯ. Master �������� ���������� �κ񿡴� �ƴϹǷ� False ��ȯ�ȴ�.
        //�κ� ����
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() //�κ� ������ ����� �Ǿ��ٸ� �ش� �ݹ��Լ� ȣ��
    {
        Debug.Log($"In Lobby = {PhotonNetwork.InLobby}"); //�κ� ������ �Ǿ��ٸ� True�� ��ȯ �� ���̴�.
        //�� ���� ����� �� ����. 1.���� ��ġ����ŷ, 2.���õ� �� ����
        PhotonNetwork.JoinRandomRoom();
    }

    //�� ������ ���� �ʾ����� ���� �ݹ� �Լ� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}: {message}");

        //�� �Ӽ� ����
        RoomOptions roomOptions = new RoomOptions();
        //���� ������ �� �ִ� �ִ� ������ �� �ִ� ������ �س��� CCU�� ������ �� �ִ�.
        roomOptions.MaxPlayers = 20;
        //�� ���� ����
        roomOptions.IsOpen = true;
        //�κ񿡼� ���� ��Ͽ� �����ų�� ����. ������ ����
        roomOptions.IsVisible = true;
        //�� ����
        PhotonNetwork.CreateRoom("Room1", roomOptions); //�� �̸��� �� ����. �츮�� roomOptions�� ������ �̹� �س��Ҵ�.
    }

    //����� ���� �ִٸ� ������ �ݹ� �Լ��� ȣ���Ѵ�.
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name: {PhotonNetwork.CurrentRoom.Name}");
    }

    //�뿡 ������ �� �ݹ� �Լ�
    public override void OnJoinedRoom()
    {
        Debug.Log($"In Room = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        //������ ����� �г��� Ȯ��
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            //�÷��̾� �г���, ������ ������ ��������
            Debug.Log($"�÷��̾� �г���: {player.Value.NickName}, ���� ������: {player.Value.ActorNumber}");
        }

        if(spawnArea != null) //���� �ݶ��̴��� ������� �ʴٸ�,
        {
            Vector3 randomPoint = GetRandomPointInCollider(spawnArea);
            PhotonNetwork.Instantiate("Player", randomPoint, Quaternion.identity, 0);
        }
        else
        {
            Debug.Log("Spawn Area not assigned");
        }

        /*
        //�÷��̾� ���� ����Ʈ �׷� �迭�� �޾ƿ���. ����Ʈ �׷��� �ڽ� ������Ʈ�� Transform �޾ƿ���.
        Transform[] points = GameObject.Find("PointGroup").GetComponentsInChildren<Transform>();
        //1���� �迭�� ���̱����� ���� �� Random�� ���� ����
        int idx = Random.Range(1, points.Length);
        //�÷��̾� �������� ������ idx ��ġ�� ȸ�� ���� ����. ��Ʈ��ũ�� ���ؼ�.
        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);
        */
    }

    Vector3 GetRandomPointInCollider(Collider collider)
    {
        Bounds bounds = collider.bounds; //�ݶ��̴��� ��ǥ ��������

        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, 3.044f, randomZ); //�÷��̾��� ���̴� ����
    }
}