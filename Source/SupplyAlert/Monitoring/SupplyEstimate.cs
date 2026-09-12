namespace SupplyAlert.Monitoring {

  // DaysRemaining is null when the supply is not currently trending towards depletion
  // (stable, growing, or no meaningful data yet).
  internal readonly struct SupplyEstimate {

    public float? DaysRemaining { get; }

    private SupplyEstimate(float? daysRemaining) {
      DaysRemaining = daysRemaining;
    }

    public static SupplyEstimate Safe { get; } = new(null);
    public static SupplyEstimate Depleted { get; } = new(0f);

    public static SupplyEstimate At(float daysRemaining) {
      return new(daysRemaining);
    }

  }

}
