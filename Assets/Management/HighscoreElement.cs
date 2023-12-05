using System;

[Serializable]
public class HighscoreElement 
{
    public string playerName;
    public float points;
    public int level_id;

    
     public HighscoreElement  (string name, int points, int level_id)
    {
        playerName = name;
        this.points = points;
        this.level_id = level_id;
    }
}

