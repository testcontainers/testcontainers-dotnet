namespace DotNet.Testcontainers.Tests.Unit.Internals
{
  using System;
  using DotNet.Testcontainers.Internals;
  using Xunit;
  using Guard = DotNet.Testcontainers.Internals.Guard;

  public static class GuardTest
  {
    public static class Null
    {
      public class ThrowArgumentNullException
      {
        [Fact]
        public void IfNull()
        {
          Assert.Throws<ArgumentNullException>(
            () => Guard.Argument((object) null, nameof(this.IfNull)).NotNull());
        }
      }

      public class ThrowArgumentException
      {
        [Fact]
        public void IfNotNull()
        {
          Assert.Throws<ArgumentException>(
            () => Guard.Argument(new object(), nameof(this.IfNotNull)).Null());
        }
      }
    }

    public static class String
    {
      public class ThrowArgumentException
      {
        [Fact]
        public void IfEmpty()
        {
          Assert.Throws<ArgumentException>(
            () => Guard.Argument(string.Empty, nameof(this.IfEmpty)).NotEmpty());
        }

        [Fact]
        public void IfNotEmpty()
        {
          Assert.Throws<ArgumentException>(
            () => Guard.Argument("Not Empty", nameof(this.IfNotEmpty)).Empty());
        }
      }
    }
  }
}
