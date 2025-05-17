namespace Olympus.Infrastructure.Ai.Plugins.CoreUtilityPlugin.NativeFunctions;

public static class SimpleChanceRoll
{
  public static bool Roll(int percentChance)
  {
    // TODO: Implement real random logic
    return percentChance > 50;
  }
}
