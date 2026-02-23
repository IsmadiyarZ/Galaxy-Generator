//NAME: Zhamalidinov Ismadiyar Aybekovich, EPI-1-23
//ASGN: Lab 1
 
using System;

namespace GalaxyLib
{
    public enum GalaxyType { Elliptical, Almond, Spiral }

    public class CGalaxyEngine
    {
        private Random m_rng = new Random();

        public double GetDensity(int x, int y, int c_x, int c_y,
                                 int rad, GalaxyType g_type, double t_step)
        {
            double d_x = x - c_x;
            double d_y = y - c_y;

            switch (g_type)
            {
                case GalaxyType.Elliptical:

                    double p_rad = rad * (1.0 + Math.Sin(t_step) * 0.03);
                    double dist_e = Math.Sqrt(d_x * d_x + d_y * d_y);
                    return Math.Exp(-Math.Pow(dist_e / (p_rad * 0.6), 1.6));

                case GalaxyType.Almond:
                  
                    double tilt = (Math.PI / 4.0) + Math.Sin(t_step * 0.4) * 0.05;
                    double cos_t = Math.Cos(tilt);
                    double sin_t = Math.Sin(tilt);
                    double rx_a = d_x * cos_t + d_y * sin_t;
                    double ry_a = -d_x * sin_t + d_y * cos_t;
                    double p_dist = (rx_a * rx_a) / (rad * rad) +
                                    (ry_a * ry_a) / (Math.Pow(rad / 4.5, 2));
                    return Math.Exp(-Math.Pow(p_dist, 0.85));

                case GalaxyType.Spiral:
                    double dist_s = Math.Sqrt(d_x * d_x + d_y * d_y);
                    if (dist_s > rad * 2.5) return 0;

                    // Угол закручивания + вращение во времени
                    double twist = (0.2 * Math.PI * rad) / Math.Pow(dist_s + 1, 0.1);
                    double angle = twist + t_step;

                    double cos_s = Math.Cos(angle);
                    double sin_s = Math.Sin(angle);
                    double rx_s = d_x * cos_s + d_y * sin_s;
                    double ry_s = -d_x * sin_s + d_y * cos_s;

                    double w_arm = rad * 0.15;
                    double arms = Math.Exp(-(ry_s * ry_s) / (w_arm * w_arm)) *
                                  Math.Exp(-(rx_s * rx_s) / (rad * rad)) +
                                  Math.Exp(-(rx_s * rx_s) / (w_arm * w_arm)) *
                                  Math.Exp(-(ry_s * ry_s) / (rad * rad));

                    double core = Math.Exp(-dist_s / (rad * 0.2));
                    return Math.Min(1.0, arms + core);

                default: return 0;
            }
        }
    }
}