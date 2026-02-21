using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;


namespace TextBox
{
    #region ENUMS

    public enum InnerIconType
    {
        None,
        Email,
        User,
        Lock,
        Phone,
        Search,
        Url
    }

    public enum InnerIconPosition
    {
        Left,
        Right
    }

    public enum ValidationMode
    {
        None,
        CampoObrigatorio,
        Email,
        NIF,
        NIB,
        Telemovel,
        URL,
        Numero,
        TextoSemEspaco,
        CustomRegex
    }

    #endregion

    public class SmartTextBox : UserControl
    {
        private bool showErrorVisual = false;
        private Timer errorTimer = new Timer();

        private System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();
        private Timer animationTimer = new Timer();
        private ToolTip toolTip = new ToolTip();

        private const string IconFont = "Segoe Fluent Icons";

        private InnerIconType innerIcon = InnerIconType.None;
        private InnerIconPosition innerIconPosition = InnerIconPosition.Left;
        private ValidationMode validationMode = ValidationMode.None;

        private int borderRadius = 10;
        private int borderThickness = 1;
        private int iconTextSpacing = 12;

        private bool showBorderFocus = true;
        private bool autoValidationMode = true;
        private bool showErrorIcon = true;

        private int timeToolTipText = 5;

        private Color borderColor = Color.Black;
        private Color borderFocusColor = Color.FromArgb(0, 120, 215);
        private Color borderErrorColor = Color.Red;

        private bool isValid = true;
        private bool isFocused = false;

        private float focusProgress = 0f;
        private float errorFadeProgress = 0f;

        private string customRegexPattern = "";
        private string validationMessage = "Valor inválido.";
        private string errorMessage = "Valor inválido.";
        private string focusMessage = "";

        #region PROPERTIES

