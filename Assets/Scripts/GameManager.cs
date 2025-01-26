using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject gameMechanics;
    
    public delegate void GameStartHandler();
    public event GameStartHandler OnGameStart;

    private bool _isGameLost;
    
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }
    
    public void StartGame()
    {
        startScreen.SetActive(false);
        gameMechanics.SetActive(true);
        OnGameStart?.Invoke();
    }

    public void LoseGame()
    {
        if (_isGameLost) return;
        
        _isGameLost = true;
        gameMechanics.SetActive(false);
        loseScreen.SetActive(true);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
