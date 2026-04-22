namespace RealmStudioX.Core
{
    public interface IUndoableCommand : IDisposable
    {
        void Execute();
        void Undo();
    }

}
