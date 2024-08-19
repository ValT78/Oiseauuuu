using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;
using UnityEditor;
using TMPro;

public class LeaderBoardManagerScript : MonoBehaviour
{

    public static LeaderBoardManagerScript Instance { get; private set; }
    string leaderboardKey = "learderboardKeyVTCT";
    public TextMeshProUGUI playerNames;
    public TextMeshProUGUI playerScores;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        Login();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Getting score");
            GetLeaderBoard();

        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Publishing score");
            PublishScore(Random.Range(0, 1000));
        }
    }

    public void Login()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");

                return;
            }

            Debug.Log("successfully started LootLocker session");
        });
    }

    public void PublishScore(int score)
    {

        LootLockerSDKManager.SubmitScore("", score, leaderboardKey, (response) =>
        {
            if (!response.success)
            {
                Debug.Log("Could not submit score!");
                Debug.Log(response.errorData.ToString());
                return;
            }
            Debug.Log("Successfully submitted score!");

        });
    }

    public void GetLeaderBoard()
    {
        bool done = false;
        int count = 50;

        LootLockerSDKManager.GetScoreList(leaderboardKey, count, 0, (response) =>
        {
            if (response.success)
            {
                string tempString = "";

                LootLockerLeaderboardMember[] members = response.items;

                for (int i = 0; i < members.Length; i++)
                {
                    tempString += members[i].rank + ". ";
                    if (members[i].player.name != "")
                    {
                        tempString += members[i].player.name;
                    }
                    else
                    {
                        tempString += members[i].player.id;
                    }
                    tempString += " Score : " + members[i].score + "\n";
                    tempString += "\n";
                }
                done = true;
                playerNames.text = tempString;
                playerScores.text = ";";
            }
            else
            {
                Debug.Log("Failed" + response.errorData);
                done = true;
            }
        });

    }

}
