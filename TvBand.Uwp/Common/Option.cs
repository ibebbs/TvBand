using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvBand.Uwp.Common
{
    public class Option<T>
    {
        public Option(T value)
        {
            Value = value;
            IsSome = true;
            IsNone = false;
        }

        public Option()
        {
            Value = default(T);
            IsSome = false;
            IsNone = true;
        }

        public T Value { get; private set; }

        public bool IsSome { get; private set; }

        public bool IsNone { get; private set; }
    }

    public static class Option
    {
        public static Option<T> Some<T>(T value)
        {
            return new Option<T>(value);
        }

        public static Option<T> None<T>()
        {
            return new Option<T>();
        }
    }
}
