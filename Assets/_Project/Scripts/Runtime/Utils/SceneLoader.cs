using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static bool TryLoadScene(string sceneName, Object context)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("Scene name is not assigned in the inspector.", context);
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' cannot be loaded. Check that it exists and is added to Build Settings.", context);
            return false;
        }

        SceneManager.LoadScene(sceneName);
        return true;
    }
}
