using System.Collections.Generic;

public static class GlobalGameManager
{
    public static List<PlayerRes> PlayersResList = new List<PlayerRes>();
    public static void Add(PlayerRes res)
    {
        PlayersResList.Add(res);
    }
}