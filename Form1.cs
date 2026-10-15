using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LaRuletaDeLaSuerte
{
    public partial class Form1 : Form
    {
        private readonly ConsultasRuleta _consultas = new ConsultasRuleta();

        // Nombre del jugador logueado. Se rellena tras un login correcto.
        private string _jugadorActual = null;

        // Timer para refrescar el chat periódicamente
        private Timer chatTimer;

        public Form1()
        {
            InitializeComponent();

            // --- Login / Registro (los eventos ya están enganchados en el Designer) ---

            // --- Consultas ---
            txtResultados.ReadOnly = true;
            btnJugadores.Click  += btnJugadores_Click;
            btnResultados.Click += btnResultados_Click;
            btnPeriodo.Click    += btnPeriodo_Click;

            // Desactiva consultas hasta que haya login
            btnJugadores.Enabled  = false;
            btnResultados.Enabled = false;
            btnPeriodo.Enabled    = false;

            // --- Chat ---
            button1.Click += button1_Click;

            chatTimer = new Timer();
            chatTimer.Interval = 2000;                 // cada 2 segundos
            chatTimer.Tick += (s, ev) => RefrescarChat();
            chatTimer.Start();

            // Parar el timer al cerrar el formulario
            this.FormClosing += (s, ev) =>
            {
                if (chatTimer != null) chatTimer.Stop();
            };
        }

        // ============================================================
        // LOGIN  -> 2/usuario/password
        // ============================================================
        private void Login_Click(object sender, EventArgs e)
        {
            string usuario  = UserBox.Text.Trim();
            string password = PasswordBox.Text;

            if (usuario == "" || password == "")
            {
                MessageBox.Show("Rellena usuario y contraseña", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string resp = _consultas.Login(usuario, password);

            if (resp == "OK")
            {
                _jugadorActual = usuario;
                MessageBox.Show("¡Bienvenido, " + usuario + "!",
                                "Login correcto",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnJugadores.Enabled  = true;
                btnResultados.Enabled = true;
                btnPeriodo.Enabled    = true;
            }
            else if (resp == "SOCKET_ERROR")
            {
                MessageBox.Show("No se pudo conectar con el servidor.",
                                "Error de conexión",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.",
                                "Error de login",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // REGISTRO  -> 1/usuario/password
        // ============================================================
        private void Create_Acount_Click(object sender, EventArgs e)
        {
            string usuario  = UserBox.Text.Trim();
            string password = PasswordBox.Text;

            if (usuario == "" || password == "")
            {
                MessageBox.Show("Rellena usuario y contraseña", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string resp = _consultas.Registro(usuario, password);

            if (resp == "OK")
            {
                MessageBox.Show("Usuario creado. Ya puedes iniciar sesión.",
                                "Registro correcto",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (resp == "YA EXISTE")
            {
                MessageBox.Show("Ese nombre de usuario ya está en uso.",
                                "Usuario existente",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (resp == "SOCKET_ERROR")
            {
                MessageBox.Show("No se pudo conectar con el servidor.",
                                "Error de conexión",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Error al registrar: " + resp,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // BOTÓN 1: jugadores con los que he jugado
        // ============================================================
        private void btnJugadores_Click(object sender, EventArgs e)
        {
            if (_jugadorActual == null)
            {
                txtResultados.Text = "Inicia sesión primero.";
                return;
            }
            txtResultados.Text =
                _consultas.JugadoresConLosQueHeJugado(_jugadorActual);
        }

        // ============================================================
        // BOTÓN 2: resultados contra uno o varios jugadores
        // ============================================================
        private void btnResultados_Click(object sender, EventArgs e)
        {
            if (_jugadorActual == null)
            {
                txtResultados.Text = "Inicia sesión primero.";
                return;
            }

            string[] nombres = txtJugadores.Text.Split(
                new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var limpios = new List<string>();
            foreach (string n in nombres)
                if (n.Trim() != "") limpios.Add(n.Trim());

            if (limpios.Count == 0)
            {
                txtResultados.Text =
                    "Escribe el nombre de uno o más jugadores (separados por comas).";
                return;
            }

            txtResultados.Text =
                _consultas.ResultadosConJugadores(_jugadorActual, limpios.ToArray());
        }

        // ============================================================
        // BOTÓN 3: partidas en un periodo
        // ============================================================
        private void btnPeriodo_Click(object sender, EventArgs e)
        {
            if (_jugadorActual == null)
            {
                txtResultados.Text = "Inicia sesión primero.";
                return;
            }
            if (dtpDesde.Value.Date > dtpHasta.Value.Date)
            {
                txtResultados.Text =
                    "La fecha inicial no puede ser posterior a la final.";
                return;
            }

            txtResultados.Text = _consultas.PartidasEnPeriodo(
                _jugadorActual, dtpDesde.Value.Date, dtpHasta.Value.Date);
        }

        // ============================================================
        // CHAT — enviar mensaje
        // ============================================================
        private void button1_Click(object sender, EventArgs e)
        {
            if (_jugadorActual == null)
            {
                MessageBox.Show("Inicia sesión antes de chatear.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string texto = textBox3.Text.Trim();
            if (texto == "") return;

            if (texto.Contains("/"))
            {
                MessageBox.Show("El mensaje no puede contener '/'.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _consultas.EnviarMensaje(_jugadorActual, texto);
            textBox3.Clear();
            textBox3.Focus();
            RefrescarChat();     // refresco inmediato tras enviar
        }

        // ============================================================
        // CHAT — pedir todos los mensajes y pintarlos
        // ============================================================
        private void RefrescarChat()
        {
            string resp = _consultas.ObtenerMensajes();

            if (resp == "SOCKET_ERROR" || resp == "ERROR" || resp == "VACIO")
                return;   // nada que mostrar o error momentáneo

            string[] lineas = resp.Split(new[] { '/' },
                                         StringSplitOptions.RemoveEmptyEntries);

            // "usuario:mensaje" → "usuario: mensaje"
            string nuevo = string.Join(Environment.NewLine,
                System.Array.ConvertAll(lineas, l => l.Replace(":", ": ")));

            if (richTextBox1.Text != nuevo + Environment.NewLine)
            {
                richTextBox1.Text = nuevo + Environment.NewLine;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
        }
    }
}