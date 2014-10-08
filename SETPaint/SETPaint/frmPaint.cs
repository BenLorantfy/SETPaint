﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace SETPaint
{
    public partial class frmPaint : Form
    {
        int mouseX;
        int mouseY;
        string tool;
        Draw draw;
        FileIO fileIO;
        bool undo;
        bool checkPainting = false;
        public frmPaint()
        {           
            InitializeComponent();
            draw = new Draw();
            fileIO = new FileIO();
            mouseX = 0;
            mouseY = 0;
            tool = "none";
            undo = false;
        }
        private void mousePosition_display()
        {
            if (checkPainting)
            {
                mousePosition.Text = "X: " + this.PointToClient(Control.MousePosition).X + "    Y: " + this.PointToClient(Control.MousePosition).Y;
            }

            else
            {
                mousePosition.Text = "";
            }
        }
        private void pnlCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            mousePosition_display();
            mouseX = e.X;
            mouseY = e.Y;

            if (tool == "drawingLine" || tool == "drawingRectangle" || tool == "drawingEllipse")
            {
                pnlCanvas.Invalidate();
            }
        }

        private void pnlCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            checkPainting = true;
            txtStrokeWidth_Leave(sender, e);
            draw.Start(mouseX, mouseY);

            switch (tool)
            {
                case "line":
                    tool = "drawingLine";
                    break;

                case "rectangle":
                    tool = "drawingRectangle";
                    break;

                case "ellipse":
                    tool = "drawingEllipse";
                    break;
            }
            
        }

        private void pnlCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            checkPainting = false;

            switch (tool)
            {
                case "drawingLine":
                    tool = "stopDrawingLine";
                    pnlCanvas.Invalidate();
                    break;

                case "drawingRectangle":
                    tool = "stopDrawingRectangle";
                    pnlCanvas.Invalidate();
                    break;

                case "drawingEllipse":
                    tool = "stopDrawingEllipse";
                    pnlCanvas.Invalidate();
                    break;
            }

            
        }

        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (undo)
            {
                draw.Undo(e.Graphics);
                undo = false;
            }

            //
            // Do all drawing here
            //     
            switch (tool){
                case "drawingLine":
                    draw.Line(e.Graphics, mouseX, mouseY);
                    break;
                
                case "drawingRectangle":
                    draw.Rectangle(e.Graphics, mouseX, mouseY);
                    break;

                case "drawingEllipse":
                    draw.Ellipse(e.Graphics, mouseX, mouseY);
                    break;

                case "stopDrawingLine":
                    draw.End(e.Graphics);
                    tool = "line";
                    break;
                
                case "stopDrawingRectangle":
                    draw.End(e.Graphics);
                    tool = "rectangle";
                    break;
                
                case "stopDrawingEllipse":
                    draw.End(e.Graphics);
                    tool = "ellipse";
                    break;

                case "clearing":
                    draw.ClearAndReset(e.Graphics);
                    tool = "none";
                    break;

                default:
                    draw.Redraw(e.Graphics);
                    break;

            }
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            tool = "line";
            txtStrokeWidth_Leave(sender, e);
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            tool = "rectangle";
            txtStrokeWidth_Leave(sender, e);
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            tool = "ellipse";
            txtStrokeWidth_Leave(sender, e);
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undo = true;
            pnlCanvas.Invalidate();
        }
        
        private void btnSwitch_Click(object sender, EventArgs e)
        {
            Color fill = pnlFillColor.BackColor;
            Color stroke = pnlStrokeColor.BackColor;

            pnlFillColor.BackColor = stroke;
            pnlStrokeColor.BackColor = fill;

            draw.SetStrokeColor(fill);
            draw.SetFillColor(stroke);
        }

        private void pnlFillColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorPicker = new ColorDialog();
            colorPicker.ShowDialog();
            Color color = colorPicker.Color;
            pnlFillColor.BackColor = color;
            draw.SetFillColor(color);
        }

        private void pnlStrokeColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorPicker = new ColorDialog();
            colorPicker.ShowDialog();
            Color color = colorPicker.Color;
            pnlStrokeColor.BackColor = color;
            draw.SetStrokeColor(color);
        }

        private void trkStrokeWidth_Scroll(object sender, EventArgs e)
        {
            int width = trkStrokeWidth.Value;
            draw.SetStrokeWidth(width);
            txtStrokeWidth.Text = Convert.ToString(width);
        }

        private void txtStrokeWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;          //Stops ding when enter is pressed
                txtStrokeWidth_Leave(sender, e);
            }
        }

        private void txtStrokeWidth_Leave(object sender, EventArgs e)
        {
            //
            // Tries to convert textbox value to integer
            // If string is not a valid integer, it sets the width to 1
            //
            string textBoxValue = txtStrokeWidth.Text;
            int width = 1;
            if (!int.TryParse(textBoxValue, out width) || width < 1)
            {
                width = 1;
                txtStrokeWidth.Text = "1";
            }
            else if (width > 100)
            {
                width = 100;
                txtStrokeWidth.Text = "100";
            }

            if (width <= 20)
            {
                trkStrokeWidth.Value = width;
            }
            else
            {
                trkStrokeWidth.Value = 20;
            }
            
            draw.SetStrokeWidth(width);           
        }

        private void eraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to delete entire image?", "Clear Image", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                tool = "clearing";
                pnlCanvas.Invalidate();
            } 
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveDialog.FileName.ToString();
                fileIO.Save(fileName, draw.Shapes());
            }
            
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //fileIO.Export("test", draw.Shapes()); //Delete this if its not implemented
            Point screenLocation = PointToScreen(pnlCanvas.Location);
            Bitmap bmp = new Bitmap(pnlCanvas.Width, pnlCanvas.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(new Point(screenLocation.X, screenLocation.Y), new Point(pnlCanvas.Location.X, pnlCanvas.Location.Y), new Size(pnlCanvas.Width, pnlCanvas.Height));

            bool isSave = true;
            SaveFileDialog sfdlg = new SaveFileDialog();
            sfdlg.Title = "Save Image";
            sfdlg.Filter = @"jpeg|*.jpg|bmp|*.bmp|gif|*.gif";
            if (sfdlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = sfdlg.FileName.ToString();

                if (fileName != "" && fileName != null)
                {
                    string fileExtName = fileName.Substring(fileName.LastIndexOf(".") + 1).ToString();
                    System.Drawing.Imaging.ImageFormat imgformat = null;

                    if (fileExtName != "")
                    {
                        switch (fileExtName)
                        {
                            case "jpg":
                                imgformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                                break;
                            case "bmp":
                                imgformat = System.Drawing.Imaging.ImageFormat.Bmp;
                                break;
                            case "gif":
                                imgformat = System.Drawing.Imaging.ImageFormat.Gif;
                                break;
                            default:
                                MessageBox.Show("Only save as: jpg,bmp,gif");
                                isSave = false;
                                break;
                        }

                    }
                    if (imgformat == null)
                    {
                        imgformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    }

                    if (isSave)
                    {
                        try
                        {
                            bmp.Save(fileName, imgformat);
                            MessageBox.Show("Export Success!");
                        }
                        catch
                        {
                            MessageBox.Show("Failed");
                        }
                    }
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openDialog.FileName.ToString();
                fileIO.Open(fileName, draw.Shapes());
                tool = "none";
                pnlCanvas.Invalidate();
            }
            
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("SET Paint" + "\n" + "Ben Lorantfy" + "\n" + "Chuang Liu" + "\n" + "Version 1.2" + "\n" + "Conestoga College");
        }


        

       
    }
}
