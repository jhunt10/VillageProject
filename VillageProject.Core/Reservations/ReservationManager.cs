using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Reservations;

public class ReservationManager
{
    private Dictionary<string, Reservation> _reservations = new Dictionary<string, Reservation>();

    private static List<string> _makingIds = new List<string>();

    internal static void _CheckIfMaking(string id)
    {
        if (!_makingIds.Contains(id))
            throw new Exception("Reservations can not be made outside of ReservationManager.");
    }

    public Reservation GetReservation(string id)
    {
        if (_reservations.ContainsKey(id))
            return _reservations[id];
        throw new Exception($"Failed to find Reservation with id '{id}'.");
    }

    public TReservable GetReservationSourceAs<TReservable>(string reservationId)
    {
        var reservation = GetReservation(reservationId);
        // Source is a component
        if (reservation.SourceId.Contains(":"))
        {
            var sourceComp = DimMaster.GetCompAsTypeById<ICompInst>(reservation.SourceId, errorIfNull:true);
            if (sourceComp is TReservable)
                return (TReservable)sourceComp;
            throw new Exception($"Failed to cast ReservationSource '{reservation.SourceId}' as {typeof(TReservable).FullName}.");
        }
        var sourceInst = DimMaster.GetInstById(reservation.SourceId, errorIfNull:true);
        if (sourceInst is TReservable)
            return (TReservable)sourceInst;
        throw new Exception($"Failed to cast ReservationSource '{reservation.SourceId}' as {typeof(TReservable).FullName}.");
    }
    
    public TReservable GetReservationTargetAs<TReservable>(string reservationId)
        where TReservable : IReservable
    {
        var reservation = GetReservation(reservationId);
        // Source is a component
        if (reservation.TargetId.Contains(":"))
        {
            var targetComp = DimMaster.GetCompAsTypeById<ICompInst>(reservation.TargetId, errorIfNull:true);
            if (targetComp is TReservable)
                return (TReservable)targetComp;
            throw new Exception($"Failed to cast ReservationTarget '{reservation.TargetId}' as {typeof(TReservable).FullName}.");
        }
        var targetInst = DimMaster.GetInstById(reservation.TargetId, errorIfNull:true);
        if (targetInst is TReservable)
            return (TReservable)targetInst;
        throw new Exception($"Failed to cast ReservationTarget '{reservation.TargetId}' as {typeof(TReservable).FullName}.");
    }
    
    /// <summary>
    /// Attempt to make a reservation. If successful,the Result.Message is the Id of the reservation.
    /// </summary>
    /// <param name="source">IReservable making the reservation</param>
    /// <param name="target">IReservable to be reserved</param>
    /// <param name="data">DataDict containing metadata about the reservation</param>
    /// <returns>Result. If successful,the Message is the Id of the reservation.</returns>
    public Result MakeReservations(IReservable source, IReservable target, DataDict? data)
    {
        var dataDict = data ?? new DataDict();
        _makingIds.Add(dataDict.Id);
        var reservation = new Reservation(source, target, dataDict);
        var targetRes = target.TryReserve(reservation);
        if (!targetRes.Success)
            return targetRes;
        _reservations.Add(reservation.Id, reservation);
        return new Result(true, reservation.Id);
    }

    private List<string> _endingReservations = new List<string>(); 
    public void EndReservation(string reservationId)
    {
        if(_endingReservations.Contains(reservationId))
            return;
        
        if(!_reservations.ContainsKey(reservationId))
            return;

        _endingReservations.Add(reservationId);
        var reservation = _reservations[reservationId];
        
        try
        {
            var target = GetReservationTargetAs<IReservable>(reservationId);
            target.EndReservation(reservation);
        }
        catch (Exception e)
        {
            
        }
        
        try
        {
            var source = GetReservationSourceAs<IReservable>(reservationId);
            source.EndReservation(reservation);
        }
        catch (Exception e)
        {
            
        }

        _reservations.Remove(reservation.Id);
        _endingReservations.Remove(reservationId);
    }
}