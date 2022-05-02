using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class PlayerController : MonoBehaviour, Debuggable
{
	[SerializeField] protected GameObject _camera;
	[SerializeField] protected GameObject _UI;
	[SerializeField] protected GameObject _me;
	[SerializeField] protected PhotonView _view;
	[SerializeField] protected GameObject _mesh;
	protected SceneController _sceneController;
	public Constants.Team Team;
	public int ID;
	public int Score;


	// ------------ UNITY METHODS ------------

	void Awake()
	{
		ID = _view.ViewID;
		SetActive();
        SetTeam();
	}

	void OnEnable() { GameController.gameActive += OnGameActive; }

	void OnDisable() { GameController.gameActive -= OnGameActive; }

	void Start()
	{
		DontDestroyOnLoad(gameObject);

		gameObject.layer = Constants.LayerPlayer;

		FindObjectOfType<PreGameController>().Register(this);

		FindObjectOfType<HudDebugPanel>().Register(this);

		if (_view.IsMine) gameObject.tag = "Client";
		else
		{
			Destroy(_camera);
			Destroy(_UI);
			Destroy(_me);
		}

		// Allow master client to move players from one scene to another.
        PhotonNetwork.AutomaticallySyncScene = true;

		// Lock players cursor to center screen.
        Cursor.lockState = CursorLockMode.Locked;
	}


	// ------------ PRIVATE METHODS ------------

	private void OnGameActive(GameController game)
	{
		_sceneController = game;
		_sceneController.Register(this);
		Show();
	}

	protected abstract void SetActive();

    protected abstract void SetTeam();

    public abstract void SetSceneController(SceneController sceneController);

	public abstract void IncrementScore();


	// ------------ PUBLIC METHODS ------------

	public void Show()
	{
		if (!_view.IsMine)
		{
			gameObject.layer = Constants.LayerPlayer;
			_mesh.SetActive(true);
		}
	}

	public void Hide()
	{
		if (!_view.IsMine)
		{
			gameObject.layer = Constants.LayerOutsideReality;
			_mesh.SetActive(false);
		}
	}

	public Hashtable GetDebugValues()
	{
		Hashtable debugValues = new Hashtable();
		debugValues.Add($"{_view.ViewID} layer", gameObject.layer);
		return debugValues;
	}


	// ------------ RPC METHODS ------------

	[PunRPC]
	public void RPC_incrementScore() { Score++; }
}
