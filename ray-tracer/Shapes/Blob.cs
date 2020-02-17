using System;
using System.Collections.Generic;

namespace ray_tracer.Shapes
{
    // Thnaks to Grégory Massal !
    // http://www.massal.net/article/raytrace/page4.html
    public struct Poly
    {
        public double A;
        public double B;
        public double C;
    }

    public class Blob : AbstractShape, IComparer<Tuple<double, Poly>>
    {
        const int zoneNumber = 10;

        public List<Tuple> Points = new List<Tuple>();
        public double Size { get; set; } = 1;

        // une zone est définie par un rapport du carré de la distance à la sphère sur le carré de son rayon
        // puis par les gamma et beta incrémentaux qui approchent la courbe 1/ dist^2
        // ces deux coefficients ne sont calculés qu'une seule fois par exécution dans l'initZones.
        struct Zone
        {
            public double fCoef, fGamma, fBeta;

            public Zone(double fCoef, double fGamma, double fBeta)
            {
                this.fCoef = fCoef;
                this.fGamma = fGamma;
                this.fBeta = fBeta;
            }
        }

        static Zone[] zoneTab =
        {
            new Zone(10.0, 0, 0),
            new Zone(5.0, 0, 0),
            new Zone(3.33333, 0, 0),
            new Zone(2.5, 0, 0),
            new Zone(2.0, 0, 0),
            new Zone(1.66667, 0, 0),
            new Zone(1.42857, 0, 0),
            new Zone(1.25, 0, 0),
            new Zone(1.1111, 0, 0),
            new Zone(1.0, 0, 0)
        };

        static Blob()
        {
            double fLastGamma = 0.0;
            double fLastBeta = 0.0;
            double fLastInvRSquare = 0.0;
            for (int i = 0; i < zoneNumber - 1; i++)
            {
                double fInvRSquare = 1.0 / zoneTab[i + 1].fCoef;
                // fGamma est la pente entre le point d'entrée et le point de sortie
                // on ne stocke que la valeur incrémentale par rapport à la zone précédente
                zoneTab[i].fGamma = (fLastInvRSquare - fInvRSquare) / (zoneTab[i].fCoef - zoneTab[i + 1].fCoef) - fLastGamma;
                fLastGamma = zoneTab[i].fGamma + fLastGamma;
                // en faisant - fLastGamma et + fLastGamma à la ligne suivante 
                // on s'économise l'utilisation d'une temporaire
                // je sais c'est débile..

                // fBeta est la valeur de la droite approchant la courbe pour dist = 0
                // on ne stocke que la valeur incrémentale par rapport à la zone précédente
                zoneTab[i].fBeta = fInvRSquare - fLastGamma * zoneTab[i + 1].fCoef - fLastBeta;
                fLastBeta = zoneTab[i].fBeta + fLastBeta;
                fLastInvRSquare = fInvRSquare;
            }

            // la derniere zone d'influence agit comme un signal que l'on traite comme un cas particulier
            zoneTab[zoneNumber - 1].fGamma = 0.0f;
            zoneTab[zoneNumber - 1].fBeta = 0.0f;
        }

