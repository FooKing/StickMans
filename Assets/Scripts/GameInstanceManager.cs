using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameInstanceManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject stickPrefab;
    public List<GameObject> spawnedStickmen;

    

    void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            print("K pressed");
            SpawnPlayers();
        }
    }
    
    
    private void SpawnPlayers()
    {
        
        //Shuffle list then spawn.
        List<string> templist = uiManager.retrievedNamesList;

        for (int i = 0; i < templist.Count; i++)
        {
            string temp = templist[i];
            int randomIndex = Random.Range(i, templist.Count);
            templist[i] = templist[randomIndex];
            templist[randomIndex] = temp;
        }

        for (int i = 0; i < uiManager.retrievedNamesList.Count; i++)
        {
            GameObject currentPlayer = Instantiate(stickPrefab,new Vector3((i -8f) * 1f,0f,0f), quaternion.identity);
            StickmanBalance currentStickScript = currentPlayer.GetComponent<StickmanBalance>();
            currentStickScript.stickname = uiManager.retrievedNamesList[i];
            currentStickScript.name = uiManager.retrievedNamesList[i];
            spawnedStickmen.Add(currentPlayer);
        }

        foreach (var go in spawnedStickmen)
        {
            go.GetComponent<StickmanBalance>().addEnemiesToList(spawnedStickmen);
        }
        

    }

 
}
