using CastReporting.HL.Domain;
using CastReporting.HL.Repositories;
using CastReporting.HL.Repositories.Interfaces;
using CastReporting.Mediation;

namespace CastReporting.HL.Services;

public class AccountService
{
    public AccountService(HLWSConnection connection)
    {
        _connection = connection;
    }

    private readonly HLWSConnection _connection;

    public IList<AppId> GetAvailableApplications()
    {
        IHighlightRepository repo = new HighlightRepository(_connection, null);
        return repo.GetDomainAppIds();
    }

    public IList<Snapshot> GetAvailableSnapshots(AppId app) {
        IHighlightRepository repo = new HighlightRepository(_connection, null);
        return repo.GetAppSnapshots(app.Id);
    }
}
