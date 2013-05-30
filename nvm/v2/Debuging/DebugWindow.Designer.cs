namespace nvm.v2.Debuging
{
    partial class DebugWindow
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
            this.label1 = new System.Windows.Forms.Label();
            this.varList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NumIp = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.stkCnt = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtopCode = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.stackTop = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NumIp)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "Debugger";
            // 
            // varList
            // 
            this.varList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.varList.Font = new System.Drawing.Font("Consolas", 12F);
            this.varList.FormattingEnabled = true;
            this.varList.ItemHeight = 19;
            this.varList.Location = new System.Drawing.Point(12, 86);
            this.varList.Name = "varList";
            this.varList.Size = new System.Drawing.Size(240, 384);
            this.varList.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "Locals:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 12F);
            this.label3.Location = new System.Drawing.Point(321, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 19);
            this.label3.TabIndex = 3;
            this.label3.Text = "IP:";
            // 
            // NumIp
            // 
            this.NumIp.Location = new System.Drawing.Point(363, 13);
            this.NumIp.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.NumIp.Name = "NumIp";
            this.NumIp.Size = new System.Drawing.Size(123, 20);
            this.NumIp.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 12F);
            this.label4.Location = new System.Drawing.Point(321, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 19);
            this.label4.TabIndex = 5;
            this.label4.Text = "Stack Count:";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // stkCnt
            // 
            this.stkCnt.AutoSize = true;
            this.stkCnt.Font = new System.Drawing.Font("Consolas", 12F);
            this.stkCnt.Location = new System.Drawing.Point(444, 56);
            this.stkCnt.Name = "stkCnt";
            this.stkCnt.Size = new System.Drawing.Size(36, 19);
            this.stkCnt.TabIndex = 6;
            this.stkCnt.Text = "(0)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Consolas", 12F);
            this.label5.Location = new System.Drawing.Point(294, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 19);
            this.label5.TabIndex = 7;
            this.label5.Text = "Current opcode:";
            // 
            // txtopCode
            // 
            this.txtopCode.AutoSize = true;
            this.txtopCode.Font = new System.Drawing.Font("Consolas", 12F);
            this.txtopCode.Location = new System.Drawing.Point(444, 117);
            this.txtopCode.Name = "txtopCode";
            this.txtopCode.Size = new System.Drawing.Size(27, 19);
            this.txtopCode.TabIndex = 8;
            this.txtopCode.Text = "OP";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(492, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "JUMP";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(258, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 19);
            this.label6.TabIndex = 10;
            this.label6.Text = "Disassembly:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Consolas", 12F);
            this.textBox1.Location = new System.Drawing.Point(262, 175);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(392, 292);
            this.textBox1.TabIndex = 11;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(117, 14);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(48, 17);
            this.checkBox1.TabIndex = 12;
            this.checkBox1.Text = "Step";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Consolas", 12F);
            this.label7.Location = new System.Drawing.Point(339, 84);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 19);
            this.label7.TabIndex = 13;
            this.label7.Text = "Stack Top:";
            // 
            // stackTop
            // 
            this.stackTop.AutoSize = true;
            this.stackTop.Font = new System.Drawing.Font("Consolas", 12F);
            this.stackTop.Location = new System.Drawing.Point(444, 84);
            this.stackTop.Name = "stackTop";
            this.stackTop.Size = new System.Drawing.Size(36, 19);
            this.stackTop.TabIndex = 14;
            this.stackTop.Text = "(0)";
            // 
            // DebugWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(666, 484);
            this.Controls.Add(this.stackTop);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtopCode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.stkCnt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.NumIp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.varList);
            this.Controls.Add(this.label1);
            this.Name = "DebugWindow";
            this.Text = "DebugWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DebugWindow_FormClosing);
            this.Load += new System.EventHandler(this.DebugWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NumIp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox varList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown NumIp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label stkCnt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label txtopCode;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label stackTop;
    }
}