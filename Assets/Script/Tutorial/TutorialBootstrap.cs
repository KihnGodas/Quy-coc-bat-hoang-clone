using UnityEngine;

public sealed class TutorialBootstrap : MonoBehaviour
{
    [SerializeField] private ArenaBounds arenaBounds;
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool ensurePlayerExperience = true;
    [SerializeField] private bool ensurePlayerCultivationState = true;
    [SerializeField] private bool ensurePlayerSpellController = true;
    [SerializeField] private bool ensurePlayerUltimateController = true;
    [SerializeField] private bool logBootstrap = true;

    private void Awake()
    {
        ResolveReferences();
        EnsureRuntimePlayerComponents();
    }

    private void Start()
    {
        ResolveReferences();

        if (logBootstrap)
        {
            Debug.Log("Tutorial bootstrap ready.");
        }
    }

    public void ResolveReferences()
    {
        if (arenaBounds == null)
            arenaBounds = FindComponentInScene<ArenaBounds>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void EnsureRuntimePlayerComponents()
    {
        if (player == null) return;

        if (ensurePlayerExperience && player.GetComponent<PlayerExperience>() == null)
            player.gameObject.AddComponent<PlayerExperience>();

        if (ensurePlayerCultivationState && player.GetComponent<PlayerCultivationState>() == null)
            player.gameObject.AddComponent<PlayerCultivationState>();

        if (ensurePlayerSpellController && player.GetComponent<PlayerSpellController>() == null)
            player.gameObject.AddComponent<PlayerSpellController>();

        if (ensurePlayerUltimateController && player.GetComponent<PlayerUltimateController>() == null)
            player.gameObject.AddComponent<PlayerUltimateController>();
    }

    private static T FindComponentInScene<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return FindFirstObjectByType<T>();
#else
        return Object.FindObjectOfType<T>();
#endif
    }
}
