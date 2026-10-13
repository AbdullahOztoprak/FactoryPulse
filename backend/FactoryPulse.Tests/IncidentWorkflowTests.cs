using FactoryPulse.Api.Models;
using FactoryPulse.Api.Services;
using Xunit;

namespace FactoryPulse.Tests;

public class IncidentWorkflowTests
{
    [Theory]
    [InlineData(IncidentStatus.Open, IncidentStatus.Acknowledged, true)]
    [InlineData(IncidentStatus.Open, IncidentStatus.Resolved, true)]
    [InlineData(IncidentStatus.Acknowledged, IncidentStatus.Resolved, true)]
    [InlineData(IncidentStatus.Resolved, IncidentStatus.Open, false)]
    [InlineData(IncidentStatus.Acknowledged, IncidentStatus.Open, false)]
    [InlineData(IncidentStatus.Open, IncidentStatus.Open, false)]
    public void Status_transition_rules_are_consistent(IncidentStatus current, IncidentStatus next, bool expected)
    {
        Assert.Equal(expected, IncidentWorkflow.CanTransition(current, next));
    }
}
