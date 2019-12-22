using System;
using ray_tracer.Lights;
using ray_tracer.Patterns;

namespace ray_tracer
{
    public class Material 
    {
        public IPattern Pattern { get; set; }
        public double Ambient { get;  set;}
        public double Diffuse { get;  set;}
        public double Specular { get;  set;}
        public int Shininess { get; set; }
        public double Reflective { get;  set;}
        public double Transparency { get;  set;}
        public double RefractiveIndex { get;  set;}

        public Material(IPattern pattern, double ambient = 0.1, double diffuse = 0.9, double specular = 0.9, int shininess = 200, double reflective = 0, double transparency = 0, double refractiveIndex = 1)
        {
            Pattern = pattern;
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
            Shininess = shininess;
            Reflective = reflective;
            Transparency = transparency;
            RefractiveIndex = refractiveIndex;
        }
        
        public Material( Color color, double ambient=0.1, double diffuse=0.9, double specular=0.9, int shininess=200, double reflective = 0.0, double transparency = 0, double refractiveIndex = 1) : this(new SolidPattern(color), ambient, diffuse, specular, shininess, reflective, transparency, refractiveIndex)
        {
        }

        public Material() : this(new Color(1,1,1))
        {
                
        }

        public unsafe Color Lighting(ILight light, IShape shape, Tuple point, Tuple eye, Tuple normal, double lightIntensity)
        {
            var color = Pattern.GetColorAtShape(shape, point);
            double* x = stackalloc double[1];
            double* y = stackalloc double[1];
            double* z = stackalloc double[1];
            x[0] = light.Position.X;
            y[0] = light.Position.Y;
            z[0] = light.Position.Z;
            return Lighting(1, x, y, z, point, eye, normal, lightIntensity, color, light.Intensity);
        }

        public unsafe Color Lighting(ILight light, Tuple point, Tuple eye, Tuple normal, double lightIntensity)
        {
            var color = Pattern.GetColor(point);
            double* x = stackalloc double[1];
            double* y = stackalloc double[1];
            double* z = stackalloc double[1];
            x[0] = light.Position.X;
            y[0] = light.Position.Y;
            z[0] = light.Position.Z;
            
            return Lighting(1, x, y, z, point, eye, normal, lightIntensity, color, light.Intensity);
        }
        
        public unsafe Color Lighting(int nbLights, double* x, double* y, double* z, Tuple point, Tuple eye, Tuple normal, double lightIntensity, Color color, Color lightColor)
        {
            var effectiveColor = color * lightColor;
            // compute the ambient contribution
            var ambient = effectiveColor * Ambient;

            // find the direction to the light source
            var lightv = Helper.CreateVector(x[0] - point.X, y[0] - point.Y, z[0] - point.Z).Normalize();
            
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
                var reflect = (-lightv).Reflect(normal);
                var reflectDotEye = reflect.DotProduct(eye);
                if (reflectDotEye <= 0)
                {
                    specular = Color.Black;
                }
                else
                {
                    // compute the specular contribution
                    var factor = Math.Pow(reflectDotEye, Shininess);
                    specular = lightColor * Specular * factor;
                }
            }
            // Add the three contributions together to get the final shading
            var colorFromLight = diffuse + specular;
            if (lightIntensity < 1)
            {
                colorFromLight *= lightIntensity;
            }
            return ambient + colorFromLight;
        }

#region EqualsHashCode
        protected bool Equals(Material other)
        {
            return Equals(Pattern, other.Pattern) && Ambient.Equals(other.Ambient) && Diffuse.Equals(other.Diffuse) && Specular.Equals(other.Specular) && Shininess == other.Shininess  && Reflective.Equals(other.Reflective);
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
                hashCode = (hashCode * 397) ^ Reflective.GetHashCode();
                hashCode = (hashCode * 397) ^ Shininess;
                return hashCode;
            }
        }
    }
#endregion

    

}