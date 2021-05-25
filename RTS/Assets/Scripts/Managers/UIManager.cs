using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mouseOrKeyboardText;
    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.hasChosenTypeOfCamera)
        {
            AddMouseOrKeyboardMoveset();
        }
    }

     private void AddMouseOrKeyboardMoveset()
    {

            if (Input.GetKeyDown(KeyCode.M))
            {
                PlayerHandler.PlayerHandlerInstance.characterInput = GameManager.Instance.player.AddComponent<PlayerInputMouse>();
                PlayerHandler.PlayerHandlerInstance.cameraController?.Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
                GameManager.Instance.hasChosenTypeOfCamera = true;
                mouseOrKeyboardText.SetActive(false);
                Time.timeScale = 1;
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                PlayerHandler.PlayerHandlerInstance.characterInput = GameManager.Instance.player.AddComponent<PlayerInputKeyboard>();
                PlayerHandler.PlayerHandlerInstance.cameraController?.Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
                GameManager.Instance.hasChosenTypeOfCamera = true;
                mouseOrKeyboardText.SetActive(false);
                Time.timeScale = 1;
            }
        
    }
}
