using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth; //������ ���� ���� ����
    public TMP_InputField email;
    public TMP_InputField password;
    public TMP_Text loginFailedtext;
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void Create() //���� ����
    {
        auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task => //���ٽ�, ����� task�� ���´�.
        {
            if (task.IsFaulted) //������ ������ ������ ���,
            {
                Debug.Log("���� ���� ����");
                return;
            }
            if (task.IsCanceled) //���� ������ �������� ���(��Ʈ��ũ ���, ���� ���)
            {
                Debug.Log("���� ���� ���");
                return;
            }
            FirebaseUser newUser = task.Result.User; //������ ������ ���ϰų� �������� ��찡 �ƴ� ���
        });
    }

    public async void LogIn() //�α��� �Լ� ���� ������ ������ �ڵ� ����, ����
    {
        try
        {
            var signInTask = await auth.SignInWithEmailAndPasswordAsync(email.text, password.text);
            Debug.Log("�α��� ����");
            SceneManager.LoadScene(1);
        }
        catch (System.Exception e)
        {
            Debug.Log($"�α��� ����: {e.Message}");
            loginFailedtext.text = "Login Failed";
        }
    }

    public void LogOut() //�α׾ƿ� �Լ�
    {
        auth.SignOut();
        Debug.Log("�α׾ƿ�"); //�α׾ƿ� Ȯ��
    }
}