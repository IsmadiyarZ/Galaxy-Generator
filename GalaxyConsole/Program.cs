/* NAME: Zhamalidinov Ismadiyar Aybekovich, EPI-1-23
 * ASGN: Lab 1 (Console - Full Parameters)
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using GalaxyLib;

namespace GalaxyConsole
{
    class CProgram
    {
        static void Main(string[] args)
        {
            CGalaxyEngine engine = new CGalaxyEngine();
            Random local_rnd = new Random();

            Console.WriteLine("---------------------------------------------\n");
            Console.WriteLine("             ГЕНЕРАТОР ГАЛАКТИКИ             \n");
            Console.WriteLine("---------------------------------------------\n");
            Console.WriteLine("\nВыберите тип галактики:");
            Console.WriteLine("1 [ Эллиптическая ]");
            Console.WriteLine("2 [ Миндалевидная ]");
            Console.WriteLine("3 [ Спиральная    ]");
            Console.Write("\nВаш выбор: ");

            string inp_type = Console.ReadLine() ?? "";
            GalaxyType sel_type = GalaxyType.Spiral;
            if (inp_type == "1") sel_type = GalaxyType.Elliptical;
            else if (inp_type == "2") sel_type = GalaxyType.Almond;

            
            Console.Write("\nВведите  радиус галактики (рекомендуется 100-300): ");
            string inp_rad = Console.ReadLine() ?? "";
            if (!int.TryParse(inp_rad, out int img_rad))
            {
                Console.WriteLine("Некорректный ввод. Установлен радиус 200.");
                img_rad = 200;
            }

     
            Console.WriteLine("\nВыберите цвет звезд:");
            Console.WriteLine("1 - Белый      (White)");
            Console.WriteLine("2 - Красный    (Red)");
            Console.WriteLine("3 - Оранжевый  (Orange)");
            Console.WriteLine("4 - Желтый     (Yellow)");
            Console.WriteLine("5 - Зеленый    (Green)");
            Console.WriteLine("6 - Голубой    (DeepSkyBlue)");
            Console.WriteLine("7 - Синий      (Blue)");
            Console.WriteLine("8 - Фиолетовый (Purple)");
            Console.WriteLine("9 - Розовый    (Pink)");
            Console.Write("\nВаш выбор (1-9): ");

            string inp_clr = Console.ReadLine() ?? "";
            Color star_color;

            switch (inp_clr)
            {

                case "1": star_color = Color.White; break;
                case "2": star_color = Color.Red; break;
                case "3": star_color = Color.Orange; break;
                case "4": star_color = Color.Yellow; break;
                case "5": star_color = Color.Green; break;
                case "6": star_color = Color.DeepSkyBlue; break;
                case "7": star_color = Color.Blue; break;
                case "8": star_color = Color.Purple; break;
                case "9": star_color = Color.Pink; break;
                default: star_color = Color.White; break;
            }

            Console.WriteLine("\nНачинаю расчет... Подождите немного.");

            
            int img_size = Math.Max(600, img_rad * 3);
            int center = img_size / 2;
            Bitmap bmp_res = new Bitmap(img_size, img_size);

       

            using (Graphics g = Graphics.FromImage(bmp_res))
            {
                g.Clear(Color.Black);
                DrawSimpleSpace(g, img_size, img_size);
            }

 
            for (int i = 0; i<img_size; i++)
            {
                for (int j = 0; j < img_size; j++)
                {
                    double dens = engine.GetDensity(i, j, center, center, img_rad, sel_type, 0);

                    if (dens < 0.01) continue;

                    if (local_rnd.NextDouble() < dens * 1.3)
                    {
                      
                        int r_v = (int)(star_color.R + (255 - star_color.R) * dens);
                        int g_v = (int)(star_color.G + (255 - star_color.G) * dens);
                        int b_v = (int)(star_color.B + (255 - star_color.B) * dens);

                        bmp_res.SetPixel(i, j, Color.FromArgb(255,
                            Math.Min(255, r_v), Math.Min(255, g_v), Math.Min(255, b_v)));
                    }
                    else if (dens > 0.1)
                    {
                        int dust_a = (int)(dens * 45);
                        bmp_res.SetPixel(i, j, Color.FromArgb(dust_a, star_color));
                    }
                }
            }

            string out_path = "output_console.bmp";
            bmp_res.Save(out_path);

            Console.WriteLine("\n---------------------------------------------");
            Console.WriteLine("УСПЕХ! Изображение сохранено: " + out_path);
            Console.WriteLine("Нажмите любую клавишу для завершения...");
            Console.ReadKey();
        }

        static void DrawSimpleSpace(Graphics g, int w, int h)
        {
            Random rnd = new Random(42);
            for (int i = 0; i < 400; i++)
            {
                int x = rnd.Next(0, w);
                int y = rnd.Next(0, h);
                int b = rnd.Next(100, 255);
                int sz = (rnd.Next(10) > 8) ? 2 : 1;
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(b, b, b)))
                {
                    g.FillRectangle(sb, x,y, sz, sz);
                }
            }
        }
    }
}