using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Mantiene la lista de objetivos activos y actualiza su representacion en pantalla.
/// </summary>
public class ObjectiveController : MonoBehaviour
{
    [Tooltip("Texto de UI donde se muestran los objetivos actuales.")]
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

/// <summary>
/// Estado serializable de un objetivo mostrado al jugador.
/// </summary>
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
