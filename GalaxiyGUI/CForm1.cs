//NAME: Zhamalidinov Ismadiyar Aybekovich, EPI-1-23
//ASGN: Lab 1 GUI 
 
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GalaxyLib;

namespace GalaxiyGUI
{
    public partial class CForm1 : Form
    {
        private CGalaxyEngine m_engine = new CGalaxyEngine();
        private Color m_star_color =  Color.FromArgb(108, 64, 254);
        private Color m_bg_color = Color.Black;

        private System.Windows.Forms.Timer m_anim_timer = new System.Windows.Forms.Timer();
        private double m_time_val = 0;
        private bool m_is_animating = false;

        private PictureBox m_pic_box = null!;
        private Button m_btn_generate = null!;
        private Button m_btn_anim = null!;
        private Button m_btn_star_clr = null!;
        private Button m_btn_bg_clr = null!;
        private Button m_btn_save = null!;
        private TrackBar m_radius_scroll = null!;
        private ComboBox m_type_combo = null!;
        private Label m_lbl_rad = null!;
        private Label m_lbl_type = null!;


        public CForm1()
        {
            InitializeComponent();

            m_type_combo.Items.AddRange(new object[] {
                "Эллиптическая",  "Миндалевидная", "Спиральная"
            });
            m_type_combo.SelectedIndex = 2;

            m_anim_timer.Interval = 40;
            m_anim_timer.Tick += (s, e) => {
                m_time_val += 0.07;
                RenderGalaxy(m_time_val);
            };

            ApplyRounding();
            this.Text = "Galaxy Generator 2000";
        }

