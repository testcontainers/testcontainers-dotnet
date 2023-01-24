namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using Xunit;

  public static class GuardTest
  {
    public static class NullPreconditions
    {
      public sealed class DoNotThrowException
      {
        [Fact]
        public void IfNull()
        {
          var exception = Record.Exception(() => Guard.Argument((object)null, nameof(this.IfNull)).Null());
          Assert.Null(exception);
        }

        [Fact]
        public void IfNotNull()
        {
          var exception = Record.Exception(() => Guard.Argument(new object(), nameof(this.IfNotNull)).NotNull());
          Assert.Null(exception);
        }

        [Fact]
        public void ThrowIf()
        {
          var exception = Record.Exception(() => Guard.Argument(new object(), nameof(this.ThrowIf)).ThrowIf(_ => false, _ => new ArgumentException()));
          Assert.Null(exception);
        }
      }

      public sealed class ThrowArgumentException
      {
        [Fact]
        public void IfNull()
        {
          Assert.Throws<ArgumentException>(() => Guard.Argument((object)null, nameof(this.IfNull)).NotNull());
        }

        [Fact]
        public void IfNotNull()
        {
          Assert.Throws<ArgumentException>(() => Guard.Argument(new object(), nameof(this.IfNotNull)).Null());
        }

        [Fact]
        public void ThrowIf()
        {
          Assert.Throws<ArgumentException>(() => Guard.Argument(new object(), nameof(this.ThrowIf)).ThrowIf(_ => true, _ => new ArgumentException()));
        }
      }
    }

    public static class StringPreconditions
    {
      public sealed class DoNotThrowException
      {
        [Fact]
        public void IfEmpty()
        {
          var exception = Record.Exception(() => Guard.Argument(string.Empty, nameof(this.IfEmpty)).Empty());
          Assert.Null(exception);
        }

        [Fact]
        public void IfNotEmpty()
        {
          var exception = Record.Exception(() => Guard.Argument("Not Empty", nameof(this.IfNotEmpty)).NotEmpty());
          Assert.Null(exception);
        }
      }

      public sealed class ThrowArgumentException
      {
        [Fact]
        public void IfEmpty()
        {
          Assert.Throws<ArgumentException>(() => Guard.Argument(string.Empty, nameof(this.IfEmpty)).NotEmpty());
        }

        [Fact]
        public void IfNotEmpty()
        {
          Assert.Throws<ArgumentException>(() => Guard.Argument("Not Empty", nameof(this.IfNotEmpty)).Empty());
        }
      }
    }
  }
}
