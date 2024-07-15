using VillageProject.Core.DIM;

namespace VillageProject.Core.Reservations;

/// <summary>
/// Interface for any Inst or Comp which uses the Reservation system.
/// The same interface is used for both source and target of reservations.
/// </summary>
public interface IReservable
{
    public string Id { get; }
    public Result TryReserve(Reservation reservation);
    public void EndReservation(Reservation reservation);
}