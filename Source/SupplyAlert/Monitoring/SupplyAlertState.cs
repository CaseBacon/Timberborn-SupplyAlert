namespace SupplyAlert.Monitoring {

  // Per-supply hysteresis state machine: fires once when crossing at/below the threshold,
  // then stays silent until the supply recovers safely above it (threshold * RearmMultiplier).
  internal class SupplyAlertState {

    private const float RearmMultiplier = 1.15f;

    private bool _baselineCaptured;
    private bool _alertActive;

    // Returns true exactly when a new alert should be raised.
    public bool ShouldAlert(float? daysRemaining, float thresholdDays) {
      var isBelowThreshold = daysRemaining.HasValue && daysRemaining.Value <= thresholdDays;

      if (!_baselineCaptured) {
        // Don't alert on the first evaluation after load/new game just because the
        // supply already happens to be low - only react to a live crossing.
        _baselineCaptured = true;
        _alertActive = isBelowThreshold;
        return false;
      }

      if (!_alertActive && isBelowThreshold) {
        _alertActive = true;
        return true;
      }

      if (_alertActive && IsSafelyRecovered(daysRemaining, thresholdDays)) {
        _alertActive = false;
      }

      return false;
    }

    public void Reset() {
      _baselineCaptured = false;
      _alertActive = false;
    }

    private static bool IsSafelyRecovered(float? daysRemaining, float thresholdDays) {
      return !daysRemaining.HasValue || daysRemaining.Value > thresholdDays * RearmMultiplier;
    }

  }

}
