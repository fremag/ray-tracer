using System;

namespace ray_tracer.Shapes.IsoSurface
{
    public class Triplet
    {
        public int Index0;
        public int Index1;
        public int Index2;

        public Triplet(int index0, int index1, int index2)
        {
            Index0 = index0;
            Index1 = index1;
            Index2 = index2;
        }

        protected bool Equals(Triplet other)
        {
            return Index0 == other.Index0 && Index1 == other.Index1 && Index2 == other.Index2;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Triplet) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Index0, Index1, Index2);
        }
    }
}