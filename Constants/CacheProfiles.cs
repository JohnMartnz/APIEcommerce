using System;
using Microsoft.AspNetCore.Mvc;

namespace APIEcommerce.Constants;

public static class CacheProfiles
{
  public const string Default10 = "Default10";
  public const string Default20 = "Default20";

  public static readonly CacheProfile Profile10 = new()
  {
    Duration = 10,
  };

  public static readonly CacheProfile Profile20 = new()
  {
    Duration = 20
  };
}
