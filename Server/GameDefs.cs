// This file contains all the shared constant definitions between the Client and Server

using System.Numerics;

public class GameDefs
{
  public const float MidZ = 8.5f; // The middle lane is defined as all Z values < MidZ and > -MidZ
  public const float MidBridge = 1.6f; // The middle bridge has Z values < MidBridge and > -MidBridge 
  public const float MidBarrier = 13.6f; // Any X value beyond this in the middle lane, must be on the bridge

  public const float ExtremeBarrier = 4f; // Any x value beyond this in the top or bottom lane, must be on a bridge
  public const float BridgeWidth = 3f; // This is the width of the bridge
  public const float BridgeZ = 19.6f; // This is the distance from the middle to the edge of the bridge

  public const float BaseX = 36f;
  public const float BaseZ = 0f;

  public const float MaxMana = 10f;
  public const float ManaPerSecond = .5f;
  public const float StartingMana = 2.5f;

  /*
   * The "river" is shaped like an octagon, with the rightmost and leftmost edges split in the center
   * As such, the points around the octagon can be labelled like this:
   *     BRIDGE
   *     c----d
   *   b/      \e
   *  a|        |f
   *     BRIDGE
   *  l|        |g
   *   k\      / h
   *     j----i
   *     BRIDGE
   * With the lines comprising it being AB, BC, CD, DE, EF, GH, HI, IJ, JK, and KL
   */
  private static readonly Vector2 ra = new Vector2(-MidBarrier, MidZ / 2 - MidBridge);
  private static readonly Vector2 rb = new Vector2(-MidBarrier, MidZ);
  private static readonly Vector2 rc = new Vector2(-ExtremeBarrier, BridgeZ);
  private static readonly Vector2 rd = new Vector2(ExtremeBarrier, BridgeZ);
  private static readonly Vector2 re = new Vector2(MidBarrier, MidZ);
  private static readonly Vector2 rf = new Vector2(MidBarrier, MidZ / 2 - MidBridge);
  private static readonly Vector2 rg = new Vector2(MidBarrier, -(MidZ / 2 - MidBridge));
  private static readonly Vector2 rh = new Vector2(MidBarrier, -MidZ);
  private static readonly Vector2 ri = new Vector2(ExtremeBarrier, -BridgeZ);
  private static readonly Vector2 rj = new Vector2(-ExtremeBarrier, -BridgeZ);
  private static readonly Vector2 rk = new Vector2(-MidBarrier, -MidZ);
  private static readonly Vector2 rl = new Vector2(-MidBarrier, -(MidZ / 2 - MidBridge));

  // Finds whether the line segment connecting a to b crosses an illegal region 
  public static bool CrossesRiver(Vector2 a, Vector2 b)
  {
    return Intersects(a, b, ra, rb) ||
           Intersects(a, b, rb, rc) ||
           Intersects(a, b, rc, rd) ||
           Intersects(a, b, rd, re) ||
           Intersects(a, b, re, rf) ||
           Intersects(a, b, rg, rh) ||
           Intersects(a, b, rh, ri) ||
           Intersects(a, b, ri, rj) ||
           Intersects(a, b, rj, rk) ||
           Intersects(a, b, rk, rl);
  }

  // Finds whether two line segments, AB and CD, intersect
  private static bool Intersects(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
  {
    return (CounterClockwise(a, c, d) != CounterClockwise(b, c, d)) &&
           (CounterClockwise(a, b, c) != CounterClockwise(a, b, d));
  }

  // Helper function for intersection detection: https://bryceboe.com/2006/10/23/line-segment-intersection-algorithm/
  private static bool CounterClockwise(Vector2 a, Vector2 b, Vector2 c)
  {
    return (c.Y - a.Y) * (b.X - a.X) > (b.Y - a.Y) * (c.X - a.X);
  }
}