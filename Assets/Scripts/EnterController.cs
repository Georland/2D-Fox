using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterController : MonoBehaviour
{
    public AudioSource enterAudio; // 到达终点的音效
    public GameObject dialogue;    // 到达终点弹出的对话框
    bool arrived = false;          // 判断是否已到达finish 点    

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
