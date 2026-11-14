public enum LootSpotKind
{
    Loose, Container,
}

public readonly record struct LootSpot(int X, int Y, LootSpotKind Kind);