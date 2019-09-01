using System;
using ray_tracer.Patterns;

namespace ray_tracer
{
    public class Material
    {
        public IPattern Pattern { get; set; }
        public double Ambient { get;  set;}
        public double Diffuse { get;  set;}
        public double Specular { get;  set;}
        public int Shininess { get;  set;}

        public Material(IPattern pattern, double ambient = 0.1, double diffuse = 0.9, double specular = 0.9, int shininess = 200)
        {
            Pattern = pattern;
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
            Shininess = shininess;
        }
        
        public Material( Color color, double ambient=0.1, double diffuse=0.9, double specular=0.9, int shininess=200) : this(new SolidPattern(color), ambient, diffuse, specular, shininess)
        {
        }

        public Material() : this(new Color(1,1,1))
        {
                
        }

        public Color Lighting(PointLight light, IShape shape, Tuple point, Tuple eye, Tuple normal, bool isShadowed)
        {
            var objectPoint = shape.Transform.Inverse() * point;
            var patternPoint = Pattern.Transform.Inverse() * objectPoint;
            return Lighting(light, patternPoint, eye, normal, isShadowed);
        }
        
        public Color Lighting(PointLight light, Tuple point, Tuple eye, Tuple normal, bool isShadowed)
        {
            var color = Pattern.GetColor(point);
            var effectiveColor = color * light.Intensity;
            // find the direction to the light source
            var lightv = (light.Position - point).Normalize();
            // compute the ambient contribution
            var ambient = effectiveColor * Ambient;
            if (isShadowed)
            {
                return ambient;
            }
            
            // light_dot_normal represents the cosine of the angle between the
            // light vector and the normal vector. A negative number means the
            // light is on the other side of the surface.
            var lightDotNormal = lightv.DotProduct(normal);
            Color diffuse;
            Color specular;
            
            if (lightDotNormal < 0)
            {
                diffuse = Color.Black;
                specular= Color.Black;
            }
            else
            {
                // compute the diffuse contribution
                diffuse = effectiveColor * Diffuse * lightDotNormal;
                // reflect_dot_eye represents the cosine of the angle between the
                // reflection vector and the eye vector. A negative number means the
                // light reflects away from the eye.
                var reflect = -lightv.Reflect(normal);
                var reflectDotEye = reflect.DotProduct(eye);
                if (reflectDotEye <= 0)
                {
                    specular = Color.Black;
                }
                else
                {
                    // compute the specular contribution
                    var factor = Math.Pow(reflectDotEye, Shininess);
                    specular = light.Intensity * Specular * factor;
                }
            }
            // Add the three contributions together to get the final shading
            return ambient + diffuse + specular;
        }
        
#region EqualsHashCode
        protected bool Equals(Material other)
        {
            return Equals(Pattern, other.Pattern) && Ambient.Equals(other.Ambient) && Diffuse.Equals(other.Diffuse) && Specular.Equals(other.Specular) && Shininess == other.Shininess;
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
                var hashCode = (Pattern != null ? Pattern.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Ambient.GetHashCode();
                hashCode = (hashCode * 397) ^ Diffuse.GetHashCode();
                hashCode = (hashCode * 397) ^ Specular.GetHashCode();
                hashCode = (hashCode * 397) ^ Shininess;
                return hashCode;
            }
        }
    }
#endregion    
}