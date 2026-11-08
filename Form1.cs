using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace LaRuletaDeLaSuerte
{
    public partial class Form1 : Form
    {
        // Elemento de la lista de conectados: marca al propio usuario
        private class ItemUsuario
        {
            public string Nombre;
            public bool Yo;
            public override string ToString() { return Yo ? "★ " + Nombre + " (tú)" : Nombre; }
        }

        private readonly ConsultasRuleta _consultas = new ConsultasRuleta();
        private string _jugadorActual = null;
        private string _usuarioPendiente = null;
        private Thread _hiloServidor;
        private volatile bool _cerrando = false;
        private FormPartida _formPartida = null;

        public Form1()
        {
            InitializeComponent();

            txtResultados.ReadOnly = true;
            btnJugadores.Click  += btnJugadores_Click;
            btnResultados.Click += btnResultados_Click;
            btnPeriodo.Click    += btnPeriodo_Click;
            button1.Click       += button1_Click;
            btnJugar.Click      += btnJugar_Click;

            SetConsultasHabilitadas(false);

            this.FormClosing += (s, ev) =>
            {
                _cerrando = true;
                _consultas.Desconectar();   // el servidor nos quita de la lista y de la mesa
            };
        }

        private void SetConsultasHabilitadas(bool on)
        {
            btnJugadores.Enabled = on;
            btnResultados.Enabled = on;
            btnPeriodo.Enabled = on;
            btnJugar.Enabled = on;
        }

        // ============================================================
        // CONEXIÓN + HILO DEDICADO A LOS MENSAJES DEL SERVIDOR
        // ============================================================
        private bool AsegurarConexion()
        {
            if (_consultas.Conectado) return true;
            if (!_consultas.Conectar())
            {
                MessageBox.Show("No se pudo conectar con el servidor.", "Error de conexión",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            _hiloServidor = new Thread(AtenderServidor);
            _hiloServidor.IsBackground = true;
            _hiloServidor.Start();
            return true;
        }

        // Este hilo SOLO lee del socket y pasa cada mensaje al hilo de la interfaz.
        private void AtenderServidor()
        {
            string msg;
            while ((msg = _consultas.LeerMensaje()) != null)
            {
                string m = msg;
                try { BeginInvoke(new Action(() => ProcesarMensaje(m))); }
                catch (InvalidOperationException) { return; }
            }
            if (!_cerrando)
            {
                try { BeginInvoke(new Action(ConexionPerdida)); }
                catch (InvalidOperationException) { }
            }
        }

        private void ConexionPerdida()
        {
            _consultas.Desconectar();
            _jugadorActual = null;
            _usuarioPendiente = null;
            if (_formPartida != null) { FormPartida f = _formPartida; _formPartida = null; f.Close(); }
            SetConsultasHabilitadas(false);
            lstConectados.Items.Clear();
            MessageBox.Show("Se perdió la conexión con el servidor.", "Desconectado",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool Enviar(bool ok)
        {
            if (!ok)
                MessageBox.Show("No se pudo enviar la petición al servidor.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return ok;
        }

        // ============================================================
        // DISTRIBUIDOR DE MENSAJES (hilo de la UI)
        // ============================================================
        private void ProcesarMensaje(string msg)
        {
            int i = msg.IndexOf('/');
            int codigo;
            if (i < 0 || !int.TryParse(msg.Substring(0, i), out codigo)) return;
            string payload = msg.Substring(i + 1);

            switch (codigo)
            {
                case 1:  RespuestaRegistro(payload); break;
                case 2:  RespuestaLogin(payload); break;
                case 3:  txtResultados.Text = ConsultasRuleta.FormatearJugadores(payload); break;
                case 4:  txtResultados.AppendText(ConsultasRuleta.FormatearResultados(payload) + Environment.NewLine); break;
                case 5:  txtResultados.Text = ConsultasRuleta.FormatearPeriodo(payload); break;
                case 7:  MostrarHistorial(payload); break;
                case 8:  RespuestaUnirse(payload); break;
                case 10: ActualizarConectados(payload); break;   // push
                case 11: AnadirLineaChat(payload); break;        // push
                case 14:
                case 18: if (_formPartida != null) _formPartida.Respuesta(codigo, payload); break;
                case 22: if (_formPartida != null) _formPartida.ActualizarEstado(payload); break;  // push
                case 25: if (_formPartida != null) _formPartida.FinRonda(payload); break;          // push
                case 26: if (_formPartida != null) _formPartida.AnadirLog(payload); break;         // push
            }
        }

        private void RespuestaRegistro(string resp)
        {
            if (resp == "OK")
                MessageBox.Show("Usuario creado. Ya puedes iniciar sesión.", "Registro correcto",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (resp == "YA EXISTE")
                MessageBox.Show("Ese nombre de usuario ya está en uso.", "Usuario existente",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (resp == "NOMBRE INVALIDO")
                MessageBox.Show("El nombre solo puede tener letras, números y '_' (máx. 20).", "Nombre no válido",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Error al registrar: " + resp, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void RespuestaLogin(string resp)
        {
            if (resp == "OK")
            {
                _jugadorActual = _usuarioPendiente;
                SetConsultasHabilitadas(true);
                _consultas.PedirHistorial();
                MessageBox.Show("¡Bienvenido, " + _jugadorActual + "!", "Login correcto",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (resp == "YA CONECTADO")
                MessageBox.Show("Ese usuario ya tiene una sesión abierta.", "Error de login",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error de login",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            _usuarioPendiente = null;
        }

        // ============================================================
        // LOGIN / REGISTRO
        // ============================================================
        private void Login_Click(object sender, EventArgs e)
        {
            string usuario = UserBox.Text.Trim();
            string password = PasswordBox.Text;
            if (usuario == "" || password == "")
            {
                MessageBox.Show("Rellena usuario y contraseña", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_jugadorActual != null)
            {
                MessageBox.Show("Ya has iniciado sesión.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!AsegurarConexion()) return;
            _usuarioPendiente = usuario;
            Enviar(_consultas.Login(usuario, password));
        }

        private void Create_Acount_Click(object sender, EventArgs e)
        {
            string usuario = UserBox.Text.Trim();
            string password = PasswordBox.Text;
            if (usuario == "" || password == "")
            {
                MessageBox.Show("Rellena usuario y contraseña", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!AsegurarConexion()) return;
            Enviar(_consultas.Registro(usuario, password));
        }

        // ============================================================
        // CONSULTAS
        // ============================================================
        private void btnJugadores_Click(object sender, EventArgs e)
        {
            if (_jugadorActual == null) { txtResultados.Text = "Inicia sesión primero."; return; }
            Enviar(_consultas.PedirJugadores());
        }

        private void btnResultados_Click(object sender, EventArgs e)
        {
            if (_jugadorActual == null) { txtResultados.Text = "Inicia sesión primero."; return; }
            var limpios = new List<string>();
            foreach (string n in txtJugadores.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (n.Trim() != "") limpios.Add(n.Trim());
            if (limpios.Count == 0)
            {
                txtResultados.Text = "Escribe el nombre de uno o más jugadores (separados por comas).";
                return;
            }
            txtResultados.Clear();
            foreach (string rival in limpios)
                Enviar(_consultas.PedirResultados(rival));
        }

        private void btnPeriodo_Click(object sender, EventArgs e)
        {
            if (_jugadorActual == null) { txtResultados.Text = "Inicia sesión primero."; return; }
            if (dtpDesde.Value.Date > dtpHasta.Value.Date)
            {
                txtResultados.Text = "La fecha inicial no puede ser posterior a la final.";
                return;
            }
            Enviar(_consultas.PedirPeriodo(dtpDesde.Value.Date, dtpHasta.Value.Date));
        }

        // ============================================================
        // LISTA DE CONECTADOS (push del servidor) — marca al propio usuario
        // ============================================================
        private void ActualizarConectados(string payload)
        {
            ItemUsuario sel = lstConectados.SelectedItem as ItemUsuario;
            string seleccionado = sel != null ? sel.Nombre : null;

            lstConectados.BeginUpdate();
            lstConectados.Items.Clear();
            foreach (string nombre in payload.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int idx = lstConectados.Items.Add(new ItemUsuario
                    { Nombre = nombre, Yo = nombre == _jugadorActual });
                if (nombre == seleccionado) lstConectados.SelectedIndex = idx;
            }
            lstConectados.EndUpdate();
        }

        // ============================================================
        // MESA DE JUEGO
        // ============================================================
        private void btnJugar_Click(object sender, EventArgs e)
        {
            if (_jugadorActual == null)
            {
                MessageBox.Show("Inicia sesión antes de jugar.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_formPartida != null) { _formPartida.Focus(); return; }

            _formPartida = new FormPartida(_consultas, _jugadorActual);
            _formPartida.FormClosed += (s, ev) =>
            {
                _formPartida = null;
                if (_consultas.Conectado) _consultas.SalirMesa();   // inofensivo si no estaba sentado
            };
            _formPartida.Show(this);
            Enviar(_consultas.Unirse());
        }

        // 8/ : respuesta a mi petición de sentarme
        private void RespuestaUnirse(string resp)
        {
            if (resp == "OK") return;
            string m = resp == "LLENA" ? "La mesa está llena (máximo 3 jugadores)."
                     : resp == "EN_PARTIDA" ? "Hay una ronda en curso. Espera a que termine."
                     : "No se pudo unir a la mesa (" + resp + ").";
            MessageBox.Show(m, "Mesa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (_formPartida != null) _formPartida.Close();
        }

        // ============================================================
        // CHAT
        // ============================================================
        private void button1_Click(object sender, EventArgs e)
        {
            if (_jugadorActual == null)
            {
                MessageBox.Show("Inicia sesión antes de chatear.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string texto = textBox3.Text.Trim();
            if (texto == "") return;
            if (texto.Contains("/"))
            {
                MessageBox.Show("El mensaje no puede contener '/'.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Enviar(_consultas.EnviarMensaje(texto));
            textBox3.Clear();
            textBox3.Focus();
        }

        private void MostrarHistorial(string payload)
        {
            richTextBox1.Clear();
            if (payload == "VACIO" || payload == "ERROR") return;
            foreach (string l in payload.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
                AnadirLineaChat(l);
        }

        private void AnadirLineaChat(string linea)
        {
            string[] p = linea.Split(new[] { ':' }, 2);
            string texto = p.Length == 2 ? p[0] + ": " + p[1] : linea;
            richTextBox1.AppendText(texto + Environment.NewLine);
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    }
}