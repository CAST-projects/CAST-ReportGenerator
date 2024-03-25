namespace CastReporting.HL.Domain;


// Modèle de l'entité HL correspondant à un snapshot d'analyse
public class Snapshot
{
    static readonly DateTime EPOCH = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public long snapshotDate { get; set; }
    public DateTime SnapshotDate => EPOCH + new TimeSpan(0, 0, (int)(snapshotDate / 1000));

    public string SnapshotLabel { get; set; } = string.Empty;

    public override string ToString() => string.IsNullOrWhiteSpace(SnapshotLabel)
        ? SnapshotDate.ToShortDateString()
        : $"{SnapshotDate.ToShortDateString()} - {SnapshotLabel}";
}

