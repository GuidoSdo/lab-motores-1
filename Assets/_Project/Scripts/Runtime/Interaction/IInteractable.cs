using UnityEngine;

/// <summary>
/// Contrato para objetos que pueden ser accionados por el jugador u otro interactor.
/// </summary>
public interface IInteractable
{
    void Interact(GameObject interactor);
}
