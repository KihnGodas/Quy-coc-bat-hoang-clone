using System;
using UnityEngine;

public sealed class PlayerExperience : MonoBehaviour
{
    [SerializeField, Min(0f)] private float currentExperience;
    [SerializeField, Min(1)] private int level = 1;
    [SerializeField, Min(1f)] private float baseExperienceToNextLevel = 35f;
    [SerializeField, Min(1.01f)] private float experienceGrowth = 1.15f;
    [SerializeField] private PlayerCultivationState cultivationState;

    public int Level => level;
    public float CurrentExperience => currentExperience;
    public float ExperienceToNextLevel => GetExperienceToNextLevel(level);
    public bool CanUseUltimate => cultivationState != null && cultivationState.CanUseUltimate;

    public event Action<int> OnLevelChanged;
    public event Action<float, float> OnExperienceChanged;
    public event Action<CultivationRealm> OnRealmChanged;

    private void Awake()
    {
        ResolveReferences();
        SyncCultivationState();
        RaiseExperienceChanged();
    }

    private void Update()
    {
        ResolveReferences();
    }

    public void AddExperience(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentExperience += amount;
        bool leveledUp = false;

        while (currentExperience >= ExperienceToNextLevel)
        {
            currentExperience -= ExperienceToNextLevel;
            level++;
            leveledUp = true;
            OnLevelChanged?.Invoke(level);
            SyncCultivationState();
            OnRealmChanged?.Invoke(GetRealmForLevel(level));
        }

        if (leveledUp)
        {
            RaiseExperienceChanged();
        }
        else
        {
            RaiseExperienceChanged();
        }
    }

    public void SetProgress(int newLevel, float newCurrentExperience)
    {
        level = Mathf.Max(1, newLevel);
        currentExperience = Mathf.Max(0f, newCurrentExperience);
        SyncCultivationState();
        OnLevelChanged?.Invoke(level);
        OnRealmChanged?.Invoke(GetRealmForLevel(level));
        RaiseExperienceChanged();
    }

    private void SyncCultivationState()
    {
        if (cultivationState == null)
        {
            return;
        }

        CultivationRealm realm = GetRealmForLevel(level);
        cultivationState.SetRealm(realm);
    }

    private void ResolveReferences()
    {
        if (cultivationState == null)
        {
            cultivationState = GetComponent<PlayerCultivationState>();
        }
    }

    private void RaiseExperienceChanged()
    {
        OnExperienceChanged?.Invoke(currentExperience, ExperienceToNextLevel);
    }

    private float GetExperienceToNextLevel(int currentLevel)
    {
        int effectiveLevel = Mathf.Max(1, currentLevel);
        return Mathf.Max(1f, Mathf.Round(baseExperienceToNextLevel * Mathf.Pow(experienceGrowth, effectiveLevel - 1)));
    }

    private CultivationRealm GetRealmForLevel(int currentLevel)
    {
        if (currentLevel >= 10)
        {
            return CultivationRealm.KimDan;
        }

        if (currentLevel >= 5)
        {
            return CultivationRealm.TrucCo;
        }

        return CultivationRealm.LuyenKhi;
    }
}