        private void MakeRound(Control ctrl, int rad)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, rad, rad, 180, 90);
            path.AddArc(ctrl.Width - rad, 0, rad, rad, 270, 90);
            path.AddArc(ctrl.Width - rad, ctrl.Height - rad, rad, rad, 0, 90);
            path.AddArc(0, ctrl.Height - rad, rad, rad, 90, 90);
            path.CloseFigure();
            ctrl.Region = new Region(path);
        }

        private void ApplyRounding()
        {
            MakeRound(m_btn_generate, 15);
            MakeRound(m_btn_anim, 15);
            MakeRound(m_btn_star_clr, 15);
            MakeRound(m_btn_bg_clr, 15);
            MakeRound(m_btn_save, 15);
            MakeRound(m_pic_box, 30);
        }

        private void RenderGalaxy(double current_t)
        {
            int w = m_pic_box.Width;
            int h = m_pic_box.Height;
            Bitmap frame = new Bitmap(w, h);

            using (Graphics g = Graphics.FromImage(frame))
            {
                // Сглаживание для красивых облаков
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Заливаем базовый черный фон
                g.Clear(m_bg_color);

                // ВСТАВЛЯЕМ ГЕНЕРАЦИЮ ЗВЕЗДНОГО НЕБА
                DrawSpaceBackground(g, w, h);
            }

            int cx = w / 2;
            int cy = h / 2;
            int rad = m_radius_scroll.Value;
            GalaxyType g_t = (GalaxyType)m_type_combo.SelectedIndex;
            Random rnd = new Random();

            // Далее идет твой цикл отрисовки самой галактики...
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    double dens = m_engine.GetDensity(x, y, cx, cy, rad, g_t, current_t);
                    if (dens < 0.01) continue;

                    // Рисуем звезды галактики поверх сгенерированного неба
                    if (rnd.NextDouble() < dens * 1.4)
                    {
                        // ... (твой код отрисовки звезд) ...
                        int r_v = (int)(m_star_color.R + (255 - m_star_color.R) * dens);
                        int g_v = (int)(m_star_color.G + (255 - m_star_color.G) * dens);
                        int b_v = (int)(m_star_color.B + (255 - m_star_color.B) * dens);

                        frame.SetPixel(x, y, Color.FromArgb(255, Math.Min(255, r_v), Math.Min(255, g_v), Math.Min(255, b_v)));
                    }
                }
            }

            if (m_pic_box.Image != null) m_pic_box.Image.Dispose();
            m_pic_box.Image = frame;
        }

        private void DrawSpaceBackground(Graphics g, int w, int h)
        {
            // Используем фиксированное число 42, чтобы звезды не меняли положение при анимации
            Random rnd = new Random(42);

            // 1. Сначала просто заливаем всё черным (или выбранным цветом фона)
            g.Clear(m_bg_color);

            // 2. Рисуем обычные звезды-точки
            // Сделаем около 250 штук
            for (int i = 0; i < 250; i++)
            {
                int x = rnd.Next(0, w);
                int y = rnd.Next(0, h);

                // Делаем разную яркость, чтобы не все были чисто белыми
                int bright = rnd.Next(100, 255);
                Color star_clr = Color.FromArgb(bright, bright, bright);

                // Большинство звезд - 1 пиксель, некоторые - чуть больше (2 пикселя)
                int size = 1;
                if (rnd.Next(10) > 8) size = 2; // Каждая 10-я звезда будет чуть крупнее

                using (SolidBrush sb = new SolidBrush(star_clr))
                {
                    // Рисуем звезду как маленький квадратик или кружок
                    g.FillRectangle(sb, x, y, size, size);
                }
            }
        }
        private void OnGenerateClick(object sender, EventArgs e) => RenderGalaxy(m_time_val);

        private void OnAnimToggle(object sender, EventArgs e)
        {
            m_is_animating = !m_is_animating;
            if (m_is_animating) m_anim_timer.Start(); else m_anim_timer.Stop();
            m_btn_anim.Text = m_is_animating? "СТОП" : "АНИМАЦИЯ";
        }

        private void OnSaveImage(object sender, EventArgs e)
        {
            if (m_pic_box.Image == null)
            {
                MessageBox.Show("Сначала сгенерируйте галактику!", "Внимание");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "BMP Image|*.bmp|PNG Image|*.png|JPEG Image|*.jpg";
                sfd.Title = "Сохранить вашу галактику";
                sfd.FileName = "my_galaxy_" + DateTime.Now.ToString("HHmmss");

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
            
                        m_pic_box.Image.Save(sfd.FileName);
                        MessageBox.Show("Галактика успешно сохранена!", "Готово");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при сохранении: " + ex.Message);
                    }
                }
            }
        }

        private void OnStarColorClick(object sender, EventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    m_star_color = dlg.Color;
                    m_btn_star_clr.BackColor = m_star_color;
                    UpdateBrightness(m_btn_star_clr, m_star_color);
                }
            }
        }

        private void OnBgColorClick(object sender, EventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    m_bg_color = dlg.Color;
                    m_btn_bg_clr.BackColor = m_bg_color;
                    m_pic_box.BackColor = m_bg_color;
                    UpdateBrightness(m_btn_bg_clr, m_bg_color);
                    RenderGalaxy(m_time_val); 
                }
            }
        }

        private void UpdateBrightness(Button btn, Color clr)
        {
            double lum = (0.299 * clr.R + 0.587 * clr.G + 0.114 * clr.B)/255;
            btn.ForeColor = lum > 0.7 ? Color.Black : Color.White;
        }

        private void btn_style(Button b, Color bc)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.BackColor = bc;
            b.ForeColor = Color.White;
            b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            b.FlatAppearance.BorderSize = 0;
        }

        
        private void InitializeComponent()
        {
            this.m_pic_box = new PictureBox();
            this.m_btn_generate = new Button();
            this.m_btn_anim = new Button();
            this.m_btn_star_clr = new Button();
            this.m_btn_bg_clr = new Button();
            this.m_btn_save = new Button();
            this.m_radius_scroll = new TrackBar();
            this.m_type_combo = new ComboBox();
            this.m_lbl_rad = new Label();
            this.m_lbl_type = new Label();

            this.m_pic_box.Location = new Point(25, 25);
            this.m_pic_box.Size = new Size(500, 500);
            this.m_pic_box.BackColor = Color.Black;

            Panel p_tools = new Panel { Location = new Point(550, 25), Size = new Size(270, 520), BackColor = Color.White };

            m_lbl_type.Text = "Тип:"; m_lbl_type.Location = new Point(20, 15);
            m_type_combo.Location = new Point(20, 35); m_type_combo.Size = new Size(230, 30);
            m_type_combo.DropDownStyle = ComboBoxStyle.DropDownList;

            m_lbl_rad.Text = "Радиус:"; m_lbl_rad.Location = new Point(20, 80);
            m_radius_scroll.Location = new Point(20, 100); m_radius_scroll.Size = new Size(230, 45);
            m_radius_scroll.Minimum = 50; m_radius_scroll.Maximum = 250; m_radius_scroll.Value = 150;

            this.m_btn_generate.Location = new Point(20, 180);
            this.m_btn_generate.Size = new Size(230, 45);
            this.m_btn_generate.Text = "ГЕНЕРИРОВАТЬ";
            this.btn_style(m_btn_generate, Color.FromArgb(123, 75, 255));
            this.m_btn_generate.Click += OnGenerateClick;

            this.m_btn_anim.Location = new Point(20, 235);
            this.m_btn_anim.Size = new Size(230, 45);
            this.m_btn_anim.Text = "АНИМАЦИЯ";
            this.btn_style(m_btn_anim, Color.FromArgb(123, 75, 255));
            this.m_btn_anim.Click += OnAnimToggle;

            this.m_btn_star_clr.Location = new Point(20, 310);
            this.m_btn_star_clr.Size = new Size(230, 40);
            this.m_btn_star_clr.Text = "ЦВЕТ ЗВЕЗД";
            this.btn_style(m_btn_star_clr, m_star_color);
            this.m_btn_star_clr.Click += OnStarColorClick;

            this.m_btn_bg_clr.Location = new Point(20, 370);
            this.m_btn_bg_clr.Size = new Size(230,  50);
            this.m_btn_bg_clr.Text = "ЦВЕТ ФОНА";
            this.btn_style(m_btn_bg_clr, m_bg_color);
            this.m_btn_bg_clr.Click += OnBgColorClick;



            this.m_btn_save.Location = new Point(20, 430);
            this.m_btn_save.Size = new Size(230, 45);
            this.m_btn_save.Text = "СОХРАНИТЬ КАРТИНКУ";
            this.btn_style(m_btn_save, Color.FromArgb(123, 75, 255));
            this.m_btn_save.Click += OnSaveImage; 

            p_tools.Controls.Add(m_lbl_type); p_tools.Controls.Add(m_type_combo);
            p_tools.Controls.Add(m_lbl_rad); p_tools.Controls.Add(m_radius_scroll);
            p_tools.Controls.Add(m_btn_generate); p_tools.Controls.Add(m_btn_anim);
            p_tools.Controls.Add(m_btn_star_clr); p_tools.Controls.Add(m_btn_bg_clr);
            p_tools.Controls.Add(m_btn_save);

            this.BackColor = Color.FromArgb(240, 240, 245);
            this.ClientSize = new Size(860, 580);
            this.Controls.Add(m_pic_box);
            this.Controls.Add(p_tools);
            this.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}