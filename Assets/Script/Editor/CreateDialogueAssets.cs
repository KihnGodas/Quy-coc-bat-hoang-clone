using UnityEditor;
using UnityEngine;

public static class CreateDialogueAssets
{
    [MenuItem("Tools/Create Tutorial Dialogue Assets")]
    public static void CreateTutorialDialogueAssets()
    {
        string dialoguePath = "Assets/ScriptableObjects/Dialogue";
        if (!AssetDatabase.IsValidFolder(dialoguePath))
        {
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Dialogue");
        }

        SpeakerSO playerSpeaker = ScriptableObject.CreateInstance<SpeakerSO>();
        string playerSpeakerPath = $"{dialoguePath}/Speaker_Player.asset";
        AssetDatabase.CreateAsset(playerSpeaker, playerSpeakerPath);
        SerializedObject playerSO = new SerializedObject(playerSpeaker);
        playerSO.FindProperty("speakerName").stringValue = "Tiểu Vũ";
        playerSO.FindProperty("highlightColor").colorValue = new Color(0.25f, 0.65f, 1f, 1f);
        playerSO.FindProperty("dimColor").colorValue = new Color(0.25f, 0.65f, 1f, 0.3f);
        playerSO.ApplyModifiedProperties();

        SpeakerSO guideSpeaker = ScriptableObject.CreateInstance<SpeakerSO>();
        string guideSpeakerPath = $"{dialoguePath}/Speaker_Guide.asset";
        AssetDatabase.CreateAsset(guideSpeaker, guideSpeakerPath);
        SerializedObject guideSO = new SerializedObject(guideSpeaker);
        guideSO.FindProperty("speakerName").stringValue = "Lão Tiên Sinh";
        guideSO.FindProperty("highlightColor").colorValue = new Color(1f, 0.85f, 0.4f, 1f);
        guideSO.FindProperty("dimColor").colorValue = new Color(1f, 0.85f, 0.4f, 0.3f);
        guideSO.ApplyModifiedProperties();

        DialogueSO tutorialDialogue = ScriptableObject.CreateInstance<DialogueSO>();
        string dialogueAssetPath = $"{dialoguePath}/TutorialOpening.asset";
        AssetDatabase.CreateAsset(tutorialDialogue, dialogueAssetPath);
        SerializedObject dialogueSO = new SerializedObject(tutorialDialogue);
        SerializedProperty linesProp = dialogueSO.FindProperty("lines");
        linesProp.ClearArray();

        string[] texts = new string[]
        {
            "Chào mừng con đến với thế giới tu tiên! Ta là Lão Tiên Sinh, người sẽ hướng dẫn con những bước đầu tiên.",
            "Dạ thưa tiên sinh! Con rất háo hức được bắt đầu hành trình tu luyện.",
            "Tốt lắm! Trước hết, hãy di chuyển bằng các phím WASD hoặc phím mũi tên. Hãy thử đi đến phía trước mặt.",
            "Vâng ạ! Con sẽ làm theo lời tiên sinh."
        };

        linesProp.arraySize = texts.Length;
        for (int i = 0; i < texts.Length; i++)
        {
            SerializedProperty line = linesProp.GetArrayElementAtIndex(i);
            line.FindPropertyRelative("speaker").objectReferenceValue = i % 2 == 0 ? guideSpeaker : playerSpeaker;
            line.FindPropertyRelative("text").stringValue = texts[i];
        }
        dialogueSO.ApplyModifiedProperties();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created dialogue assets:\n  {playerSpeakerPath}\n  {guideSpeakerPath}\n  {dialogueAssetPath}");

        Selection.activeObject = tutorialDialogue;
        EditorGUIUtility.PingObject(tutorialDialogue);
    }
}
