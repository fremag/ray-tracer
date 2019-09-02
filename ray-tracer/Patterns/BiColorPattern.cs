namespace ray_tracer.Patterns
{
    public abstract class BiColorPattern : AbstractPattern
    {
        public Color ColorA { get;  }
        public Color ColorB { get;  }

        protected BiColorPattern(Matrix transform, Color colorA, Color colorB) : base(transform)
        {
            ColorA = colorA;
            ColorB = colorB;
        }

        protected BiColorPattern(Color colorA, Color colorB) : this(Helper.CreateIdentity(), colorA, colorB)
        {
        }

        protected BiColorPattern() : this(Color.White, Color.Black)
        {}
    }
}