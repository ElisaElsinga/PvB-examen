using SK.Libretro.Examples;
using SK.Libretro.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//reads keyboard and performs certain actions that can't be done from UI
public class Mainloop : MonoBehaviour
{
    public LibretroInstanceVariable _libretro;
    [SerializeField] private GameObject _MainMenu;
    [SerializeField] private GameObject _OptionMenu;
    [SerializeField] private GameObject _SaveMenu;
    [SerializeField] private GameObject _LoadMenu;
    [SerializeField] private Button _resumeGame;
    [SerializeField] private Button _restartGame;
    [SerializeField] private Button _closeGame;
    private bool mKeyPressed;

    private static Mainloop instance;

    public static Mainloop Instance
    { get { return instance; } }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            _libretro.Current.FastForward = true;
        }
        else if (Input.GetKeyUp(KeyCode.N))
        {
            _libretro.Current.FastForward = false;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!mKeyPressed)
            {
                _libretro.PauseContent();
                _MainMenu.SetActive(true);
                PausedCallback(); 
            }
            else
            {
                _libretro.ResumeContent();
                _OptionMenu.SetActive(false);
                _MainMenu.SetActive(false);
                _SaveMenu.SetActive(false);
                _LoadMenu.SetActive(false);
            }
            mKeyPressed = !mKeyPressed;
        }

       
        if (Input.GetKeyDown(KeyCode.R))
        {
            _libretro.Current.Rewind = true;
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            _libretro.Current.Rewind = false;
        }
    }
    private void PausedCallback()
    {
        _resumeGame.onClick.AddListener(HandleButtonClick);
        _restartGame.onClick.AddListener(HandleButtonClick);
        _closeGame.onClick.AddListener(HandleButtonClick);
    }
    private void HandleButtonClick()
    {
        mKeyPressed = false;
    }

}
