using UnityEngine;

public sealed class CombatTimer
{
    private float duration;
    private float elapsed;
    private bool isRunning;

    public float Duration => duration;
    public float Elapsed => elapsed;
    public float Remaining => Mathf.Max(duration - elapsed, 0f);
    public bool IsRunning => isRunning;
    public bool IsComplete => isRunning && elapsed >= duration;

    public void Start(float combatDuration)
    {
        duration = Mathf.Max(0f, combatDuration);
        elapsed = 0f;
        isRunning = duration > 0f;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void Tick(float deltaTime)
    {
        if (!isRunning)
        {
            return;
        }

        elapsed = Mathf.Min(elapsed + Mathf.Max(deltaTime, 0f), duration);
    }
}
