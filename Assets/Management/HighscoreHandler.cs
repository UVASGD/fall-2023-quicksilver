using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HighscoreHandler : MonoBehaviour
{
    List<HighscoreElement> highscoreList = new List<HighscoreElement>();
    [SerializeField] int maxCount = 5;
    [SerializeField] string filename;
    [SerializeField] GameObject end;

    public delegate void OnHighscoreListChanged(List<HighscoreElement> list);
    public static event OnHighscoreListChanged onHighscoreListChanged;

    private void Start()
    {
        LoadHighscores(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadHighscores(int level_id)
    {
        List<HighscoreElement> tempList = FileHandler.ReadListFromJSON<HighscoreElement>(filename);
        
        for (int i = 0; i < tempList.Count; i++)
        {
            if (tempList[i].level_id == level_id)
            {
                highscoreList.Add(tempList[i]);
            }
        }

        while (highscoreList.Count > maxCount)
        {
            highscoreList.RemoveAt(maxCount);
        }

        onHighscoreListChanged.Invoke(highscoreList);
    }

    private void SaveHighscore()
    {
        FileHandler.SaveToJSON<HighscoreElement> (highscoreList, filename);
    }

    public void AddHighscoreIfPossible (HighscoreElement element)
    {
        Debug.Log(element.points);
        Debug.Log(element.playerName);
        Debug.Log(element.level_id);
        for (int i = 0; i < maxCount; i++)
        {
            if (i >= highscoreList.Count || element.points > highscoreList[i].points)
            {
                // add new high score
                highscoreList.Insert(i, element);

                while (highscoreList.Count > maxCount)
                {
                    highscoreList.RemoveAt(maxCount);
                }

                SaveHighscore();

                if (onHighscoreListChanged != null)
                {
                    onHighscoreListChanged.Invoke(highscoreList);
                }

                break;
            }
        }
    }
}
