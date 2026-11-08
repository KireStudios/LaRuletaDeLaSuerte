namespace LaRuletaDeLaSuerte
{
    partial class FormPartida
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.lblTurno = new System.Windows.Forms.Label();
            this.panelJ1 = new System.Windows.Forms.Panel();
            this.txtPuntos1 = new System.Windows.Forms.TextBox();
            this.lblNombre1 = new System.Windows.Forms.Label();
            this.panelJ2 = new System.Windows.Forms.Panel();
            this.txtPuntos2 = new System.Windows.Forms.TextBox();
            this.lblNombre2 = new System.Windows.Forms.Label();
            this.panelJ3 = new System.Windows.Forms.Panel();
            this.txtPuntos3 = new System.Windows.Forms.TextBox();
            this.lblNombre3 = new System.Windows.Forms.Label();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.btnGirar = new System.Windows.Forms.Button();
            this.lblLog = new System.Windows.Forms.Label();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.panelJ1.SuspendLayout();
            this.panelJ2.SuspendLayout();
            this.panelJ3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTurno
            // 
            this.lblTurno.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTurno.Location = new System.Drawing.Point(20, 15);
            this.lblTurno.Name = "lblTurno";
            this.lblTurno.Size = new System.Drawing.Size(680, 36);
            this.lblTurno.TabIndex = 0;
            this.lblTurno.Text = "Esperando jugadores...";
            this.lblTurno.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelJ1
            // 
            this.panelJ1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelJ1.Controls.Add(this.txtPuntos1);
            this.panelJ1.Controls.Add(this.lblNombre1);
            this.panelJ1.Location = new System.Drawing.Point(20, 65);
            this.panelJ1.Name = "panelJ1";
            this.panelJ1.Size = new System.Drawing.Size(210, 130);
            this.panelJ1.TabIndex = 1;
            // 
            // txtPuntos1
            // 
            this.txtPuntos1.BackColor = System.Drawing.Color.White;
            this.txtPuntos1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold);
            this.txtPuntos1.Location = new System.Drawing.Point(10, 60);
            this.txtPuntos1.Name = "txtPuntos1";
            this.txtPuntos1.ReadOnly = true;
            this.txtPuntos1.Size = new System.Drawing.Size(188, 44);
            this.txtPuntos1.TabIndex = 1;
            this.txtPuntos1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblNombre1
            // 
            this.lblNombre1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblNombre1.Location = new System.Drawing.Point(10, 12);
            this.lblNombre1.Name = "lblNombre1";
            this.lblNombre1.Size = new System.Drawing.Size(188, 30);
            this.lblNombre1.TabIndex = 0;
            this.lblNombre1.Text = "Jugador 1";
            this.lblNombre1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelJ2
            // 
            this.panelJ2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelJ2.Controls.Add(this.txtPuntos2);
            this.panelJ2.Controls.Add(this.lblNombre2);
            this.panelJ2.Location = new System.Drawing.Point(255, 65);
            this.panelJ2.Name = "panelJ2";
            this.panelJ2.Size = new System.Drawing.Size(210, 130);
            this.panelJ2.TabIndex = 2;
            // 
            // txtPuntos2
            // 
            this.txtPuntos2.BackColor = System.Drawing.Color.White;
            this.txtPuntos2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold);
            this.txtPuntos2.Location = new System.Drawing.Point(10, 60);
            this.txtPuntos2.Name = "txtPuntos2";
            this.txtPuntos2.ReadOnly = true;
            this.txtPuntos2.Size = new System.Drawing.Size(188, 44);
            this.txtPuntos2.TabIndex = 1;
            this.txtPuntos2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblNombre2
            // 
            this.lblNombre2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblNombre2.Location = new System.Drawing.Point(10, 12);
            this.lblNombre2.Name = "lblNombre2";
            this.lblNombre2.Size = new System.Drawing.Size(188, 30);
            this.lblNombre2.TabIndex = 0;
            this.lblNombre2.Text = "Jugador 2";
            this.lblNombre2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelJ3
            // 
            this.panelJ3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelJ3.Controls.Add(this.txtPuntos3);
            this.panelJ3.Controls.Add(this.lblNombre3);
            this.panelJ3.Location = new System.Drawing.Point(490, 65);
            this.panelJ3.Name = "panelJ3";
            this.panelJ3.Size = new System.Drawing.Size(210, 130);
            this.panelJ3.TabIndex = 3;
            // 
            // txtPuntos3
            // 
            this.txtPuntos3.BackColor = System.Drawing.Color.White;
            this.txtPuntos3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold);
            this.txtPuntos3.Location = new System.Drawing.Point(10, 60);
            this.txtPuntos3.Name = "txtPuntos3";
            this.txtPuntos3.ReadOnly = true;
            this.txtPuntos3.Size = new System.Drawing.Size(188, 44);
            this.txtPuntos3.TabIndex = 1;
            this.txtPuntos3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblNombre3
            // 
            this.lblNombre3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblNombre3.Location = new System.Drawing.Point(10, 12);
            this.lblNombre3.Name = "lblNombre3";
            this.lblNombre3.Size = new System.Drawing.Size(188, 30);
            this.lblNombre3.TabIndex = 0;
            this.lblNombre3.Text = "Jugador 3";
            this.lblNombre3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnIniciar
            // 
            this.btnIniciar.Enabled = false;
            this.btnIniciar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnIniciar.Location = new System.Drawing.Point(20, 215);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(330, 55);
            this.btnIniciar.TabIndex = 4;
            this.btnIniciar.Text = "Iniciar ronda";
            this.btnIniciar.UseVisualStyleBackColor = true;
            this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);
            // 
            // btnGirar
            // 
            this.btnGirar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnGirar.Enabled = false;
            this.btnGirar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.btnGirar.Location = new System.Drawing.Point(370, 215);
            this.btnGirar.Name = "btnGirar";
            this.btnGirar.Size = new System.Drawing.Size(330, 55);
            this.btnGirar.TabIndex = 5;
            this.btnGirar.Text = "Tirar Ruleta";
            this.btnGirar.UseVisualStyleBackColor = false;
            this.btnGirar.Click += new System.EventHandler(this.btnGirar_Click);
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblLog.Location = new System.Drawing.Point(20, 287);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(169, 20);
            this.lblLog.TabIndex = 6;
            this.lblLog.Text = "Registro de la partida";
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.White;
            this.rtbLog.Location = new System.Drawing.Point(20, 314);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(680, 186);
            this.rtbLog.TabIndex = 7;
            this.rtbLog.Text = "";
            // 
            // FormPartida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 520);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.btnGirar);
            this.Controls.Add(this.btnIniciar);
            this.Controls.Add(this.panelJ3);
            this.Controls.Add(this.panelJ2);
            this.Controls.Add(this.panelJ1);
            this.Controls.Add(this.lblTurno);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPartida";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "La Ruleta de la Suerte - Mesa";
            this.panelJ1.ResumeLayout(false);
            this.panelJ1.PerformLayout();
            this.panelJ2.ResumeLayout(false);
            this.panelJ2.PerformLayout();
            this.panelJ3.ResumeLayout(false);
            this.panelJ3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTurno;
        private System.Windows.Forms.Panel panelJ1;
        private System.Windows.Forms.TextBox txtPuntos1;
        private System.Windows.Forms.Label lblNombre1;
        private System.Windows.Forms.Panel panelJ2;
        private System.Windows.Forms.TextBox txtPuntos2;
        private System.Windows.Forms.Label lblNombre2;
        private System.Windows.Forms.Panel panelJ3;
        private System.Windows.Forms.TextBox txtPuntos3;
        private System.Windows.Forms.Label lblNombre3;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.Button btnGirar;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.RichTextBox rtbLog;
    }
}