using System;

[Serializable]
public struct DialogueLine
{
    public SpeakerSO speaker;
    public string text;
    public bool autoAdvance;
    public float autoAdvanceDelay;
    public string onStartEvent;
    public string onEndEvent;
}
