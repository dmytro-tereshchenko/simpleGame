using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class Initial : MonoBehaviour
{
    private System.Random rand = new System.Random();
    private int countEnemies = 30;
    private int countHpPotions = 10;
    private int countMaxHpPotions = 3;
    /*private UnityEngine.Object enemyPrefab;
    private UnityEngine.Object hpPotionPrefab;
    private UnityEngine.Object maxHpPotionPrefab;*/
    private GameObject enemyPrefab;
    private GameObject hpPotionPrefab;
    private GameObject maxHpPotionPrefab;
    private GameObject player;
    private float enemySpawnTime = 30;
    private float potionSpawnTime = 60;
    private float enemySpawn;
    private float potionSpawn;
    private List<GameObject> enemies;
    private List<GameObject> hpPotions;
    private List<GameObject> maxHpPotions;

    // Start is called before the first frame update
    void Start()
    {
        /*enemyPrefab = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath("Assets/Sceleton.prefab", typeof(GameObject));
        hpPotionPrefab = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath("Assets/RPG Pack/Prefabs/Bottle_Health.prefab", typeof(GameObject));
        maxHpPotionPrefab = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath("Assets/RPG Pack/Prefabs/Bottle_Endurance.prefab", typeof(GameObject));*/
        enemyPrefab = Resources.Load<GameObject>("Sceleton") as GameObject;
        hpPotionPrefab = Resources.Load<GameObject>("Bottle_Health") as GameObject;
        maxHpPotionPrefab = Resources.Load<GameObject>("Bottle_Endurance") as GameObject;
        player = GameObject.Find("Knight");
        enemies = new List<GameObject>();
        hpPotions = new List<GameObject>();
        maxHpPotions = new List<GameObject>();
        for (int i = 1; i <= countEnemies; i++)
        {
            GenerateEnemy();
        }
        for (int i = 1; i <= countHpPotions; i++)
        {
            GenerateHpPotion();
        }
        for (int i = 1; i <= countMaxHpPotions; i++)
        {
            GenerateMaxHpPotion();
        }
    }

    public bool DestroyEnemy(GameObject enemy)
    {
        if (enemy != null && enemies.Remove(enemy))
        {
            Destroy(enemy);
            return true;
        }
        return false;
    }

    public bool DestroyHpPotion(GameObject hp)
    {
        if (hp != null && hpPotions.Remove(hp))
        {
            Destroy(hp);
            return true;
        }
        return false;
    }

    public bool DestroyMaxHpPotion(GameObject hp)
    {
        if (hp != null && maxHpPotions.Remove(hp))
        {
            Destroy(hp);
            return true;
        }
        return false;
    }

    private void GenerateEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab) as GameObject;
        enemy.transform.position = new Vector3(rand.Next(-80, 80), 0, rand.Next(-80, 80));
        enemy.GetComponent<EnemyFolow>().target = player.transform;
        enemy.GetComponent<EnemyFolow>().speed = 5;
        enemy.GetComponent<EnemyFolow>().maxHp = 50;
        enemy.GetComponent<EnemyFolow>().xp = 30;
        enemies.Add(enemy);
    }

    private void GenerateHpPotion()
    {
        GameObject hp = Instantiate(hpPotionPrefab) as GameObject;
        hp.transform.position = new Vector3(rand.Next(-80, 80), 0, rand.Next(-80, 80));
        hpPotions.Add(hp);
    }

    private void GenerateMaxHpPotion()
    {
        GameObject hp = Instantiate(maxHpPotionPrefab) as GameObject;
        hp.transform.position = new Vector3(rand.Next(-80, 80), 0, rand.Next(-80, 80));
        maxHpPotions.Add(hp);
    }

    private void Awake()
    {
        enemySpawn = float.PositiveInfinity;
        potionSpawn = float.PositiveInfinity;
        StartGenerateEnemy(enemySpawnTime);
        StartGeneratePotions(potionSpawnTime);
    }

    public void StartGenerateEnemy(float delay)
    {
        enemySpawn = Time.time + delay;
        StartCoroutine(RunTimerGenerateEnemy());
    }

    public void StartGeneratePotions(float delay)
    {
        potionSpawn = Time.time + delay;
        StartCoroutine(RunTimerGeneratePotions());
    }

    private IEnumerator RunTimerGenerateEnemy()
    {
        while (Time.time < enemySpawn) yield return null;
        if(enemies.Count< countEnemies)
        {
            GenerateEnemy();
        }
        enemySpawn = float.PositiveInfinity;
        StartGenerateEnemy(enemySpawnTime);
    }

    private IEnumerator RunTimerGeneratePotions()
    {
        while (Time.time < potionSpawn) yield return null;
        if (maxHpPotions.Count < countMaxHpPotions &&
            (float)hpPotions.Count / (float)maxHpPotions.Count > (float)countHpPotions / (float)countMaxHpPotions)
        {
            GenerateMaxHpPotion();
        }
        else if (hpPotions.Count < countHpPotions)
        {
            GenerateHpPotion();
        }
        potionSpawn = float.PositiveInfinity;
        StartGeneratePotions(potionSpawnTime);
    }
}
