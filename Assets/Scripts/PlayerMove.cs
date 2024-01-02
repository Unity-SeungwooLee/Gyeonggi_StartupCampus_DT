using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMove : MonoBehaviour
{
    //플레이어 상태 생성
    enum PlayerState
    {
        Idle, Move
    }
    //플레이어 상태 변수
    PlayerState playerState;

    CharacterController characterController;
    private new Transform transform;
    private new Camera camera;

    //플레이어 이동 속도
    public float movespeed = 10f;

    //플레이어 카메라 회전
    public float rotSpeed = 200f;

    //회전값 변수
    float mx = 0;

    //시네머신 가져오기
    CinemachineVirtualCamera virtualCamera;

    //플레이어 움직임 true/false
    //bool isMoving = false;

    //Animator 선언
    Animator anim;

    //포톤뷰 가져오기
    PhotonView pv;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        camera = Camera.main;

        //virtualCamera 가져오기
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        //애니메이터 가져오기
        anim = GetComponent<Animator>();

        playerState = PlayerState.Idle;

        //포톤뷰 가져오기
        pv = GetComponent<PhotonView>();

        if (pv.IsMine)//내 클라이언트 라면, (남의 캐릭터를 움직일 수 없게 함)
        {
            virtualCamera.Follow = transform; //나 자신의 transform
            virtualCamera.LookAt = transform; //나 자신의 transform
        }
    }
    void Update()
    {
        if (pv.IsMine)
        {
            //플레이어 상태 스위치
            switch (playerState)
            {
                case PlayerState.Idle:
                    Idle();
                    break;
                case PlayerState.Move:
                    Move();
                    break;
            }
        }
    }

    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");

    void Idle()
    {
        if (Input.anyKey)
        {
            playerState = PlayerState.Move;
            Debug.Log("플레이어 이동");
        }
    }
    void Move()
    {
        //카메라 설정
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        Vector3 moveDir = (cameraForward * v) + (cameraRight * h);

        moveDir.Set(moveDir.x, 0.0f, moveDir.z);
        characterController.SimpleMove(moveDir * movespeed);

        if (Input.GetKey("w"))
        {
            transform.forward = cameraForward;
            anim.SetBool("isWalking", true);
        }
        else if (Input.GetKey("a"))
        {
            transform.right = cameraForward * 90;
            anim.SetBool("isWalking", true);
        }
        else if (Input.GetKey("s"))
        {
            transform.forward = -cameraForward;
            anim.SetBool("isWalking", true);
        }
        else if (Input.GetKey("d"))
        {
            transform.right = cameraForward * -90;
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

    }
}