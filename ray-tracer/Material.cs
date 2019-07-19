namespace ray_tracer
{
    public class Material
    {
        public Color Color { get; set; }
        public double Ambient { get;  set;}
        public double Diffuse { get;  set;}
        public double Specular { get;  set;}
        public int Shininess { get;  set;}

        public Material( Color color, double ambient, double diffuse, double specular, int shininess)
        {
            Color = color;
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
            Shininess = shininess;
        }

        public Material() : this(new Color(1,1,1), 0.1, 0.9, 0.9, 200)
        {
                
        }

        protected bool Equals(Material other)
        {
            return Equals(Color, other.Color) && Ambient.Equals(other.Ambient) && Diffuse.Equals(other.Diffuse) && Specular.Equals(other.Specular) && Shininess == other.Shininess;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Material) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Color != null ? Color.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Ambient.GetHashCode();
                hashCode = (hashCode * 397) ^ Diffuse.GetHashCode();
                hashCode = (hashCode * 397) ^ Specular.GetHashCode();
                hashCode = (hashCode * 397) ^ Shininess;
                return hashCode;
            }
        }
    }
}