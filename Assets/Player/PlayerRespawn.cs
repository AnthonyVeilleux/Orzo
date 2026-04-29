using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    public CharacterController controller;
    public PauseMenu pauseMenu;
    public GameObject player;
    [SerializeField] private bool reloadSceneOnRespawn = true;

    private void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (player == null)
            player = gameObject;

        CheckpointManager checkpointManager = CheckpointManager.Instance;
        if (checkpointManager != null && checkpointManager.ConsumeRespawnAfterSceneReload())
        {
            PauseMenu.ForceResumeState();
            RespawnPlayer(checkpointManager);
        }
    }

    public void RespawnAtCheckpoint()
    {
        CheckpointManager checkpointManager = CheckpointManager.Instance;

        Debug.Log("Respawning at checkpoint...");
        if (checkpointManager == null || !checkpointManager.HasCheckpoint())
        {
            Debug.Log("No checkpoint set");
            return;
        }

        // Resume game state before reload/teleport so paused state cannot block gameplay.
        if (pauseMenu != null)
            pauseMenu.Resume();
        else
            PauseMenu.ForceResumeState();

        if (reloadSceneOnRespawn)
        {
            checkpointManager.RequestRespawnAfterSceneReload();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        RespawnPlayer(checkpointManager);
    }

    private void RespawnPlayer(CheckpointManager checkpointManager)
    {
        if (checkpointManager == null || !checkpointManager.HasCheckpoint())
            return;

        if (controller != null)
            controller.enabled = false;

        Vector3 respawnPos = checkpointManager.GetPosition();
        Quaternion respawnRot = checkpointManager.GetRotation();

        Debug.Log($"Respawning player at position: {respawnPos}, rotation: {respawnRot.eulerAngles}");

        if (player != null)
        {
            player.transform.position = respawnPos;
            player.transform.rotation = respawnRot;
        }

        if (controller != null)
            controller.enabled = true;
    }
}