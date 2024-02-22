using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
public class OwlScanerEnemy : MonoBehaviour
{
    private FieldOfView _fieldOfView;

	[Header("Player")]
	[SerializeField] private PlayerSanity _playerSanity;

	[Header("Values")]
	[SerializeField] private float _sanityPercentToStartScanning = 0.6f;
	[SerializeField][Range(10, 300)] private float _breakDelayInSeconds = 60f;

	[SerializeField] private BoxEnemy[] _boxEnemies;

	private bool _isEnemyCalledOut = false;

	private void Start()
	{
		_fieldOfView ??= GetComponent<FieldOfView>();
	}

	private void Update()
	{
		if (_playerSanity.SanityPercent > _sanityPercentToStartScanning || !_isEnemyCalledOut)
			return;

		_fieldOfView.VisionCheck();

		if (_fieldOfView.InstantDetectTarget || _fieldOfView.CanSeePlayer)
			CallEnemy();
	}

	private void CallEnemy()
	{
		if (_boxEnemies.Length < 0 || _isEnemyCalledOut)
			return;

		BoxEnemy boxEnemy = _boxEnemies[Random.Range(0, _boxEnemies.Length)];

		boxEnemy.OrderToAttack(_fieldOfView.Target.position);

		_isEnemyCalledOut = true;

		StartCoroutine(TakeBreak());
	}

	private IEnumerator TakeBreak()
	{
		yield return new WaitForSeconds(_breakDelayInSeconds);

		_isEnemyCalledOut = false;
	}
}
