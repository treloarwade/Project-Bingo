using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DingoSystem;

public static class Loader
{
    public enum Scene
    {
        Battle,
        SampleScene
    }

    public static void Load(Scene scene, List<DingoID> loadeddingos = null, bool isTrainer = false)
    {
        switch (scene)
        {
            case Scene.Battle:
                // Store the list of dingos in a global variable or some data holder accessible to the Battle scene
                BattleManager.SetRandomDingos(loadeddingos);
                BattleManager.SetTrainerDingos(isTrainer);
                UnityEngine.SceneManagement.SceneManager.LoadScene(scene.ToString());
                break;
            default:
                // Load other scenes without passing any additional parameters
                UnityEngine.SceneManagement.SceneManager.LoadScene(scene.ToString());
                break;
        }
    }
}

