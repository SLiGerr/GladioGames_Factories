namespace Source.Code
{
    public interface IBuilding : ICellBehaviour
    {
        int Production { get; set; }

        void AddProduction(int value);
        int TakeProduction(int value);
    }
}