        public override void IntersectLocal(ref Tuple origin, ref Tuple direction, Intersections intersections)
        {
            List<Tuple<double, Poly>> polynomMap = new List<Tuple<double, Poly>>();

            // tous ces calculs pourraient probablement 
            // etre réduit lors d'un précalcul mais j'essaie
            // de privilégier la clarté à la rapidité pour l'instant

            double rSquare = Size * Size;
            // Cela ne sert à rien d'avoir des spheres de taille zero
            // mais soyons de bons paranoiaques tout de meme
            if (rSquare == 0.0f)
                return;
            double rInvSquare = 1.0f / rSquare;
            for (int i = 0; i < Points.Count; i++)
            {
                double A, B, C;
                double fDelta, t0, t1;
                Tuple currentPoint = Points[i];

                Tuple vDist = currentPoint - origin;
                A = 1.0;
                B = -2.0 * direction.DotProduct(vDist);
                C = vDist.DotProduct(vDist);

                // on parcourt la liste des zones d'influences de la sphère courante
                // on calcule la nouvelle version du polynome qui a cours dans 
                // cette zone d'influence et on le stocke de manière incrémentale
                // ce qui importe c'est la différence avec la zone précédente, ce qui permet
                // de bien gérer le cas de sphères imbriquées les unes dans les autres
                for (int j = 0; j < zoneNumber - 1; j++)
                {
                    // On calcule le Delta de l'équation s'il est négatif
                    // il n'y a pas de solution donc pas de point d'intersection
                    fDelta = B * B - 4.0f * (C - zoneTab[j].fCoef * rSquare);
                    if (fDelta < 0.0f)
                        break;

                    t0 = 0.5f * (-B - Math.Sqrt(fDelta));
                    // cool on ne s'occupe pas de l'ordre il est ici explicite t0 < t1
                    t1 = 0.5f * (-B + Math.Sqrt(fDelta));
                    Poly poly0 = new Poly
                    {
                        A = zoneTab[j].fGamma * A * rInvSquare,
                        B = zoneTab[j].fGamma * B * rInvSquare,
                        C = zoneTab[j].fGamma * C * rInvSquare + zoneTab[j].fBeta
                    };
                    Poly poly1 = new Poly {A = -poly0.A, B = -poly0.B, C = -poly0.C};

                    // les variations du polynome sont trièes par leur position sur le rayon
                    // au fur et à mesure qu'elles sont insérées. C'est la map qui nous garantit cela.
                    // ce serait peut-etre plus optimal de placer dans un vector sans se soucier de l'ordre
                    // et trier ensuite mais je me fiche de l'optimisation en fait.
                    polynomMap.Add(new Tuple<double, Poly>(t0, poly0));
                    polynomMap.Add(new Tuple<double, Poly>(t1, poly1));
                }
            }

            try
            {
                polynomMap.Sort(this);
            }
            catch (Exception e)
            {
                Console.WriteLine("No !");
            }


            // en dehors de toute zone d'influence le champ de potentiel est nul
            Poly currentPolynom = new Poly {A = 0.0, B = 0.0, C = 0.0};

            double t = double.PositiveInfinity;
            for (var i = 0; i < polynomMap.Count - 1; i++)
            {
                var kvp = polynomMap[i];
                Poly poly = kvp.Item2;
                double key = kvp.Item1;

                // comme on a stocké les polynomes de manière incrémentale on 
                // reconstruit le polynome de la zone d'influence courante
                currentPolynom.A += poly.A;
                currentPolynom.B += poly.B;
                currentPolynom.C += poly.C;

                double nextKey = polynomMap[i + 1].Item1;

                // ça ne sert à rien de résoudre l'équation si la zone d'influence est avant le point de départ
                // ou après le point d'arrivée sur le rayon
                if ((t > key) && nextKey > 0.01f)
                {
                    // on peut se permettre de résoudre la dernière équation de manière exacte
                    // après toutes les approximations que l'on a fait avec un nombre suffisant de découpages,
                    // il devrait être difficile de faire la distinction entre le blob et son découpage approché.
                    double fDelta = currentPolynom.B * currentPolynom.B - 4.0f * currentPolynom.A * (currentPolynom.C - 1.0f);
                    if (fDelta < 0.0f)
                        continue;

                    bool retValue = false;

                    double t0 = (0.5f / currentPolynom.A) * (-currentPolynom.B - Math.Sqrt(fDelta));
                    if ((t0 > 0.01f) && (t0 >= key) && (t0 < nextKey) && (t0 <= t))
                    {
                        t = t0;
                        retValue = true;
                    }

                    double t1 = (0.5f / currentPolynom.A) * (-currentPolynom.B + Math.Sqrt(fDelta));
                    if ((t1 > 0.01f) && (t1 >= key) && (t1 < nextKey) && (t1 <= t))
                    {
                        t = t1;
                        retValue = true;
                    }

                    if (retValue)
                    {
                        intersections.Add(new Intersection(t, this));
                        return;
                    }
                }
            }
        }

        public override Tuple NormalAtLocal(Tuple worldPoint, Intersection hit = null)
        {
            Tuple gradient = Helper.CreateVector(0, 0, 0);
            // float fSomme = 0.0f;
            double fRSquare = Size * Size;
            for (int i = 0; i < Points.Count; i++)
            {
                Tuple normal = worldPoint - Points[i];
                double fDistSquare = normal.DotProduct(normal);
                if (fDistSquare <= 0.001f)
                    continue;
                double fDistFour = fDistSquare * fDistSquare;
                normal = (fRSquare / fDistFour) * normal;

                // c'est la vraie formule du gradient dans un champ de potentiel 
                // et non pas une simple moyenne
                gradient = gradient + normal;
                // fSomme = fSomme + (fRSquare/fDistFour);
            }

            return gradient;
        }

        public override Bounds Box { get; }

        public int Compare(Tuple<double, Poly> x, Tuple<double, Poly> y)
        {
            return (int) (x.Item1 - y.Item1);
        }
    }
}
#endif