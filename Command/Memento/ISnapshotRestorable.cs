namespace Command.Memento;

public interface ISnapshotRestorable
{
    ISnapshot CreateSnapshot();
    Task RestoreSnapshot(ISnapshot snapshot, CancellationToken cancellationToken = default);
}