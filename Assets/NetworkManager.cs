using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.NetworkRunnerCallbackArgs;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkManager instance;
    public NetworkRunner networkRunner;
    public NetworkPrefabRef playerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        networkRunner.ProvideInput = true;
        networkRunner.AddCallbacks(this);
        StartGame();
    }

    private async void StartGame()
    {
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "iam_tuan",
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        await networkRunner.StartGame(startArgs);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData
        {
            direction = Vector3.zero
        };

        if (Input.GetKey(KeyCode.W)) data.direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) data.direction += Vector3.back;
        if (Input.GetKey(KeyCode.A)) data.direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) data.direction += Vector3.right;

        input.Set(data);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} left the game.");
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.LogWarning($"Input missing from player {player}.");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"Server shutdown. Reason: {shutdownReason}");
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server.");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnected from server.");
    }

    public void OnConnectRequest(NetworkRunner runner, ConnectRequest request, byte[] token)
    {
        Debug.Log("Connection request received.");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError($"Connection failed with {remoteAddress}. Reason: {reason}");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Session list updated.");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("Custom authentication response received.");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log($"Reliable data received from player {player}.");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scene load done.");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Scene load started.");
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log($"Object {obj} entered player {player}'s AOI.");
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log($"Object {obj} exited player {player}'s AOI.");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("User simulation message received.");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Host migration.");
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        Debug.Log($"Reliable data progress from player {player}: {progress * 100}%");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"Disconnected from server. Reason: {reason}");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        Debug.Log($"Reliable data received from player {player} with key {key}.");
    }
}
