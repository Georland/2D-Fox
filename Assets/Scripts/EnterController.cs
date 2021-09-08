using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterController : MonoBehaviour
{
    public AudioSource enterAudio; // �����յ����Ч
    public GameObject dialogue;    // �����յ㵯���ĶԻ���
    bool arrived = false;          // �ж��Ƿ��ѵ���finish ��    

    private void Update()
    {
        if (arrived && Input.GetKeyDown(KeyCode.E))
        {
            enterAudio.Play();
            Invoke("NextStage", 1.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            arrived = true;
            //enterAudio.Play();
            dialogue.SetActive(true);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            dialogue.SetActive(false);
        }
    }

    void NextStage()
    {        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        //for (int i = 0; i < Scenes.Length; i++)
        //{
        //    if (Scenes[i].Equals(activeScene))
        //    {
        //        SceneManager.LoadScene(Scenes[i + 1].name);
        //        return;
        //    }
        //}
    }
}
