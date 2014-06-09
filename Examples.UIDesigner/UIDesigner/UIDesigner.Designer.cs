namespace Examples.Classes
{
    partial class UIDesigner
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
            this.components = new System.ComponentModel.Container();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.but_add = new System.Windows.Forms.Button();
            this.but_remove = new System.Windows.Forms.Button();
            this.but_gencode = new System.Windows.Forms.Button();
            this.but_close = new System.Windows.Forms.Button();
            this.but_cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.but_copytex = new System.Windows.Forms.Button();
            this.but_up = new System.Windows.Forms.Button();
            this.but_down = new System.Windows.Forms.Button();
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(12, 26);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.Size = new System.Drawing.Size(494, 149);
            this.dgv.TabIndex = 0;
            // 
            // but_add
            // 
            this.but_add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.but_add.Location = new System.Drawing.Point(12, 181);
            this.but_add.Name = "but_add";
            this.but_add.Size = new System.Drawing.Size(99, 23);
            this.but_add.TabIndex = 1;
            this.but_add.Text = "Add textures...";
            this.tt.SetToolTip(this.but_add, "Add new textures from image files");
            this.but_add.UseVisualStyleBackColor = true;
            // 
            // but_remove
            // 
            this.but_remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.but_remove.Location = new System.Drawing.Point(117, 181);
            this.but_remove.Name = "but_remove";
            this.but_remove.Size = new System.Drawing.Size(99, 23);
            this.but_remove.TabIndex = 2;
            this.but_remove.Text = "Remove textures";
            this.tt.SetToolTip(this.but_remove, "Remove selected textures");
            this.but_remove.UseVisualStyleBackColor = true;
            // 
            // but_gencode
            // 
            this.but_gencode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.but_gencode.Location = new System.Drawing.Point(441, 181);
            this.but_gencode.Name = "but_gencode";
            this.but_gencode.Size = new System.Drawing.Size(107, 23);
            this.but_gencode.TabIndex = 3;
            this.but_gencode.Text = "Generate code";
            this.tt.SetToolTip(this.but_gencode, "Generate rectangles code and paste it into the clipboard");
            this.but_gencode.UseVisualStyleBackColor = true;
            // 
            // but_close
            // 
            this.but_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.but_close.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.but_close.Location = new System.Drawing.Point(311, 235);
            this.but_close.Name = "but_close";
            this.but_close.Size = new System.Drawing.Size(156, 23);
            this.but_close.TabIndex = 4;
            this.but_close.Text = "Close and Update";
            this.but_close.UseVisualStyleBackColor = true;
            // 
            // but_cancel
            // 
            this.but_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.but_cancel.Location = new System.Drawing.Point(473, 235);
            this.but_cancel.Name = "but_cancel";
            this.but_cancel.Size = new System.Drawing.Size(75, 23);
            this.but_cancel.TabIndex = 5;
            this.but_cancel.Text = "Cancel";
            this.but_cancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, -2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(394, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Click on the cells in Name or Rectangle columns to change object name or coords";
            // 
            // but_copytex
            // 
            this.but_copytex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.but_copytex.Location = new System.Drawing.Point(222, 181);
            this.but_copytex.Name = "but_copytex";
            this.but_copytex.Size = new System.Drawing.Size(99, 23);
            this.but_copytex.TabIndex = 7;
            this.but_copytex.Text = "Copy textures";
            this.tt.SetToolTip(this.but_copytex, "Make a copies of selected textures");
            this.but_copytex.UseVisualStyleBackColor = true;
            // 
            // but_up
            // 
            this.but_up.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.but_up.Location = new System.Drawing.Point(512, 26);
            this.but_up.Name = "but_up";
            this.but_up.Size = new System.Drawing.Size(36, 54);
            this.but_up.TabIndex = 8;
            this.but_up.Text = "Up";
            this.tt.SetToolTip(this.but_up, "Move selected texture back in rendering order");
            this.but_up.UseVisualStyleBackColor = true;
            // 
            // but_down
            // 
            this.but_down.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.but_down.Location = new System.Drawing.Point(512, 86);
            this.but_down.Name = "but_down";
            this.but_down.Size = new System.Drawing.Size(36, 54);
            this.but_down.TabIndex = 9;
            this.but_down.Text = "Down";
            this.tt.SetToolTip(this.but_down, "Move selected texture forward in rendering order");
            this.but_down.UseVisualStyleBackColor = true;
            // 
            // tt
            // 
            this.tt.IsBalloon = true;
            this.tt.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(531, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Draw order: the last one is drawn above all the others. You can change the order " +
    "by using Up or Down buttons.";
            // 
            // UIDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 270);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.but_down);
            this.Controls.Add(this.but_up);
            this.Controls.Add(this.but_copytex);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.but_cancel);
            this.Controls.Add(this.but_close);
            this.Controls.Add(this.but_gencode);
            this.Controls.Add(this.but_remove);
            this.Controls.Add(this.but_add);
            this.Controls.Add(this.dgv);
            this.MinimumSize = new System.Drawing.Size(576, 309);
            this.Name = "UIDesigner";
            this.Text = "UIDesigner control window";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button but_add;
        private System.Windows.Forms.Button but_remove;
        private System.Windows.Forms.Button but_gencode;
        private System.Windows.Forms.Button but_close;
        private System.Windows.Forms.Button but_cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button but_copytex;
        private System.Windows.Forms.Button but_up;
        private System.Windows.Forms.Button but_down;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.Label label2;
    }
}