using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class Room : MonoBehaviour
{
    [SerializeField] Stage roomType;
    public Stage RoomType => roomType;

    // Direction Ordering: North South East West
    public Portal[] portals = new Portal[4];

    NavMeshSurface navMeshSurface;
    GenerateEnemies[] spawners;
    private int monsterCount = 0;
    bool cleared = false;

    private void Awake()
    {
        spawners = GetComponentsInChildren<GenerateEnemies>();
        navMeshSurface = GetComponentInChildren<NavMeshSurface>();
    }

    private void Start()
    {
        navMeshSurface.BuildNavMesh();
    }

    void AddCount() => monsterCount++;
    void SubtractCount() => monsterCount--;
    public void EnterRoom()
    {
        if (cleared)
            return;

        Manager.Event.voidEventDic["enemySpawned"].OnEventRaised += AddCount;
        Manager.Event.voidEventDic["enemyDied"].OnEventRaised += SubtractCount;
        SpawnMonsters();
        StartCoroutine(RoomBattleRoutine());
    }

    IEnumerator RoomBattleRoutine()
    {
        while(monsterCount > 0)
        {
            Debug.Log(monsterCount);
            yield return null;
        }
        RoomCleared();
    }

    private void SpawnMonsters()
    {
        foreach (var spawner in spawners)
        {
            spawner.SpawnEnemies();
        }
    }

    private void RoomCleared()
    {
        Debug.Log("room cleared");
        cleared = true;
        Manager.Game.SpawnChest();
        ActivatePortals();
        Manager.Event.voidEventDic["enemySpawned"].OnEventRaised = null;
        Manager.Event.voidEventDic["enemyDied"].OnEventRaised = null;
    }

    public void ActivatePortal(Direction direction)
    {
        portals[(int)direction].gameObject.SetActive(true);
    }

    public void ActivatePortals()
    {
        foreach(Portal portal in portals)
        {
            if(portal.destination != null)
            {
                portal.gameObject.SetActive(true);
            }
        }
    }

    public void ConnectPortal(Direction direction, Portal neighbor)
    {
        portals[(int)direction].destination = neighbor;
    }
}