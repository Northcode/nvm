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
            this.varList.FormattingEnabled = true;
            this.varList.Location = new System.Drawing.Point(12, 73);
            this.varList.Name = "varList";
            this.varList.Size = new System.Drawing.Size(187, 394);
            this.varList.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 51);
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
            this.NumIp.Name = "NumIp";
            this.NumIp.Size = new System.Drawing.Size(125, 20);
            this.NumIp.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 12F);
            this.label4.Location = new System.Drawing.Point(240, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 19);
            this.label4.TabIndex = 5;
            this.label4.Text = "Stack Count:";
            // 
            // stkCnt
            // 
            this.stkCnt.AutoSize = true;
            this.stkCnt.Font = new System.Drawing.Font("Consolas", 12F);
            this.stkCnt.Location = new System.Drawing.Point(363, 50);
            this.stkCnt.Name = "stkCnt";
            this.stkCnt.Size = new System.Drawing.Size(36, 19);
            this.stkCnt.TabIndex = 6;
            this.stkCnt.Text = "(0)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Consolas", 12F);
            this.label5.Location = new System.Drawing.Point(213, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 19);
            this.label5.TabIndex = 7;
            this.label5.Text = "Current opcode:";
            // 
            // txtopCode
            // 
            this.txtopCode.AutoSize = true;
            this.txtopCode.Font = new System.Drawing.Font("Consolas", 12F);
            this.txtopCode.Location = new System.Drawing.Point(363, 82);
            this.txtopCode.Name = "txtopCode";
            this.txtopCode.Size = new System.Drawing.Size(27, 19);
            this.txtopCode.TabIndex = 8;
            this.txtopCode.Text = "OP";
            // 
            // DebugWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(498, 484);
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
    }
}