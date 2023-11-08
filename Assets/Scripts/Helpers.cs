namespace Tuong
{
    public static class Helpers
    {
        public static void Swap<T> (ref T lhs, ref T rhs) {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}