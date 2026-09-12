using ModSettings.Core;
using Timberborn.Modding;
using Timberborn.SettingsSystem;

namespace SupplyAlert.Settings {

  public class SupplyAlertSettings : ModSettingsOwner {

    public ModSetting<bool> FoodAlertsEnabled { get; } =
      new(true,
          ModSettingDescriptor.CreateLocalized("CaseBacon.SupplyAlert.Settings.FoodAlertsEnabled"));

    public ModSetting<bool> WaterAlertsEnabled { get; } =
      new(true,
          ModSettingDescriptor.CreateLocalized("CaseBacon.SupplyAlert.Settings.WaterAlertsEnabled"));

    public ModSetting<float> FoodThresholdDays { get; } =
      new(1.0f,
          ModSettingDescriptor.CreateLocalized("CaseBacon.SupplyAlert.Settings.FoodThresholdDays"));

    public ModSetting<float> WaterThresholdDays { get; } =
      new(1.0f,
          ModSettingDescriptor.CreateLocalized("CaseBacon.SupplyAlert.Settings.WaterThresholdDays"));

    public SupplyAlertSettings(ISettings settings,
                               ModSettingsOwnerRegistry modSettingsOwnerRegistry,
                               ModRepository modRepository) : base(
        settings, modSettingsOwnerRegistry, modRepository) {
    }

    public override string HeaderLocKey => "CaseBacon.SupplyAlert.Settings.Header";

    public override ModSettingsContext ChangeableOn => ModSettingsContext.All;

    protected override string ModId => "CaseBacon.SupplyAlert";

  }

}
