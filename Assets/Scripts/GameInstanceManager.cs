using System.Collections;
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
    private bool _gameRunning;
    private int startCountdown = 5;

    

    void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            uiManager.GetComponent<UIDocument>().enabled = false;
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
            currentStickScript.gameInstanceManager = this;
            spawnedStickmen.Add(currentPlayer);
        }

        if (_gameRunning == false)
        {
            _gameRunning = true;
            StartCoroutine(StartGameCountdown(startCountdown));
        }
       

    }

    private IEnumerator StartGameCountdown(int timeToStart)
    {
        int counter = timeToStart;
        while (counter > 0)
        {
            print(counter);
            yield return new WaitForSeconds(1);
            counter--;
        }
        
    }
    


}
