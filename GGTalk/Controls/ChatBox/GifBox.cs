using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GGTalk.Controls
{ 
    public class GifBox : Control
    {
        #region 变量
        private Image currentImage = null;
        private Rectangle currentImageRectangle = Rectangle.Empty;
        private Size currentImageSize = Size.Empty;        
        private bool isAnimate = false;
        private Color _borderColor = Color.Transparent;
        private EventHandler _eventAnimator;

        #endregion

        #region 构造函数

        public GifBox()
            : base()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.CacheText |
                ControlStyles.ResizeRedraw, true);

            SetStyle(ControlStyles.Opaque, false);

            this.Cursor = Cursors.Arrow;
        }

        #endregion

        #region 属性        
        public Image Image
        {
            get { return this.currentImage; }
            set
            {
                StopAnimate();
                this.currentImage = value;
                this.currentImageSize = this.currentImage == null ? Size.Empty : this.currentImage.Size;
                this.ComputeImageRectangle();
                this.isAnimate = false;
                if (value != null)
                {
                    this.isAnimate = ImageAnimator.CanAnimate(this.currentImage);                    
                }
              
                Invalidate(this.currentImageRectangle);
                if (!DesignMode)
                {
                    StartAnimate();
                }
            }
        }       

        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                base.Invalidate();
            }
        }

        private void ComputeImageRectangle()
        {
            if (this.currentImage == null)
            {
                this.currentImageRectangle = Rectangle.Empty;              
                return;
            }
            
            if (this.currentImageSize.Width <= this.Width && this.currentImageSize.Height <= this.Height)
            {
                this.currentImageRectangle.X = (Width - this.currentImageSize.Width) / 2;
                this.currentImageRectangle.Y = (Height - this.currentImageSize.Height) / 2;
                this.currentImageRectangle.Width = this.currentImageSize.Width;
                this.currentImageRectangle.Height = this.currentImageSize.Height;
            }
            else if (this.currentImageSize.Width > this.Width)
            {
                int newImgHeight = this.Width * this.currentImageSize.Height / this.currentImageSize.Width; ;
                this.currentImageRectangle = new Rectangle(0, 0, this.Width, newImgHeight);
            }
            else
            {
                int newImgWidth = this.Height * this.currentImageSize.Width / this.currentImageSize.Height;
                this.currentImageRectangle = new Rectangle(0, 0, newImgWidth, this.Height);
            }
        }

        private bool CanAnimate
        {
            get { return isAnimate; }
        }

        private EventHandler EventAnimator
        {
            get
            {
                if (_eventAnimator == null)
                    _eventAnimator = delegate(object sender, EventArgs e)
                    {
                        Invalidate(this.currentImageRectangle);
                    };
                return _eventAnimator;
            }
        }

        #endregion

        #region Override

        protected override void OnSizeChanged(EventArgs e)
        {
            this.ComputeImageRectangle();
            base.OnSizeChanged(e);
        }
               
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            if (currentImage != null)
            {
                UpdateImage();
                e.Graphics.DrawImage(this.currentImage, this.currentImageRectangle, 0, 0, this.currentImageSize.Width, this.currentImageSize.Height, GraphicsUnit.Pixel);
            }

            ControlPaint.DrawBorder(
                    e.Graphics,
                    ClientRectangle,
                    _borderColor,
                    ButtonBorderStyle.Solid);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _eventAnimator = null;
                isAnimate = false;
                if (currentImage != null)
                    currentImage = null;
            }

        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            StopAnimate();
        }

        #endregion

        #region Private Method

        private void StartAnimate()
        {
            if (CanAnimate)
            {
                ImageAnimator.Animate(currentImage, EventAnimator);
            }
        }

        private void StopAnimate()
        {
            if (CanAnimate)
            {
                ImageAnimator.StopAnimate(currentImage, EventAnimator);
            }
        }
       
        private void UpdateImage()
        {
            if (CanAnimate)
            {
                ImageAnimator.UpdateFrames(currentImage);
            }
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GifBox
            // 
            
            this.ResumeLayout(false);
        }
    }
}
