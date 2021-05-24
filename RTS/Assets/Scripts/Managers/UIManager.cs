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
            StartCoroutine(AddMouseOrKeyboardMoveset());
        }
    }

     IEnumerator AddMouseOrKeyboardMoveset()
    {

            if (Input.GetKeyDown(KeyCode.M))
            {
                GameManager.Instance.player.AddComponent<PlayerInputMouse>();
                yield return new WaitForSecondsRealtime(2f);
                PlayerHandler.PlayerHandlerInstance.characterInput = GetComponent<CharacterInput>();
                Debug.Log(PlayerHandler.PlayerHandlerInstance.characterInput);
                PlayerHandler.PlayerHandlerInstance.cameraController?.Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
                GameManager.Instance.hasChosenTypeOfCamera = true;
                mouseOrKeyboardText.SetActive(false);
                Time.timeScale = 1;
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                GameManager.Instance.player.AddComponent<PlayerInputKeyboard>();
                yield return new WaitForSeconds(2f);
                PlayerHandler.PlayerHandlerInstance.characterInput = GetComponent<CharacterInput>();
                PlayerHandler.PlayerHandlerInstance.cameraController?.Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
                GameManager.Instance.hasChosenTypeOfCamera = true;
                mouseOrKeyboardText.SetActive(false);
                Time.timeScale = 1;
            }
        
    }
}
