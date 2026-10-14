using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaRuletaDeLaSuerte
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, EventArgs e)
        {
            string usuario = UserBox.Text.Trim();
            string password = PasswordBox.Text;

            if (usuario == "" || password == "")
            {
                MessageBox.Show("Rellena usuario y contraseña", "Aviso",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Ruta al ejecutable de C (AJUSTA ESTA RUTA)
            // Si es Windows nativo: "C:\\ruta\\a\\login.exe"
            // Si está dentro de la VM Linux: usar Plink/SSH (ver más abajo)
            string rutaLogin = @"C:\ruta\a\login.exe";

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = rutaLogin;
            psi.Arguments = $"\"{usuario}\" \"{password}\"";
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            try
            {
                Process p = Process.Start(psi);
                string salida = p.StandardOutput.ReadToEnd().Trim();
                p.WaitForExit();

                if (salida == "OK")
                {
                    MessageBox.Show("¡Bienvenido, " + usuario + "!",
                                    "Login correcto",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                    // Aquí abrirías el formulario del juego:
                    // FormJuego f = new FormJuego(usuario);
                    // f.Show();
                    // this.Hide();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos",
                                    "Error de login",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void Create_Acount_Click(object sender, EventArgs e)
        {
            string usuario = UserBox.Text.Trim();
            string password = PasswordBox.Text;

            if (usuario == "" || password == "")
            {
                MessageBox.Show("Rellena usuario y contraseña",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string rutaLogin = @"C:\ruta\a\login.exe";

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = rutaLogin;
            psi.Arguments = $"registro \"{usuario}\" \"{password}\""; 
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            try
            {
                Process p = Process.Start(psi);
                string salida = p.StandardOutput.ReadToEnd().Trim();
                p.WaitForExit();

                if (salida == "OK")
                    MessageBox.Show("Usuario creado. Ya puedes iniciar sesión.",
                                    "Registro correcto",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                else if (salida == "YA_EXISTE")
                    MessageBox.Show("Ese nombre de usuario ya está en uso.",
                                    "Usuario existente",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Error al registrar: " + salida,
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
