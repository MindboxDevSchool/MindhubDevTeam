using System;

namespace ItHappened.Infrastructure
{
    public abstract class Maybe<T>
    {
        private Maybe() { }

        public sealed class Some : Maybe<T>
        {
            public Some(T value) => Value = value != null ? value : throw new ArgumentNullException(nameof(value));
            public T Value { get; }
        }

        public sealed class None : Maybe<T> { }
    }
}