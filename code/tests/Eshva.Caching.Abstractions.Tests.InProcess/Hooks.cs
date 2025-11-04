using Eshva.Caching.Abstractions.Tests.InProcess.Common;
using Reqnroll;
using Xunit.Abstractions;

namespace Eshva.Caching.Abstractions.Tests.InProcess;

[Binding]
public sealed class Hooks {
  [BeforeScenario]
  public void CreateCaches(ScenarioContext scenarioContext, ITestOutputHelper logger) {
    var cachesContext = new CachesContext(logger);
    scenarioContext.ScenarioContainer.RegisterInstanceAs(cachesContext);
  }
}
