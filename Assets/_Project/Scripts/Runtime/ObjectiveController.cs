using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveController : MonoBehaviour
{
    [SerializeField] private TMP_Text objectivesText;

    private List<Objective> objectives = new();

    private void Start()
    {
        AddObjective("- Usar la estación [E] para desbloquear la puerta");
        AddObjective("- Encuentra la estación de vigilancia");
        AddObjective("- Encuentra la llave para escapar");
    }

    public void AddObjective(string text)
    {
        objectives.Add(new Objective(text));
        RefreshUI();
    }

    public void CompleteObjective(string text)
    {
        Objective objective =
            objectives.Find(o => o.text == text);

        if (objective != null)
        {
            objective.completed = true;
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        objectivesText.text = "";

        foreach (Objective objective in objectives)
        {
            if (objective.completed)
                objectivesText.text += $"<s>{objective.text}</s>\n";
            else
                objectivesText.text += $"{objective.text}\n";
        }
    }
}

[System.Serializable]
public class Objective
{
    public string text;
    public bool completed;

    public Objective(string text)
    {
        this.text = text;
        completed = false;
    }
}