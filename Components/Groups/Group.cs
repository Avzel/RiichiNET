namespace RiichiNET.Components.Groups;

using Enums;

internal abstract class Group
{
    internal abstract Mentsu Mentsu { get; }
    internal abstract bool Open { get; }
    internal bool Akadora { get; private protected set; }
    
    internal abstract List<Tile> GetSortedTiles();
    internal abstract bool HasYaoChuu();
    internal abstract bool OnlyHonors();
    internal abstract bool OnlyGreens();
}
