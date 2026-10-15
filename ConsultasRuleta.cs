using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LaRuletaDeLaSuerte
{
    /// <summary>
    /// Cliente TCP que envía peticiones al servidor y devuelve la respuesta.
    /// Todas las consultas viajan al servidor mediante sockets.
    /// </summary>
    public class ConsultasRuleta
    {
        // ---- Configuración del servidor ----
        private const string SERVER_IP   = "192.168.56.101"; // IP de tu VM Linux
        private const int    SERVER_PORT = 9090;
        private const int    BUFFER_SIZE = 8192;

        // ============================================================
        // Petición genérica: envía texto, recibe texto.
        // Único punto de comunicación con el servidor.
        // ============================================================
        private string EnviarPeticion(string peticion)
        {
            try
            {
                IPAddress direc = IPAddress.Parse(SERVER_IP);
                IPEndPoint ipep = new IPEndPoint(direc, SERVER_PORT);

                using (Socket server = new Socket(AddressFamily.InterNetwork,
                                                  SocketType.Stream,
                                                  ProtocolType.Tcp))
                {
                    server.Connect(ipep);
                    server.Send(Encoding.ASCII.GetBytes(peticion));

                    // La respuesta puede llegar en varios paquetes:
                    // leemos hasta que el servidor cierre la conexión.
                    var sb = new StringBuilder();
                    byte[] msg = new byte[BUFFER_SIZE];
                    int recibidos;
                    while ((recibidos = server.Receive(msg)) > 0)
                        sb.Append(Encoding.ASCII.GetString(msg, 0, recibidos));

                    return sb.ToString().Split('\0')[0].Trim();
                }
            }
            catch (SocketException)
            {
                return "SOCKET_ERROR";
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.Message;
            }
        }

        // ============================================================
        // LOGIN / REGISTRO (códigos 1 y 2)
        // ============================================================
        public string Login(string usuario, string password)
            => EnviarPeticion("2/" + usuario + "/" + password);

        public string Registro(string usuario, string password)
            => EnviarPeticion("1/" + usuario + "/" + password);

        // ============================================================
        // CONSULTA 1: jugadores con los que he jugado (código 3)
        // ============================================================
        public string JugadoresConLosQueHeJugado(string nombreUsuario)
        {
            string resp = EnviarPeticion("3/" + nombreUsuario);

            if (resp == "SOCKET_ERROR") return "No se pudo conectar con el servidor.";
            if (resp == "ERROR")         return "Error al consultar la base de datos.";
            if (resp == "VACIO")         return "Todavía no has jugado con otros jugadores.";

            string[] nombres = resp.Split(new[] { '/' },
                                          StringSplitOptions.RemoveEmptyEntries);
            if (nombres.Length == 0)
                return "Todavía no has jugado con otros jugadores.";

            return "Has jugado con:" + Environment.NewLine +
                   "  - " + string.Join(Environment.NewLine + "  - ", nombres);
        }

        // ============================================================
        // CONSULTA 2: resultados contra uno o varios jugadores (código 4)
        // ============================================================
        public string ResultadosConJugadores(string nombreUsuario, string[] rivales)
        {
            var sb = new StringBuilder();

            foreach (string rival in rivales)
            {
                string resp = EnviarPeticion("4/" + nombreUsuario + "/" + rival);

                if (resp == "SOCKET_ERROR")
                {
                    sb.AppendLine("No se pudo conectar con el servidor.");
                    continue;
                }
                if (resp == "ERROR")
                {
                    sb.AppendLine("Error al consultar partidas con " + rival + ".");
                    continue;
                }
                if (resp == "VACIO")
                {
                    sb.AppendLine("No hay partidas con " + rival + ".");
                    continue;
                }

                sb.AppendLine("Partidas con " + rival + ":");

                // Formato de cada partida: id:fecha:ganador:puntosUsuario/
                string[] partidas = resp.Split(new[] { '/' },
                                               StringSplitOptions.RemoveEmptyEntries);
                foreach (string p in partidas)
                {
                    string[] campos = p.Split(':');
                    if (campos.Length < 4) continue;
                    sb.AppendLine("   Partida " + campos[0] +
                                  " (" + campos[1] + ")  ganador: " + campos[2] +
                                  "  mis puntos: " + campos[3]);
                }
                sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }

        // ============================================================
        // CONSULTA 3: partidas en un periodo (código 5)
        // ============================================================
        public string PartidasEnPeriodo(string nombreUsuario,
                                        DateTime desde, DateTime hasta)
        {
            string peticion = "5/" + desde.ToString("yyyy-MM-dd") +
                              "/" + hasta.ToString("yyyy-MM-dd");

            string resp = EnviarPeticion(peticion);

            if (resp == "SOCKET_ERROR") return "No se pudo conectar con el servidor.";
            if (resp == "ERROR")         return "Error al consultar la base de datos.";
            if (resp == "VACIO")         return "No hay partidas en ese periodo.";

            string[] partidas = resp.Split(new[] { '/' },
                                           StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();
            sb.AppendLine("Partidas entre " +
                          desde.ToString("dd/MM/yyyy") + " y " +
                          hasta.ToString("dd/MM/yyyy") + ":");
            sb.AppendLine();
            foreach (string p in partidas)
            {
                string[] campos = p.Split(':');
                if (campos.Length < 3) continue;
                sb.AppendLine("   Partida " + campos[0] +
                              " (" + campos[1] + ")  ganador: " + campos[2]);
            }
            return sb.ToString();
        }

        // ============================================================
        // CHAT — código 6 (enviar) y código 7 (recibir)
        // ============================================================
        public string EnviarMensaje(string usuario, string mensaje)
            => EnviarPeticion("6/" + usuario + "/" + mensaje);

        public string ObtenerMensajes()
            => EnviarPeticion("7/x");
    }
}