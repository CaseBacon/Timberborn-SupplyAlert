using Bindito.Core;
using SupplyAlert.Monitoring;

namespace SupplyAlert {

  [Context("Game")]
  public class SupplyAlertConfigurator : Configurator {

    protected override void Configure() {
      Bind<FoodSupplyEstimator>().AsSingleton();
      Bind<WaterSupplyEstimator>().AsSingleton();
      Bind<SupplyMonitor>().AsSingleton();
    }

  }

}
