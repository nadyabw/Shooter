using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    #region Variables

    [SerializeField] private float restartDelay = 2f;

    #endregion

    #region Unity lifecycle

    private void OnEnable()
    {
        Player.OnDied += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        Player.OnDied -= HandlePlayerDeath;
    }

    #endregion

    #region Private methods

    private void HandlePlayerDeath()
    {
        StartCoroutine(UpdateRestart());
    }

    private IEnumerator UpdateRestart()
    {
        yield return new WaitForSeconds(restartDelay);

        Restart();
    }

    private void Restart()
    {
        StopAllCoroutines();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}