using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    #region Variables

    [SerializeField] private float gameOverDelay = 2f;
    [SerializeField] private float nextLevelDelay = 1f;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private GameObject gameOverViewPrefab;

    #endregion

    #region Unity lifecycle

    private void OnEnable()
    {
        Player.OnDied += HandlePlayerDeath;
        ZombieBoss.OnDied += HandleBossDeath;
        GameOverView.OnClosed += Restart;
    }

    private void OnDisable()
    {
        Player.OnDied -= HandlePlayerDeath;
        ZombieBoss.OnDied -= HandleBossDeath;
        GameOverView.OnClosed -= Restart;
    }

    #endregion

    #region Private methods

    private void HandlePlayerDeath()
    {
        StartCoroutine(UpdateRestart());
    }

    private void HandleBossDeath()
    {
        StartCoroutine(UpdateNextLevel());
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

    private void GoToNextLevel()
    {
        StopAllCoroutines();

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}