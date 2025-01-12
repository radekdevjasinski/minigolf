using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public List<GameObject> levels = new List<GameObject>();
    public Level activeLevel;
    public static LevelController Instance;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        LoadLevels();
        activeLevel = levels[0].GetComponent<Level>();
        SpawnLevel();
        Game.Instance.Reset();
    }

    private void LoadLevels()
    {
        Object[] loadedObjects = Resources.LoadAll("Levels", typeof(GameObject));

        foreach (Object obj in loadedObjects)
        {
            if (obj is GameObject prefab)
            {
                levels.Add(prefab);
            }
        }

        Debug.Log($"Załadowano {levels.Count} poziomów.");
        if(levels.Count > 0)
        {
            levels.Sort((a, b) =>
        {
            Level levelA = a.GetComponent<Level>();
            Level levelB = b.GetComponent<Level>();

            if (levelA == null || levelB == null)
                return 100; 

            return levelA.name.CompareTo(levelB.name);
        });
        }
    }
    public void NextLevel()
    {
        int nextLevel = levels.IndexOf(activeLevel.gameObject) + 1;
        if(nextLevel < levels.Count)
        {
            activeLevel = levels[nextLevel].GetComponent<Level>();
            SpawnLevel();
            activeLevel.ResetLevel();
        }
        else
        {
            activeLevel.ResetLevel();

        }

    }
    public void LoseLevel()
    {
        SpawnLevel();
        activeLevel.ResetLevel();
    }
    public void FirstLevel()
    {
        activeLevel = levels[0].GetComponent<Level>();
        SpawnLevel();
        activeLevel.ResetLevel();
    }
    private void SpawnLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Instantiate(activeLevel.gameObject, this.transform);
        activeLevel.ResetLevel();
    }
}
