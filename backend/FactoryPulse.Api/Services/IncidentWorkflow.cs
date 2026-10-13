using FactoryPulse.Api.Models;

namespace FactoryPulse.Api.Services;

public static class IncidentWorkflow
{
    public static bool CanTransition(IncidentStatus current, IncidentStatus next) =>
        current switch
        {
            IncidentStatus.Open => next is IncidentStatus.Acknowledged or IncidentStatus.Resolved,
            IncidentStatus.Acknowledged => next == IncidentStatus.Resolved,
            IncidentStatus.Resolved => false,
            _ => false
        };
}
