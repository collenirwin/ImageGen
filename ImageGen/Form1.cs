using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ImageGen
{
    public partial class Form1 : Form
    {
        #region Vars

        const int NUM_COLLUMNS = 6;
        const int DRAW_CHANCE = 50;

        Random ran = new Random();

        Color foreColor = Color.MediumSeaGreen;
        Color backColor = Color.White;

        Bitmap bmp;

        #endregion

        #region Methods

        public Form1()
        {
            InitializeComponent();

            btnForeColor.BackColor = foreColor;
            btnBackColor.BackColor = backColor;
        }

        #region Coloring

        private void btnForeColor_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
            foreColor = colorDialog.Color;
            btnForeColor.BackColor = foreColor;
        }

        private void btnBackColor_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
            backColor = colorDialog.Color;
            btnBackColor.BackColor = backColor;
        }

        private void chkRandom_CheckedChanged(object sender, EventArgs e)
        {
            btnForeColor.Enabled = !chkRandom.Checked;
            btnBackColor.Enabled = !chkRandom.Checked;
        }

        #endregion

        #region Drawing

        private Color randomColor()
        {
            return Color.FromArgb(255, ran.Next(256), ran.Next(256), ran.Next(256));
        }

        private void btnGen_Click(object sender, EventArgs e)
        {
            var tempBmp = new Bitmap(picture.Width / 2, picture.Height);

            using (var graphics = Graphics.FromImage(tempBmp))
            {
                Brush foreBrush;
                Brush backBrush;

                // determine brush colors
                if (chkRandom.Checked)
                {
                    foreBrush = new SolidBrush(randomColor());
                    backBrush = new SolidBrush(randomColor());
                }
                else
                {
                    foreBrush = new SolidBrush(foreColor);
                    backBrush = new SolidBrush(backColor);
                }

                // draw background
                graphics.FillRectangle(
                    backBrush, // color
                    0, // x
                    0, // y
                    tempBmp.Width, // width
                    tempBmp.Height // height
                );

                // set box dimensions and init x and y
                int boxWidth = picture.Width / NUM_COLLUMNS;
                int boxHeight = picture.Height / NUM_COLLUMNS;
                int xPos = 0;
                int yPos = 0;
                
                for (int x = 0; x < NUM_COLLUMNS; x++)
                {
                    for (int y = 0; y < NUM_COLLUMNS; y++)
                    {
                        // if we roll to draw a rectangle
                        if (ran.Next(1, 101) <= DRAW_CHANCE)
                        {
                            graphics.FillRectangle(foreBrush, xPos, yPos, boxWidth, boxHeight);
                        }

                        // go down
                        yPos += boxHeight;
                    }

                    // go right
                    xPos += boxWidth;

                    // reset Y
                    yPos = 0;
                }
            }

            // we just made a random image half the size of what we want
            // it looks like this: [
            // now we're going to make another image that's the same but flipped
            // it will look like this: ]

            // flip tempBmp horizontally, put it into bmpFlipped
            var tempBmpFlipped = new Bitmap(tempBmp);
            tempBmpFlipped.RotateFlip(RotateFlipType.RotateNoneFlipX);

            // create bmp with picture's dimensions
            bmp = new Bitmap(picture.Width, picture.Height);

            // now we're going to add the two images together
            // so we get the full image: []
            using (var graphics = Graphics.FromImage(bmp))
            {
                // draw tempBmp on the left
                graphics.DrawImage(tempBmp, 0, 0);

                // draw tempBmpFlipped on the right
                graphics.DrawImage(tempBmpFlipped, bmp.Width / 2, 0);
            }
            
            // display our beautiful bmp in picture.image
            picture.Image = bmp;
        }

        #endregion

        #region Saving

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (bmp != null)
            {
                saveFileDialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please generate an image first.");
                btnGen.Focus();
            }
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                bmp.Save(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to save your image with the following exception:\n" +
                    ex.Message
                );
            }
        }

        #endregion

        #endregion
    }
}
