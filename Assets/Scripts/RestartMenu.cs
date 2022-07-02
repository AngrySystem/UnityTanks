using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartMenu : MonoBehaviour
{
    Canvas canvas;
    public Text text;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    public void ShowMenu(bool losewin)
    {
        canvas.enabled = true;
        if (losewin)
        {
            text.text = "You lose";
        }
        else
        {
            text.text = "You win";
        }
    }

    public void OnClickButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
}