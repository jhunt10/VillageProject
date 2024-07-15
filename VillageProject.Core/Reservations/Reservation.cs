using VillageProject.Core.DIM;

namespace VillageProject.Core.Reservations;

public class Reservation
{
    public string Id { get; }
    public string SourceId { get; }
    public string TargetId { get; }
    public DataDict Data { get; }

    public Reservation(IReservable source, IReservable target, DataDict data)
    {
        SourceId = source.Id ?? throw new ArgumentNullException(nameof(source));
        TargetId = target.Id ?? throw new ArgumentNullException(nameof(target));
        Data = data ?? throw new ArgumentNullException(nameof(data));
        Id = data.Id;
        ReservationManager._CheckIfMaking(Id);
    }
}