
namespace Assets.Scripts.Logic
{
    internal enum CellSpace
    {
        Empty,
        Bedrock,
        Breakable,
        Flag
    }

    internal class Cell
    {
        public CellSpace Space { get;private set; }
        public Tank Occupant;
        public BreakableObject breakableObject;

        public Cell (CellSpace space, BreakableObject breakableObject = null)
        {
            Space = space;
            this.breakableObject = breakableObject;
        }

        public void Occupy(Tank occupant)
        {
            Occupant = occupant;
        }

        public bool TryToDel()
        {
            if (Space == CellSpace.Breakable || Space == CellSpace.Flag)
            {
                Space = CellSpace.Empty;
                return true;
            }
            return false;
        }
    }
}