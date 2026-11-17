public class FacilityGenerator
{
    private readonly List<RoomTemplate> _templates;
    private const int MinRooms = 750;
    private const int MaxAttempts = 250;
    private const int TargetRooms = 1500;
    private const int LoopMaxDistance = 100;
    private const double LoopChance = 1.0;

    public FacilityGenerator(List<RoomTemplate> templates)
    {
        _templates = templates;
    }

    public Facility Generate(int width, int height, int seed)
    {
        Random rng = new(seed);
        Facility? biggestFacility = null;
        
        for(int i = 0; i < MaxAttempts; i++)
        {
            Facility facility = TryGenerate(width, height, rng);
            if(facility.Rooms.Count >= MinRooms) return Finish(facility, rng);
            
            if(biggestFacility == null || facility.Rooms.Count > biggestFacility.Rooms.Count) biggestFacility = facility;
        }
        return Finish(biggestFacility!, rng);
    }

    public Facility CreateCamp(int width, int height, int seed)
    {
        Random rng = new(seed);
        TileMap map = new(width, height);
        RoomTemplate template = RoomLibary.GetCamp();
        PlacedRoom room = new(template, 50, 50);
        Stamp(map, template, room.X, room.Y);
        int eX = template.ExtractionPoint.TileX;
        int eY = template.ExtractionPoint.TileY;
        eX += room.X;
        eY += room.Y;
        Console.WriteLine($"Extraction: {eX},{eY}");
        Facility facility = new(map, [room])
        {
            ExtractionPoint = new(eX, eY)
        };
        return Finish(facility, rng);
    }

    private Facility TryGenerate(int width, int height, Random rng)
    {
        TileMap map = new(width, height);
        List<PlacedRoom> placed = new();

        RoomTemplate startTemplate = _templates.First(t => t.Name == "start");
        PlacedRoom start = new(startTemplate, width / 2 - startTemplate.Width / 2, height/2 - startTemplate.Height / 2);
        int eX = startTemplate.ExtractionPoint.TileX;
        int eY = startTemplate.ExtractionPoint.TileY;
        eX += start.X;
        eY += start.Y;
        Stamp(map, start.Template, start.X, start.Y);
        placed.Add(start);

        List<Connector> openDoors = [.. start.GetMapConnectors()];
        List<Connector> failedDoors = [];
        while(placed.Count < TargetRooms && openDoors.Count > 0)
        {
            int randomNumber = rng.Next(openDoors.Count);
            Connector door = openDoors[randomNumber];
            openDoors.RemoveAt(randomNumber);
            PlacedRoom? newRoom = TryAttachRoom(map, door, placed, rng);
            if(newRoom == null)
            {
                failedDoors.Add(door);
                continue;
            }
            placed.Add(newRoom);
            (int dx, int dy) = door.Direction.Offset();
            int usedX = door.X + dx;
            int usedY = door.Y + dy;
            foreach(Connector c in newRoom.GetMapConnectors())
            {
                if(c.X == usedX && c.Y == usedY) continue;
                openDoors.Add(c);
            }
        }
        List<Connector> leftOverDoors = [.. openDoors, .. failedDoors];
        int loops = ConnectLeftoverDoors(map, leftOverDoors, rng);
        SealDoors(map, leftOverDoors);
        Console.WriteLine($"Rooms: {placed.Count}, Loops: {loops}");
        return new Facility(map, placed)
        {
            ExtractionPoint = new(eX, eY)
        };
    }

    private Facility Finish(Facility facility, Random rng)
    {
        LootPlacer.Place(facility, rng);
        LightPlacer.Place(facility);
        return facility;
    }
    private void SealDoors(TileMap map, List<Connector> doors)
    {
        foreach(Connector c in doors)
        {
            (int dx, int dy) = c.Direction.Offset();
            int frontX = c.X + dx;
            int frontY = c.Y + dy;
            if(map.IsWall(frontX, frontY))
            {
                map.SetTile(c.X, c.Y, Tile.Wall);
            }
        }
    }

    private PlacedRoom? TryAttachRoom(TileMap map, Connector openDoor, List<PlacedRoom> placed, Random rng)
    {
        Direction needed = openDoor.Direction.Opposite();
        List<RoomTemplate> candidates = Shuffled(_templates.Where(t => t.Name != "start"), rng);
        foreach(var template in candidates)
        {
            foreach(Connector c in template.Connectors.Where(c => c.Direction == needed))
            {
                (int ox, int oy) = ComputeOrigin(openDoor, c);
                if(!CanPlace(map, template, ox, oy, placed)) continue;
                Stamp(map, template, ox, oy);
                return new(template, ox, oy);
            }
        }
        return null;
    }

    private bool CanPlace(TileMap map, RoomTemplate template, int originX, int originY, List<PlacedRoom> placed)
    {
        bool insideMap = originX > 2 && originY > 2 && originX + template.Width + 2 < map.Width && originY + template.Height + 2 < map.Height;
        if(!insideMap) return false;
        Room room = new(originX, originY, template.Width, template.Height);
        foreach(var other in placed)
        {
            if(room.Overlaps(other.Bounds, 0)) return false;
        }
        return true;
    }

    private (int ox, int oy) ComputeOrigin(Connector openDoor, Connector newDoor)
    {
        (int dx, int dy) = openDoor.Direction.Offset();

        int targetX = openDoor.X + dx;
        int targetY = openDoor.Y + dy;

        return(targetX - newDoor.X, targetY - newDoor.Y);
    }

    private void Stamp(TileMap map, RoomTemplate template, int originX, int originY)
    {
        for (int x = 0; x < template.Width; x++)
        {
            for (int y = 0; y < template.Height; y++)
            {
                map.SetTile(originX + x, originY + y, template.Tiles[x, y]);
            }
        }
    }

    private List<(int x, int y)> BuildLPath((int x, int y) from, (int x, int y) to, bool horizontalFirst)
    {
        List<(int x, int y)> steps = [];

        (int x, int y) corner = horizontalFirst ? (to.x, from.y) : (from.x, to.y);
        (int x, int y) pos = from;
        steps.Add(pos);

        while(pos != corner)
        {
            pos = (pos.x + Math.Sign(corner.x - pos.x), pos.y + Math.Sign(corner.y - pos.y));
            steps.Add(pos);
        }
        while(pos != to)
        {
            pos = (pos.x + Math.Sign(to.x - pos.x), pos.y + Math.Sign(to.y - pos.y));
            steps.Add(pos);
        }
        return steps;
    }

    private bool IsPathClear(TileMap map, List<(int x, int y)> path, Connector doorA, Connector doorB)
    {
        (int x, int y) tileA = (doorA.X, doorA.Y);
        (int x, int y) tileB = (doorB.X, doorB.Y);

        foreach(var step in path)
        {
            bool insideMap = step.x >= 1 && step.y >= 1
            && step.x <= map.Width - 2 && step.y <= map.Height - 2;
            if(!insideMap) return false;
            if(!map.IsWall(step.x, step.y)) return false;

            foreach(Direction dir in Enum.GetValues<Direction>())
            {
                (int dx, int dy) = dir.Offset();
                (int x, int y) neighbor = (step.x + dx, step.y + dy);
                if(path.Contains(neighbor) || neighbor == tileA || neighbor == tileB) continue;
                if(!map.IsWall(neighbor.x, neighbor.y)) return false;
            }
        }
        return true;
    }

    private int ConnectLeftoverDoors(TileMap map, List<Connector> doors, Random rng)
    {
        int created = 0;

        foreach (Connector a in Shuffled(doors, rng))
        {
            if(IsConnected(map, a)) continue;
            if(rng.NextDouble() > LoopChance) continue;
            (int x, int y) frontA = FrontOf(a);
            List<Connector> candidates = doors
                .Where(b => b != a && !IsConnected(map, b))
                .Where(b => ManhattanDistance(frontA, FrontOf(b)) <= LoopMaxDistance)
                .OrderBy(b => ManhattanDistance(frontA, FrontOf(b)))
                .ToList();
            if(TryConnect(map, a, candidates)) created++;
        }
        return created;
    }

    private bool TryConnect(TileMap map, Connector a, List<Connector> candidates)
    {
        (int x, int y) frontA = FrontOf(a);
        foreach(Connector b in candidates)
        {
            (int x, int y) frontB = FrontOf(b);
            foreach(bool horozontalFirst in new[] {true, false })
            {
                List<(int x, int y)> path = BuildLPath(frontA, frontB, horozontalFirst);
                if(!IsPathClear(map, path, a, b)) continue;
                foreach((int x, int y) tile in path)
                {
                    map.SetTile(tile.x, tile.y, Tile.Floor);
                }
                return true;
            }
        }
        return false;
    }

    private bool IsConnected(TileMap map, Connector door)
    {
        (int x, int y) front = FrontOf(door);
        return !map.IsWall(front.x, front.y);
    }

    private static int ManhattanDistance((int x, int y) a, (int x, int y) b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

    private static List<T> Shuffled<T>(IEnumerable<T> items, Random rng)
    {
        return items.OrderBy(_ => rng.Next()).ToList();
    }

    private static (int x, int y) FrontOf(Connector door)
    {
        (int dx, int dy) = door.Direction.Offset();
        return (door.X + dx, door.Y + dy);
    }
}