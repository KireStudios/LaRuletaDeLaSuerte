using System;
using System.Windows.Forms;

namespace LaRuletaDeLaSuerte
{
    public partial class Form1 : Form
    {
        private readonly ConsultasRuleta _consultas = new ConsultasRuleta();

        // Nombre del jugador que ha iniciado sesión (antes: id fijo).
        // Se rellena al hacer login correctamente.
        private string _jugadorActual = null;

        public Form1()
        {
            InitializeComponent();

            // --- Login / Registro (nombres tal y como los usa el Designer) ---
            Login.Click          += Login_Click;
            Create_Acount.Click  += Create_Acount_Click;

            // --- Consultas (solo activas tras login) ---
            txtResultados.ReadOnly = true;
            btnJugadores.Click  += btnJugadores_Click;
            btnResultados.Click += btnResultados_Click;
            btnPeriodo.Click    += btnPeriodo_Click;

            // Desactiva los botones de consulta hasta que haya login
            btnJugadores.Enabled  = false;
            btnResultados.Enabled = false;
            btnPeriodo.Enabled    = false;
        }

        // ============================================================
        // LOGIN  -> petición 2/usuario/password
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

                // Activa las consultas
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
        // REGISTRO -> petición 1/usuario/password
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

            var limpios = new System.Collections.Generic.List<string>();
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
    }
}