using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth; //인증을 위한 변수 선언
    public TMP_InputField email;
    public TMP_InputField password;
    public TMP_Text loginFailedtext;
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void Create() //계정 생성
    {
        auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task => //람다식, 결과는 task가 나온다.
        {
            if (task.IsFaulted) //계정을 만들지 못했을 경우,
            {
                Debug.Log("계정 생성 실패");
                return;
            }
            if (task.IsCanceled) //계정 생성을 실패했을 경우(네트워크 장애, 도중 취소)
            {
                Debug.Log("계정 생성 취소");
                return;
            }
            FirebaseUser newUser = task.Result.User; //계정을 만들지 못하거나 실패했을 경우가 아닐 경우
        });
    }

    public async void LogIn() //로그인 함수 계정 생성과 동일한 코드 복붙, 수정
    {
        try
        {
            var signInTask = await auth.SignInWithEmailAndPasswordAsync(email.text, password.text);
            Debug.Log("로그인 성공");
            SceneManager.LoadScene(1);
        }
        catch (System.Exception e)
        {
            Debug.Log($"로그인 실패: {e.Message}");
            loginFailedtext.text = "Login Failed";
        }
    }

    public void LogOut() //로그아웃 함수
    {
        auth.SignOut();
        Debug.Log("로그아웃"); //로그아웃 확인
    }
}