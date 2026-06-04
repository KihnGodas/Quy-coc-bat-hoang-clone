using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public sealed class EnemyExperienceDropper : MonoBehaviour
{
    [SerializeField] private EnemyBase enemyBase;
    [SerializeField] private Health health;
    [SerializeField] private SimpleHealth simpleHealth;
    [SerializeField, Min(0f)] private float experienceMultiplier = 1f;
    [SerializeField] private Vector2 spawnOffset = new Vector2(0.2f, 0.15f);

    private bool dropped;

    private void Awake()
    {
        ResolveReferences();
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void OnEnemyDataChanged(EnemyData enemyData)
    {
        ResolveReferences();
    }

    private void HandleDeath()
    {
        if (dropped)
        {
            return;
        }

        dropped = true;

        float experience = GetExperienceReward();
        Vector2 offset = Random.insideUnitCircle;
        if (offset.sqrMagnitude > 0.0001f)
        {
            offset.Normalize();
        }

        Vector3 spawnPosition = transform.position + (Vector3)(offset * spawnOffset);
        ExperienceOrb.Spawn(spawnPosition, experience);
    }

    private float GetExperienceReward()
    {
        if (enemyBase == null || enemyBase.Data == null)
        {
            return 8f;
        }

        return Mathf.Max(1f, enemyBase.Data.ExperienceReward * Mathf.Max(0f, experienceMultiplier));
    }

    private void ResolveReferences()
    {
        if (enemyBase == null)
        {
            enemyBase = GetComponent<EnemyBase>();
        }

        if (health == null)
        {
            health = GetComponent<Health>();
        }

        if (simpleHealth == null)
        {
            simpleHealth = GetComponent<SimpleHealth>();
        }
    }

    private void Subscribe()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
            health.OnDeath += HandleDeath;
        }

        if (simpleHealth != null)
        {
            simpleHealth.OnDeath -= HandleDeath;
            simpleHealth.OnDeath += HandleDeath;
        }
    }

    private void Unsubscribe()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
        }

        if (simpleHealth != null)
        {
            simpleHealth.OnDeath -= HandleDeath;
        }
    }
}
