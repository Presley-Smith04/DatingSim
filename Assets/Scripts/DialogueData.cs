using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    [TextArea] public string girlDialogue;
    public string[] playerChoices = new string[4];
    public int[] choicePoints = new int[4]; // Matches choices, 0-2 points
    public string[] girlResponses = new string[4]; // Slight reaction variations
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "DatingSim/Dialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueEntry[] dateDialogue; // Each line of conversation for the date
}