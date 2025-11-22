using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void GoToHall()
    {
        SceneManager.LoadScene("Hall");
    }
}
