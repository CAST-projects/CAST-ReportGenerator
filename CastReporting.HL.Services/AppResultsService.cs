using CastReporting.HL.Domain;
using CastReporting.HL.Repositories;
using CastReporting.HL.Repositories.Interfaces;

namespace CastReporting.HL.Services;

public class ApplicationService
{
    public ApplicationService(HLWSConnection connection) {
        _connection = connection;
    }

    private readonly HLWSConnection _connection;

    // load application results
    public AppInfo GetResults(AppId appId) {
        IHighlightRepository repo = new HighlightRepository(_connection, null);
        return repo.GetAppResults(appId.Id); 
    }
}
