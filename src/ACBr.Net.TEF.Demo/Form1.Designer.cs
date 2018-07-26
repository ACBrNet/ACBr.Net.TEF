namespace ACBr.Net.TEF.Demo
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnInicializar = new System.Windows.Forms.Button();
            this.btnATV = new System.Windows.Forms.Button();
            this.btnADM = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(312, 314);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // btnInicializar
            // 
            this.btnInicializar.Location = new System.Drawing.Point(330, 12);
            this.btnInicializar.Name = "btnInicializar";
            this.btnInicializar.Size = new System.Drawing.Size(75, 23);
            this.btnInicializar.TabIndex = 1;
            this.btnInicializar.Text = "Inicializar";
            this.btnInicializar.UseVisualStyleBackColor = true;
            this.btnInicializar.Click += new System.EventHandler(this.btnInicializar_Click);
            // 
            // btnATV
            // 
            this.btnATV.Location = new System.Drawing.Point(411, 12);
            this.btnATV.Name = "btnATV";
            this.btnATV.Size = new System.Drawing.Size(75, 23);
            this.btnATV.TabIndex = 2;
            this.btnATV.Text = "ATV";
            this.btnATV.UseVisualStyleBackColor = true;
            this.btnATV.Click += new System.EventHandler(this.btnATV_Click);
            // 
            // btnADM
            // 
            this.btnADM.Location = new System.Drawing.Point(330, 41);
            this.btnADM.Name = "btnADM";
            this.btnADM.Size = new System.Drawing.Size(75, 23);
            this.btnADM.TabIndex = 3;
            this.btnADM.Text = "ADM";
            this.btnADM.UseVisualStyleBackColor = true;
            this.btnADM.Click += new System.EventHandler(this.btnADM_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(411, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "CRT";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 338);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnADM);
            this.Controls.Add(this.btnATV);
            this.Controls.Add(this.btnInicializar);
            this.Controls.Add(this.richTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Teste TEF";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnInicializar;
        private System.Windows.Forms.Button btnATV;
        private System.Windows.Forms.Button btnADM;
        private System.Windows.Forms.Button button1;
    }
}

