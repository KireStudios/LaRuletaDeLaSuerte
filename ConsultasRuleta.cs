using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LaRuletaDeLaSuerte
{
    public class ConsultasRuleta
    {
        private const string SERVER_IP   = "192.168.56.101";
        private const int    SERVER_PORT = 9090;

        private Socket _socket;
        private readonly object _lockEnvio = new object();
        private readonly byte[] _buf = new byte[8192];
        private readonly StringBuilder _pendiente = new StringBuilder();
        private readonly Decoder _decoder = Encoding.UTF8.GetDecoder();

        public bool Conectado { get { return _socket != null; } }

        public bool Conectar()
        {
            if (Conectado) return true;
            try
            {
                var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(new IPEndPoint(IPAddress.Parse(SERVER_IP), SERVER_PORT));
                _pendiente.Clear();
                _socket = s;
                return true;
            }
            catch (SocketException) { return false; }
        }

        public bool Enviar(string peticion)
        {
            try
            {
                lock (_lockEnvio)
                {
                    _socket.Send(Encoding.UTF8.GetBytes(peticion + "\n"));
                }
                return true;
            }
            catch { return false; }
        }

        /// Bloqueante. Devuelve una línea completa del servidor, o null si se cerró la conexión.
        /// Solo la llama el hilo de lectura.
        public string LeerMensaje()
        {
            try
            {
                while (true)
                {
                    string acum = _pendiente.ToString();
                    int idx = acum.IndexOf('\n');
                    if (idx >= 0)
                    {
                        _pendiente.Remove(0, idx + 1);
                        return acum.Substring(0, idx).TrimEnd('\r');
                    }
                    int n = _socket.Receive(_buf);
                    if (n <= 0) return null;
                    char[] chars = new char[n];
                    int c = _decoder.GetChars(_buf, 0, n, chars, 0);
                    _pendiente.Append(chars, 0, c);
                }
            }
            catch { return null; }
        }

        public void Desconectar()
        {
            Socket s = _socket;
            _socket = null;
            if (s == null) return;
            try { s.Send(Encoding.UTF8.GetBytes("0/\n")); } catch { }
            try { s.Shutdown(SocketShutdown.Both); } catch { }   // desbloquea Receive del hilo
            try { s.Close(); } catch { }
        }

        // ---------------- Peticiones (solo envían; la respuesta llega por el hilo) ----------------
        public bool Login(string u, string p)    { return Enviar("2/" + u + "/" + p); }
        public bool Registro(string u, string p) { return Enviar("1/" + u + "/" + p); }
        public bool PedirJugadores()             { return Enviar("3/"); }
        public bool PedirResultados(string rival){ return Enviar("4/" + rival); }
        public bool PedirPeriodo(DateTime d, DateTime h)
        {
            return Enviar("5/" + d.ToString("yyyy-MM-dd") + "/" + h.ToString("yyyy-MM-dd"));
        }
        public bool EnviarMensaje(string texto)  { return Enviar("6/" + texto); }
        public bool PedirHistorial()             { return Enviar("7/"); }
        public bool Unirse()          { return Enviar("8/"); }
        public bool IniciarPartida()  { return Enviar("14/"); }
        public bool SalirMesa()       { return Enviar("15/"); }
        public bool Girar()           { return Enviar("18/"); }

        // ---------------- Formateo de respuestas ----------------
        public static string FormatearJugadores(string resp)
        {
            if (resp == "ERROR") return "Error al consultar la base de datos.";
            if (resp == "VACIO") return "Todavía no has jugado con otros jugadores.";
            string[] nombres = resp.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return "Has jugado con:" + Environment.NewLine +
                   "  - " + string.Join(Environment.NewLine + "  - ", nombres);
        }

        /// payload = "rival|datos"
        public static string FormatearResultados(string payload)
        {
            int i = payload.IndexOf('|');
            string rival = i >= 0 ? payload.Substring(0, i) : "?";
            string resp  = i >= 0 ? payload.Substring(i + 1) : payload;

            if (resp == "ERROR") return "Error al consultar partidas con " + rival + ".";
            if (resp == "VACIO") return "No hay partidas con " + rival + ".";

            var sb = new StringBuilder();
            sb.AppendLine("Partidas con " + rival + ":");
            foreach (string p in resp.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] c = p.Split(':');
                if (c.Length < 4) continue;
                sb.AppendLine("   Partida " + c[0] + " (" + c[1] + ")  ganador: " + c[2] +
                              "  mis puntos: " + c[3]);
            }
            return sb.ToString();
        }

        public static string FormatearPeriodo(string resp)
        {
            if (resp == "ERROR") return "Error al consultar la base de datos.";
            if (resp == "VACIO") return "No hay partidas en ese periodo.";
            var sb = new StringBuilder();
            sb.AppendLine("Partidas en el periodo:");
            sb.AppendLine();
            foreach (string p in resp.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] c = p.Split(':');
                if (c.Length < 3) continue;
                sb.AppendLine("   Partida " + c[0] + " (" + c[1] + ")  ganador: " + c[2]);
            }
            return sb.ToString();
        }
    }
}