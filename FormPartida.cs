using System;
using System.Drawing;
using System.Windows.Forms;

namespace LaRuletaDeLaSuerte
{
    /// Ventana de la mesa: jugadores, puntos, turno y botones Iniciar / Tirar ruleta.
    public partial class FormPartida : Form
    {
        private readonly ConsultasRuleta _consultas;
        private readonly string _yo;
        private Panel[] _paneles;
        private Label[] _nombres;
        private TextBox[] _puntos;

        /// Constructor sin parámetros: solo para el diseñador de Visual Studio.
        public FormPartida()
        {
            InitializeComponent();
        }

        public FormPartida(ConsultasRuleta consultas, string yo) : this()
        {
            _consultas = consultas;
            _yo = yo;
            Text = "La Ruleta de la Suerte - Mesa (" + yo + ")";

            _paneles = new[] { panelJ1, panelJ2, panelJ3 };
            _nombres = new[] { lblNombre1, lblNombre2, lblNombre3 };
            _puntos  = new[] { txtPuntos1, txtPuntos2, txtPuntos3 };
            Vaciar();
        }

        // ------------------------------------------------------------
        // Botones
        // ------------------------------------------------------------
        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if (_consultas != null) _consultas.IniciarPartida();
        }

        private void btnGirar_Click(object sender, EventArgs e)
        {
            if (_consultas != null) _consultas.Girar();
        }

        // ------------------------------------------------------------
        // Mensajes del servidor (los llama Form1 desde el hilo de la UI)
        // ------------------------------------------------------------

        /// Deja la mesa en blanco (sin jugadores).
        private void Vaciar()
        {
            for (int i = 0; i < 3; i++)
            {
                _nombres[i].Text = "Jugador " + (i + 1);
                _puntos[i].Text = "";
                _paneles[i].BackColor = SystemColors.Control;
            }
            lblTurno.Text = "Esperando jugadores...";
            btnIniciar.Enabled = false;
            btnGirar.Enabled = false;
        }

        /// 22/nombres|puntos|turno|en_juego   (vacío = ya no hay mesa)
        public void ActualizarEstado(string payload)
        {
            if (payload == "") { Vaciar(); return; }

            string[] p = payload.Split('|');
            if (p.Length < 4) return;

            string[] nombres = p[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] puntos  = p[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string turno = p[2];
            bool enJuego = p[3] == "1";

            for (int i = 0; i < 3; i++)
            {
                bool hay = i < nombres.Length;
                _nombres[i].Text = hay ? nombres[i] + (nombres[i] == _yo ? " (tú)" : "")
                                       : "Jugador " + (i + 1);
                _puntos[i].Text = (hay && i < puntos.Length) ? puntos[i] : "";
                _paneles[i].BackColor = (hay && enJuego && nombres[i] == turno)
                                        ? Color.LightGreen : SystemColors.Control;
            }

            if (!enJuego)
                lblTurno.Text = nombres.Length < 2 ? "Esperando jugadores (mín. 2, máx. 3)"
                                                   : "Todo listo: pulsa «Iniciar ronda»";
            else
                lblTurno.Text = turno == _yo ? "¡Es tu turno! Tira la ruleta" : "Turno de " + turno;

            btnIniciar.Enabled = !enJuego && nombres.Length >= 2;
            btnGirar.Enabled = enJuego && turno == _yo;
        }

        /// 26/texto : línea del registro
        public void AnadirLog(string texto)
        {
            rtbLog.AppendText(texto + Environment.NewLine);
            rtbLog.SelectionStart = rtbLog.Text.Length;
            rtbLog.ScrollToCaret();
        }

        /// 25/ganador|puntos
        public void FinRonda(string payload)
        {
            string[] p = payload.Split('|');
            string ganador = p[0];
            string puntos = p.Length > 1 ? p[1] : "?";
            string m = ganador == _yo ? "¡Has ganado la ronda con " + puntos + " puntos!"
                     : ganador.Contains(" y ") ? "Empate entre " + ganador + " con " + puntos + " puntos."
                     : ganador + " gana la ronda con " + puntos + " puntos.";
            AnadirLog("★ " + m);
            MessageBox.Show(this, m, "Fin de la ronda", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// Respuestas del servidor a mis acciones (14 = iniciar, 18 = girar)
        public void Respuesta(int codigo, string resp)
        {
            switch (resp)
            {
                case "OK": return;
                case "NOTURNO":     AnadirLog("» No es tu turno."); break;
                case "POCOS":       AnadirLog("» Hacen falta al menos 2 jugadores."); break;
                case "YAENPARTIDA": AnadirLog("» La ronda ya ha empezado."); break;
                default:            AnadirLog("» Error: " + resp); break;
            }
        }
    }
}