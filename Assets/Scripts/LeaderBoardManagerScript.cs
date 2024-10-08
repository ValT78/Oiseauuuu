using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeaderBoardManagerScript : MonoBehaviour
{

    public static LeaderBoardManagerScript Instance { get; private set; }
    string leaderboardKey = "learderboardKeyVTCT";
    public TextMeshProUGUI playerNameTMP;
    public TextMeshProUGUI leaderBoard;
    public TMP_InputField nameInputField;

    public Canvas leaderBoardCanvas;

    [HideInInspector]
    public string playerName;

    public float timeBetweenScoreUpdate = 10f;
    private float timer;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        timer = 3f;
    }


    void Start()
    {
        Login();
        GetLeaderBoard();

    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            print("Updating leaderboard");
            timer = timeBetweenScoreUpdate;
            GetLeaderBoard();
        }
    }

    public void Login()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                // Debug.Log("error starting LootLocker session");
                return;
            }

            if (response.player_name == "")
            {
                _SetPlayerName(RandomPlayerName());
            }
            else
            {
                playerName = response.player_name;
                playerNameTMP.text = "Your name : " + playerName;
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
                //Debug.Log(response.errorData.ToString());
                return;
            }
            Debug.Log("Successfully submitted score!");

        });

    }

    public void GetLeaderBoard()
    {
        int count = 10;

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
                        string tempName = members[i].player.name;
                        if (tempName.Length > 15)
                        {
                            tempName = tempName.Substring(0, 15);
                        }
                        tempString += tempName+": ";
                    }
                    else
                    {
                        tempString += members[i].player.id;
                    }
                    tempString += members[i].score + " citizens \n";
                }
                leaderBoard.text = tempString;
            }
            else
            {
                //Debug.Log("Failed" + response.errorData);
            }
        });

    }

    public string RandomPlayerName()
    {
        string[] baseFunnyWords = { "Gamer", "Prince", "King", "Queen", "Lord", "Mayor", "President", "Pope", "Captain" };
        string[] baseFunnyAdjectif1 = { "Mini", "Funny", "Crazy", "Silly", "Smart", "Wise", "Clever", "Friendly" };
        string[] baseFunnyAdjectif2 = {"Big", "Small", "Tiny", "Huge", "Giant", "Enormous", "Gigantic", "Colossal"};

        string funny_name = baseFunnyAdjectif1[Random.Range(0, baseFunnyAdjectif1.Length)] + baseFunnyAdjectif2[Random.Range(0, baseFunnyAdjectif2.Length)] + baseFunnyWords[Random.Range(0, baseFunnyWords.Length)];
        Debug.Log("Generated name : " + funny_name);
        return funny_name;
    }

    public void OnInputFieldEnd(string _)
    {
        name = nameInputField.text; 
        _SetPlayerName(name);
    }

    public void _SetPlayerName(string name)
    {

        LootLockerSDKManager.SetPlayerName(name, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Succesfully set player name");
            }
            else
            {
                Debug.Log("Could not set player name");
                //Debug.Log(response.errorData.ToString());
                return;
            }
        });

        Debug.Log("Changing name to " + name);
        playerName = name;
        playerNameTMP.text = "Your name : " + playerName;

        GetLeaderBoard();

    }

}
