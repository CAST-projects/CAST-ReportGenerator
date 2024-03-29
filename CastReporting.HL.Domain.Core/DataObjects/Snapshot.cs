namespace CastReporting.HL.Domain;


// Modèle de l'entité HL correspondant à un snapshot d'analyse
public class Snapshot : IComparable<Snapshot>, IEquatable<Snapshot>
{
    static readonly DateTime EPOCH = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public long snapshotDate { get; set; }
    public DateTime SnapshotDate => EPOCH + new TimeSpan(0, 0, (int)(snapshotDate / 1000));

    public string SnapshotLabel { get; set; } = string.Empty;

    public int CompareTo(Snapshot? other) =>
        snapshotDate.CompareTo(other?.snapshotDate ?? long.MinValue);

    public bool Equals(Snapshot? other) =>
        other?.snapshotDate == snapshotDate && other?.SnapshotLabel == SnapshotLabel;

    public override bool Equals(object? obj) =>
        obj is Snapshot && Equals((Snapshot)obj);

    public static bool operator ==(Snapshot? x, object? y) =>
         x?.Equals(y) ?? (y == null);

    public static bool operator !=(Snapshot? x, object? y) =>
        !(x == y);

    public override int GetHashCode() => HashCode.Combine(snapshotDate, SnapshotLabel);

    public override string ToString() => string.IsNullOrWhiteSpace(SnapshotLabel)
        ? SnapshotDate.ToShortDateString()
        : $"{SnapshotDate.ToShortDateString()} - {SnapshotLabel}";
}

