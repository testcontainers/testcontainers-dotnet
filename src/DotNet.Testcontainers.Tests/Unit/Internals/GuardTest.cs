namespace DotNet.Testcontainers.Tests.Unit.Internals
{
  using System;
  using Xunit;

  public static class GuardTest
  {
    public static class NullPreconditions
    {
      public class DoNotThrowException
      {
        [Fact]
        public void IfNull()
        {
          _ = Guard.Argument((object) null, nameof(this.IfNull)).Null();
        }

        [Fact]
        public void IfNotNull()
        {
          _ = Guard.Argument(new object(), nameof(this.IfNotNull)).NotNull();
        }
      }

      public class ThrowArgumentNullExceptionMatchImage
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

    public static class StringPreconditions
    {
      public class DoNotThrowException
      {
        [Fact]
        public void IfEmpty()
        {
          _ = Guard.Argument(string.Empty, nameof(this.IfEmpty)).Empty();
        }

        [Fact]
        public void IfNotEmpty()
        {
          _ = Guard.Argument("Not Empty", nameof(this.IfNotEmpty)).NotEmpty();
        }
      }

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
