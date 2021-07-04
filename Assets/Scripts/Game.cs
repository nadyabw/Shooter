using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    #region Variables

    [Header("UI")]
    [SerializeField] private UIProgressBar playerHealthBar;
    [SerializeField] private UIProgressBar bossHealthBar;
    [SerializeField] private Text bossText;
    [SerializeField] private UIProgressBar playerAmmoBar;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private GameObject gameOverViewPrefab;
    [SerializeField] private GameObject gameCompletedViewPrefab;

    [SerializeField] private float gameOverDelay = 2f;
    [SerializeField] private float nextLevelDelay = 1f;

    #endregion

    #region Unity lifecycle

    private void OnEnable()
    {
        Player.OnDied += HandlePlayerDeath;
        Player.OnHealthChanged += HandlePlayerHealthChange;
        Player.OnAmmoChanged += HandlePlayerAmmoChange;
        ZombieBoss.OnDied += HandleBossDeath;
        ZombieBoss.OnHealthChanged += HandleBossHealthChange;
        ZombieBoss.OnActivated += HandleBossActivation;
        GameOverView.OnClosed += Restart;
        GameCompletedView.OnClosed += StartNewGame;
    }

    private void OnDisable()
    {
        Player.OnDied -= HandlePlayerDeath;
        Player.OnHealthChanged -= HandlePlayerHealthChange;
        Player.OnAmmoChanged -= HandlePlayerAmmoChange;
        ZombieBoss.OnDied -= HandleBossDeath;
        ZombieBoss.OnHealthChanged -= HandleBossHealthChange;
        ZombieBoss.OnActivated -= HandleBossActivation;
        GameOverView.OnClosed -= Restart;
        GameCompletedView.OnClosed -= StartNewGame;
    }

    #endregion

    #region Private methods

    private void HandlePlayerDeath()
    {
        StartCoroutine(UpdateRestart());
    }

    private void HandlePlayerHealthChange(float value)
    {
        playerHealthBar.UpdateState(value);
    }

    private void HandlePlayerAmmoChange(float value)
    {
        playerAmmoBar.UpdateState(value);
    }

    private void HandleBossDeath()
    {
        StartCoroutine(UpdateNextLevel());
    }

    private void HandleBossHealthChange(float value)
    {
        bossHealthBar.UpdateState(value);
    }

    private void HandleBossActivation()
    {
        bossHealthBar.gameObject.SetActive(true);
        bossText.gameObject.SetActive(true);
    }

    private IEnumerator UpdateRestart()
    {
        yield return new WaitForSeconds(gameOverDelay);

        ShowGameOverView();
    }

    private IEnumerator UpdateNextLevel()
    {
        yield return new WaitForSeconds(nextLevelDelay);

        GoToNextLevel();
    }

    private void ShowGameOverView()
    {
        StopAllCoroutines();

        Instantiate(gameOverViewPrefab, canvasTransform);
    }

    private void ShowGameCompletedView()
    {
        Instantiate(gameCompletedViewPrefab, canvasTransform);
    }

    private void GoToNextLevel()
    {
        StopAllCoroutines();

        Player.Instance.gameObject.SetActive(false);

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            ShowGameCompletedView();
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void StartNewGame()
    {
        SceneManager.LoadScene(0);
    }

    #endregion
}