using ExitGames.Client.Photon;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : SimulationBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _networkRunner;
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Transform[] pointSpawns;
    private static int indexPointSpawn = -1;
    private Dictionary<PlayerRef, NetworkObject> spawnCharacter = new Dictionary<PlayerRef, NetworkObject>();

    private Vector3 lastMousePosition;
    private Vector3 dir = Vector3.zero;

    private async void StartGame(GameMode mode)
    {
        _networkRunner = gameObject.AddComponent<NetworkRunner>();
        _networkRunner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        await _networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "iam_tuan",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }


    private void Update()
    {
        HandleSwipeInput();
    }

    private void HandleSwipeInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastMousePosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 currentMousePosition = touch.position;
                Vector3 swipeDelta = currentMousePosition - lastMousePosition;

                if (swipeDelta.magnitude > 20) // Ngưỡng để xác định vuốt
                {
                    dir = new Vector3(swipeDelta.x, 0, swipeDelta.y).normalized;
                    lastMousePosition = currentMousePosition;
                }
            }
        }
    }

    private void Start()
    {
        StartGame(GameMode.AutoHostOrClient);
    }

    public void OnConnectedToServer(NetworkRunner runner) => Debug.Log("OnConnectedToServer");

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        => Debug.Log($"OnConnectFailed: {reason}");

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        => Debug.Log("OnConnectRequest");

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        => Debug.Log("OnCustomAuthenticationResponse");

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        => Debug.Log($"OnDisconnectedFromServer: {reason}");

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        => Debug.Log("OnHostMigration");

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData
        {
            direction = dir
        };
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        => Debug.Log($"OnInputMissing for player {player.PlayerId}");

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        => Debug.Log($"Object {obj.Id} entered AOI for player {player.PlayerId}");

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        => Debug.Log($"Object {obj.Id} exited AOI for player {player.PlayerId}");

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player.PlayerId} joined");
        if (runner.IsServer)
        {
            indexPointSpawn++;
            indexPointSpawn %= pointSpawns.Length;
            Vector3 spawnPosition = pointSpawns[indexPointSpawn].position;
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
            spawnCharacter.Add(player, networkPlayerObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player.PlayerId} left");
        if (spawnCharacter.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            spawnCharacter.Remove(player);
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        => Debug.Log($"OnReliableDataProgress for player {player.PlayerId}: {progress}");

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        => Debug.Log($"OnReliableDataReceived from player {player.PlayerId}");

    public void OnSceneLoadDone(NetworkRunner runner) => Debug.Log("OnSceneLoadDone");

    public void OnSceneLoadStart(NetworkRunner runner) => Debug.Log("OnSceneLoadStart");

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        => Debug.Log("OnSessionListUpdated");

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        => Debug.Log($"OnShutdown: {shutdownReason}");

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        => Debug.Log("OnUserSimulationMessage");
}
