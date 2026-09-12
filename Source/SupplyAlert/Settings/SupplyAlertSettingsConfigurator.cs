using Bindito.Core;

namespace SupplyAlert.Settings {

  [Context("Game")]
  [Context("MainMenu")]
  public class SupplyAlertSettingsConfigurator : Configurator {

    protected override void Configure() {
      Bind<SupplyAlertSettings>().AsSingleton();
    }

  }

}
