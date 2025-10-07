using System.Drawing;

namespace LiveSplit.UI.Components
{
    partial class MultiNameDetailsSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanelRow = new System.Windows.Forms.TableLayoutPanel();

            this.lblNth = new System.Windows.Forms.Label();
            this.chkIsVisible = new System.Windows.Forms.CheckBox();
            this.dmnDisplayTime = new System.Windows.Forms.NumericUpDown();
            this.btnTextColor = new System.Windows.Forms.Button();
            this.btnTextFont = new System.Windows.Forms.Button();

            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            //this.btnRemove = new System.Windows.Forms.Button();

            this.tableLayoutPanelRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dmnDisplayTime)).BeginInit();
            this.SuspendLayout();

            // 
            // tableLayoutPanelRow
            // 
            this.tableLayoutPanelRow.ColumnCount = 8;
            this.tableLayoutPanelRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F)); //Nth
            this.tableLayoutPanelRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F)); //IsVisible
            this.tableLayoutPanelRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F)); //DisplayTime
            this.tableLayoutPanelRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F)); //TextColor
            this.tableLayoutPanelRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 164F)); //Font
            this.tableLayoutPanelRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F)); //MoveUp
            this.tableLayoutPanelRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F)); //MoveDown
            this.tableLayoutPanelRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F)); //Clear
            this.tableLayoutPanelRow.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanelRow.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelRow.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelRow.Name = "tableLayoutPanelRow";
            this.tableLayoutPanelRow.RowCount = 1;
            this.tableLayoutPanelRow.ForeColor = Color.Black;
            this.tableLayoutPanelRow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanelRow.Size = new System.Drawing.Size(433, 27);
            this.tableLayoutPanelRow.Controls.Add(this.lblNth, 0, 0);
            this.tableLayoutPanelRow.Controls.Add(this.chkIsVisible, 1, 0);
            this.tableLayoutPanelRow.Controls.Add(this.dmnDisplayTime, 2, 0);
            this.tableLayoutPanelRow.Controls.Add(this.btnTextColor, 3, 0);
            this.tableLayoutPanelRow.Controls.Add(this.btnTextFont, 4, 0);
            this.tableLayoutPanelRow.Controls.Add(this.btnMoveUp, 5, 0);
            this.tableLayoutPanelRow.Controls.Add(this.btnMoveDown, 6, 0);
            this.tableLayoutPanelRow.Controls.Add(this.btnReset, 7, 0);
            //this.tableLayoutPanelRow.Controls.Add(this.btnRemove, 7, 0);
            // 
            // lblNth
            // 
            this.lblNth.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNth.Name = "lblNth";
            this.lblNth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkIsVisible
            // 
            this.chkIsVisible.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkIsVisible.Name = "chkIsVisible";
            this.chkIsVisible.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.chkIsVisible.UseVisualStyleBackColor = true;
            this.chkIsVisible.CheckedChanged += new System.EventHandler(this.chkIsVisible_CheckedChanged);
            this.chkIsVisible.Checked = true;
            // 
            // dmnDisplayTime
            // 
            this.dmnDisplayTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.dmnDisplayTime.ForeColor = Color.Gray;
            this.dmnDisplayTime.DecimalPlaces = 1;
            this.dmnDisplayTime.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.dmnDisplayTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.dmnDisplayTime.Name = "dmnDisplayTime";
            this.dmnDisplayTime.Size = new System.Drawing.Size(93, 19);
            this.dmnDisplayTime.TabIndex = 0;
            dmnDisplayTime.ValueChanged += dmnDisplayTime_ValueChanged;
            // 
            // btnTextColor
            // 
            this.btnTextColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTextColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTextFont.AutoSize = false;
            this.btnTextColor.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnTextColor.Name = "btnTextColor";
            this.btnTextColor.Location = new System.Drawing.Point(0, 0);
            this.btnTextColor.Size = new System.Drawing.Size(21, 21);
            this.btnTextColor.UseVisualStyleBackColor = false;
            this.btnTextColor.Click += new System.EventHandler(this.btnTextColor_Click);
            // 
            // btnTextFont
            // 
            this.btnTextFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTextFont.Name = "btnTextFont";
            this.btnTextFont.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.btnTextFont.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0); 
            this.btnTextFont.AutoSize = true;
            this.btnTextFont.ForeColor = Color.Gray;
            this.btnTextFont.Text = "(choose font)";
            this.btnTextFont.UseVisualStyleBackColor = true;
            this.btnTextFont.Click += new System.EventHandler(this.btnTextFont_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveUp.AutoSize = true;
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Text = "▲";
            this.btnMoveUp.Font = new System.Drawing.Font(this.btnMoveUp.Font.FontFamily, 8.2F);
            this.btnMoveUp.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.btnMoveUp.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveDown.AutoSize = true;
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Text = "▼";
            this.btnMoveDown.Font = new System.Drawing.Font(this.btnMoveDown.Font.FontFamily, 8.2F);
            this.btnMoveDown.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.btnMoveDown.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnReset.AutoSize = false;
            this.btnReset.Name = "btnReset";
            this.btnReset.Text = "Reset";
            this.btnReset.Margin = new System.Windows.Forms.Padding(0, 1, 2, 0);
            this.btnReset.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);

            // 
            // MultiNameDetailsSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelRow);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MultiNameDetailsSettings";
            this.Size = new System.Drawing.Size(435, 27);
            this.Load += new System.EventHandler(MultiNameDetailsSettings_Load);
            this.tableLayoutPanelRow.ResumeLayout(false);
            this.tableLayoutPanelRow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dmnDisplayTime)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRow;
        private System.Windows.Forms.Label lblNth;
        private System.Windows.Forms.CheckBox chkIsVisible;
        private System.Windows.Forms.NumericUpDown dmnDisplayTime;
        private System.Windows.Forms.Button btnTextColor;
        private System.Windows.Forms.Button btnTextFont;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnReset;
    }
}
