using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathController : MonoBehaviour
{
    [SerializeField][Range(1f, 10f)] private float _afterDeathDelay = 5f;
    [SerializeField] private string _menuSceneName = "Menu";

    public bool IsAlive { get; private set; } = true;

    public event Action OnDeath;

	public void Die()
    {
        if (!IsAlive)
            return;

		Debug.Log("The player has died");

        IsAlive = false;

        OnDeath?.Invoke();

		StartCoroutine(LoadMenuScene());
	}

    private IEnumerator LoadMenuScene()
    {
        yield return new WaitForSeconds(_afterDeathDelay);

        SceneManager.LoadScene(_menuSceneName);
    }
}
