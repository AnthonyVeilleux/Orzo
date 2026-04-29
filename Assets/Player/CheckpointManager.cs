using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private static Vector3 checkpointPosition;
    private static Quaternion checkpointRotation;
    private static bool hasCheckpoint = false;
    private static bool respawnAfterSceneReload = false;

    private void Awake()
    {
        Instance = this;
    }

    public void SetCheckpoint(Vector3 pos, Quaternion rot)
    {
        checkpointPosition = pos;
        checkpointRotation = rot;
        hasCheckpoint = true;

        Debug.Log("Checkpoint saved");
    }

    public bool HasCheckpoint()
    {
        return hasCheckpoint;
    }

    public void RequestRespawnAfterSceneReload()
    {
        respawnAfterSceneReload = true;
    }

    public bool ConsumeRespawnAfterSceneReload()
    {
        if (!respawnAfterSceneReload)
            return false;

        respawnAfterSceneReload = false;
        return true;
    }

    public Vector3 GetPosition() => checkpointPosition;
    public Quaternion GetRotation() => checkpointRotation;
}