        [Browsable(true)]
        public override string Text
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                textBox.Font = value;
                AdjustLayout();
            }
        }

        public override Color ForeColor
        {
            get => textBox.ForeColor;
            set => textBox.ForeColor = value;
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                textBox.BackColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public int BorderRadius { get => borderRadius; set { borderRadius = value; Invalidate(); } }

        [Category("Appearance")]
        public int BorderThickness { get => borderThickness; set { borderThickness = value; Invalidate(); } }

        [Category("Appearance")]
        public Color BorderColor { get => borderColor; set { borderColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color BorderFocusColor { get => borderFocusColor; set { borderFocusColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color BorderErrorColor { get => borderErrorColor; set { borderErrorColor = value; Invalidate(); } }

        [Category("Appearance")]
        public bool ShowBorderFocus { get => showBorderFocus; set { showBorderFocus = value; Invalidate(); } }

        [Category("Appearance")]
        public bool ShowErrorIcon { get => showErrorIcon; set { showErrorIcon = value; Invalidate(); } }

        [Category("Custom")]
        public InnerIconType InnerIcon { get => innerIcon; set { innerIcon = value; AdjustLayout(); Invalidate(); } }

        [Category("Custom")]
        public InnerIconPosition InnerIconPosition { get => innerIconPosition; set { innerIconPosition = value; AdjustLayout(); Invalidate(); } }

        [Category("Validation")]
        public ValidationMode ValidationMode { get => validationMode; set => validationMode = value; }

        [Category("Validation")]
        public bool AutoValidationMode { get => autoValidationMode; set => autoValidationMode = value; }

        [Category("Validation")]
        public string CustomRegexPattern { get => customRegexPattern; set => customRegexPattern = value; }

        [Category("Validation")]
        public string ValidationMessage { get => validationMessage; set => validationMessage = value; }

        [Category("Validation")]
        public string ErrorMessage { get => errorMessage; set => errorMessage = value; }

        [Category("Behavior")]
        public string FocusMessage { get => focusMessage; set => focusMessage = value; }

        [Category("Behavior")]
        public int TimeToolTipText { get => timeToolTipText; set => timeToolTipText = value < 1 ? 1 : value; }

        [Browsable(false)]
        public bool IsValid => isValid;

        #endregion

        #region CONSTRUCTOR

        public SmartTextBox()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            Height = 40;

            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = Color.White;

            textBox.TextChanged += OnTextChanged;
            textBox.KeyDown += OnKeyDown;
            textBox.Leave += OnLeave;
            textBox.GotFocus += OnGotFocus;
            textBox.LostFocus += OnLostFocus;

            Controls.Add(textBox);

            animationTimer.Interval = 15;
            animationTimer.Tick += Animate;

            Resize += (s, e) => AdjustLayout();

            DoubleBuffered = true;
            Height = 40;

            borderColor = Color.Black;
            borderErrorColor = Color.Red;
            borderFocusColor = Color.FromArgb(0, 120, 215);

            BackColor = Color.White;
            textBox.BackColor = Color.White;
            textBox.ForeColor = Color.Black;

            errorTimer.Tick += (s, e) =>
            {
                errorTimer.Stop();
                showErrorVisual = false;
                animationTimer.Start();
                Invalidate();
            };

            AdjustLayout();
        }
        private void OnGotFocus(object sender, EventArgs e)
        {
            isFocused = true;

            if (!string.IsNullOrWhiteSpace(focusMessage))
            {
                toolTip.Show(
                    focusMessage,
                    this,
                    Width / 2,
                    Height,
                    timeToolTipText * 1000);
            }

            animationTimer.Start();
        }

        private void OnLostFocus(object sender, EventArgs e)
        {
            isFocused = false;
            animationTimer.Start();
        }

        #endregion

        #region VALIDATION

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (autoValidationMode)
                ValidateText();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!autoValidationMode && e.KeyCode == Keys.Enter)
            {
                ValidateText();
                e.SuppressKeyPress = true;
            }
        }

        private void OnLeave(object sender, EventArgs e)
        {
            if (!autoValidationMode)
                ValidateText();
        }

        private void ValidateText()
        {
            if (validationMode == ValidationMode.None)
            {
                isValid = true;
                toolTip.Hide(this);
                Invalidate();
                return;
            }

            if (validationMode == ValidationMode.CampoObrigatorio)
            {
                isValid = !string.IsNullOrWhiteSpace(textBox.Text);
            }
            else
            {
                string pattern = GetPattern();
                isValid = string.IsNullOrEmpty(pattern) ||
                          Regex.IsMatch(textBox.Text ?? "", pattern);
            }

            if (!isValid)
            {
                showErrorVisual = true;

                toolTip.Show(
                    autoValidationMode ? validationMessage : errorMessage,
                    this,
                    Width / 2,
                    Height,
                    timeToolTipText * 1000);

                errorTimer.Interval = timeToolTipText * 1000;
                errorTimer.Start();
            }
            else
            {
                showErrorVisual = false;
                toolTip.Hide(this);
            }

            animationTimer.Start();
            
            AdjustLayout();

            Invalidate();
        }

        private string GetPattern()
        {
            switch (validationMode)
            {
                case ValidationMode.Email: return @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                case ValidationMode.NIF: return @"^\d{9}$";
                case ValidationMode.NIB: return @"^\d{21}$";
                case ValidationMode.Telemovel: return @"^9\d{8}$";
                case ValidationMode.URL: return @"^(https?:\/\/)?([\w\-]+\.)+[\w\-]+";
                case ValidationMode.Numero: return @"^\d+$";
                case ValidationMode.TextoSemEspaco: return @"^\S+$";
                case ValidationMode.CustomRegex: return customRegexPattern;
                default: return null;
            }
        }

        #endregion

        #region ANIMATION

        private void Animate(object sender, EventArgs e)
        {
            float speed = 0.1f;

            focusProgress = isFocused
                ? Math.Min(1f, focusProgress + speed)
                : Math.Max(0f, focusProgress - speed);

            errorFadeProgress = !isValid
                ? Math.Min(1f, errorFadeProgress + speed)
                : Math.Max(0f, errorFadeProgress - speed);

            if ((focusProgress == 0f || focusProgress == 1f) &&
                (errorFadeProgress == 0f || errorFadeProgress == 1f))
                animationTimer.Stop();

            Invalidate();
        }

        #endregion

        #region LAYOUT

        private void AdjustLayout()
        {
            int padding = 12;
            int iconSize = (int)(Height * 0.55f);

            int left = padding;
            int right = padding;

            if (innerIcon != InnerIconType.None)
            {
                if (innerIconPosition == InnerIconPosition.Left)
                    left += iconSize + iconTextSpacing;
                else
                    right += iconSize + iconTextSpacing;
            }

            if (showErrorVisual && showErrorIcon)
            {
                if (innerIcon == InnerIconType.None)
                {
                    right += iconSize + 6;
                }
                else
                {
                    if (innerIconPosition == InnerIconPosition.Left)
                        right += iconSize + 6;
                    else
                        left += iconSize + 6;
                }
            }

            textBox.Location = new Point(left, (Height - textBox.Font.Height) / 2);
            textBox.Width = Width - left - right;
        }

        #endregion

        #region PAINT

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (SolidBrush brush =
                   new SolidBrush(Parent?.BackColor ?? SystemColors.Control))
                e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(
                borderThickness,
                borderThickness,
                Width - borderThickness * 2 - 1,
                Height - borderThickness * 2 - 1);

            using (GraphicsPath path = GetRoundedRect(rect, borderRadius))
            using (SolidBrush brush = new SolidBrush(BackColor))
                e.Graphics.FillPath(brush, path);

            Color border = !isValid
                ? borderErrorColor
                : (showBorderFocus
                    ? InterpolateColor(borderColor, borderFocusColor, focusProgress)
                    : borderColor);

            using (Pen pen = new Pen(border, borderThickness))
            using (GraphicsPath path = GetRoundedRect(rect, borderRadius))
                e.Graphics.DrawPath(pen, path);

            DrawInnerIcon(e.Graphics);
            DrawErrorIcon(e.Graphics);
        }

        private void DrawInnerIcon(Graphics g)
        {
            if (innerIcon == InnerIconType.None) return;

            float size = Height * 0.5f;

            using (Font font = new Font(IconFont, size, GraphicsUnit.Pixel))
            using (Brush brush = new SolidBrush(Color.Gray))
            {
                string glyph = GetGlyph(innerIcon);
                SizeF s = g.MeasureString(glyph, font);

                float x = innerIconPosition == InnerIconPosition.Left
                    ? 12
                    : Width - s.Width - 12;

                float y = (Height - s.Height) / 2;

                g.DrawString(glyph, font, brush, x, y);
            }
        }

        private void DrawErrorIcon(Graphics g)
        {
            if (isValid || !showErrorVisual || !showErrorIcon)
                return;

            float size = Height * 0.55f;
            float padding = 12;

            float x = innerIconPosition == InnerIconPosition.Left
                ? Width - size - padding
                : padding;

            float y = (Height - size) / 2;

            int alpha = (int)(255 * errorFadeProgress);

            using (GraphicsPath triangle = new GraphicsPath())
            {
                triangle.AddPolygon(new[]
                {
                    new PointF(x + size/2, y),
                    new PointF(x + size, y + size),
                    new PointF(x, y + size)
                });

                using (SolidBrush brush =
                       new SolidBrush(Color.FromArgb(alpha, Color.Goldenrod)))
                    g.FillPath(brush, triangle);
            }

            using (SolidBrush ex =
                   new SolidBrush(Color.FromArgb(alpha, Color.Black)))
            {
                float lw = size * 0.1f;
                g.FillRectangle(ex,
                    x + size / 2 - lw / 2,
                    y + size * 0.3f,
                    lw,
                    size * 0.4f);

                g.FillEllipse(ex,
                    x + size / 2 - lw / 2,
                    y + size * 0.8f,
                    lw,
                    lw);
            }
        }

        #endregion

        #region HELPERS

        private GraphicsPath GetRoundedRect(Rectangle r, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Color InterpolateColor(Color a, Color b, float t)
        {
            return Color.FromArgb(
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        private string GetGlyph(InnerIconType icon)
        {
            switch (icon)
            {
                case InnerIconType.Email: return "\uE715";
                case InnerIconType.User: return "\uE77B";
                case InnerIconType.Lock: return "\uE72E";
                case InnerIconType.Phone: return "\uE717";
                case InnerIconType.Search: return "\uE721";
                case InnerIconType.Url: return "\uE774";
                default: return "";
            }
        }

        #endregion
    }
}