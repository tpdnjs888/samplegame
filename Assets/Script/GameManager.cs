using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MainCharacter mainChar;
    public GameObject monsterObjects;

    public Transform targetPositionObj;
    public List<Transform> subCharPosition;
    public List<SubCharacter> subChar;
    public List<Monster> monsterList;

    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
                if(_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "GameManager";
                    _instance = obj.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        for(int i = 0; i < 10; i++)
            InstantiateMonster();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPosition = Input.mousePosition;

            Vector3 viewPort = Camera.main.ScreenToViewportPoint(clickPosition);
            if (viewPort.x < 0 || viewPort.x > 1 || viewPort.y < 0 || viewPort.y > 1)
                return;

            mainChar.StopMove();
            foreach (SubCharacter subCharObj in subChar)
                subCharObj.StopMove();
            
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(clickPosition);
            targetPosition.z = 0f;

            Vector3 dist = targetPosition - mainChar.transform.position;
            float angle = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg;
            angle += -90;
            targetPositionObj.position = targetPosition;
            targetPositionObj.rotation = Quaternion.identity;
            targetPositionObj.Rotate(0f, 0f, angle);

            mainChar.StartMove(targetPosition);
            for(int i = 0; i < subChar.Count; i++)
                subChar[i].StartMove(subCharPosition[i].position);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            SpawnMonster();
    }

    private void SpawnMonster()
    {
        int monsterIndex = -1;
        for (int i = 0; i < monsterList.Count; i++)
        {
            if (monsterList[i].gameObject.activeSelf == false)
            {
                monsterIndex = i;
                break;
            }
        }

        float spawnViewportX = Random.Range(0f, 1f);
        float spawnViewportY = Random.Range(0f, 1f);
        Vector2 spawnPosition = Camera.main.ViewportToWorldPoint(new Vector2(spawnViewportX, spawnViewportY));

        Monster newMonster = null;
        if (monsterIndex == -1)
            newMonster = InstantiateMonster();
        else
            newMonster = monsterList[monsterIndex];

        newMonster.transform.position = spawnPosition;
        newMonster.Spawn();
    }

    private Monster InstantiateMonster()
    {
        Object monsterPrefab = Resources.Load("Prefab/Monster");

        GameObject newMonster = Instantiate(monsterPrefab, Vector3.zero, Quaternion.identity, monsterObjects.transform) as GameObject;
        newMonster.SetActive(false);

        Monster monsterComponent = newMonster.GetComponent<Monster>();
        monsterList.Add(monsterComponent);

        return monsterComponent;
    }
}
