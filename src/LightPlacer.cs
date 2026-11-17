public static class LightPlacer
{
    public static void Place(Facility facility)
    {
        for(int i = 0; i < facility.Rooms.Count; i++)
        {
            foreach(LightSource l in facility.Rooms[i].GetMapLightSources())
            {
                facility.LightSources.Add(l);
            }
        }
    }
}