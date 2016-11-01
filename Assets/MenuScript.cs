using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
