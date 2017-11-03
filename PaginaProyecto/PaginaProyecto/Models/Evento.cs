using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data.OleDb;
using System.IO;

namespace PaginaProyecto.Models
{
    public class Evento
    {
        //declaro conexion

        private MySqlConnection Conexiondb = new MySqlConnection("server=localhost; Uid=root; Password=Proyecto; Database=mydb; Port=3306");

        //propiedades de la clase
        public int EventoID { get; set; }

        public int UsuarioID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(60, ErrorMessage = "El nombre del evento debe tener menos de 60 caracteres")]
        [Display(Name = "Nombre del evento")]
        public string NombreEvento { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(400, ErrorMessage = "La descripcion debe tener menos de 400 caracteres")]
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(0, 99999999999)]
        [Display(Name = "Meta")]
        public int Meta { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Fecha en que termina la recaudacion (DD/MM/AAAA)")]
        public DateTime FechaTermina { get; set; }

        [Display(Name = "Embed de mapa")]
        public string EmbedUbicacion { get; set; }

        [Display(Name = "Imagen")]
        public HttpPostedFileBase Imagen { get; set; }

        public string ImagenString { get; set; }

        public Usuario UsuarioAdmin { get; set; }

        public bool EnFecha { get; set; }

        public int Recaudado { get; set; }

        public int CantDonantesEvento { get; set; }

        //metodos de la clase
        public Evento() {
            UsuarioAdmin = new Usuario();
        }
        //inserta un evento en la DB
        public void InsertarEvento()
        {
            MySqlConnection Conexiondb2 = new MySqlConnection("server=localhost; Uid=root; Password=Proyecto; Database=mydb; Port=3306");
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            try
            {
                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DB
                MySqlCommand consulta = new MySqlCommand("InsertarEvento", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;
                //Agrego los parametros
                consulta.Parameters.AddWithValue("PNombreEvento", this.NombreEvento);
                consulta.Parameters.AddWithValue("PDescripcion", this.Descripcion);
                consulta.Parameters.AddWithValue("PMeta", this.Meta);
                consulta.Parameters.AddWithValue("PFechaTermina", this.FechaTermina);
                consulta.Parameters.AddWithValue("PImagen", this.ImagenString);
                consulta.Parameters.AddWithValue("PUsuarioAdmin", this.UsuarioID);

                //ejecuto la consulta que no devuelve nada
                consulta.ExecuteNonQuery();
                tran.Commit();
                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DB

                MySqlTransaction tran2 = Conexiondb.BeginTransaction();
                Conexiondb2.Open();
                MySqlCommand consultaid = new MySqlCommand("ObtenerIdEvento", Conexiondb2, tran2);
                consultaid.CommandType = CommandType.StoredProcedure;

                //Agrego los parametros
                consultaid.Parameters.AddWithValue("PENombreEvento", this.NombreEvento);

                //ejecuto la consulta que no devuelve nada
                MySqlDataReader dr = consultaid.ExecuteReader();
                while (dr.Read())
                {
                    this.EventoID = Convert.ToInt32(dr["idEvento"].ToString());
                }
                tran2.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                //este bloque de codigo va a manejar cualquier error que pudiera 
                //ocurrir en el servidor que pudieran causar la falla del reintento,
                //como por ejemplo una conexion cerrada.
                Console.WriteLine("Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            Conexiondb.Close();
            Conexiondb2.Close();
        }

        //devuelve una lista de eventos que el usuario(id pasado como parametro) administra
        public List<Evento> ListarEventosUsuario(int idUsuarioAdmin)
        {
           
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            List<Evento> listaEventos = new List<Evento>();
            try
            {

                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DBConsulta.CommandType = CommandType.StoredProcedure;
                MySqlCommand consulta = new MySqlCommand("ListarEventosUsuario", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;
                //Agrego los parametros
                consulta.Parameters.AddWithValue("PIdUsuarioAdmin", idUsuarioAdmin);

                //ejecuto la consulta y obtengo un iterable con registros
                MySqlDataReader dr = consulta.ExecuteReader();
                while (dr.Read())
                {
                    Evento oEvento = new Evento();
                    oEvento.EventoID = Convert.ToInt32(dr["idEvento"]);
                    oEvento.NombreEvento = dr["NombreEvento"].ToString();
                    oEvento.Descripcion = dr["Descripcion"].ToString();
                    oEvento.Meta = Convert.ToInt32(dr["Meta"]);
                    oEvento.FechaTermina = Convert.ToDateTime(dr["FechaTermina"]);
                    oEvento.ImagenString = dr["Imagen"].ToString();
                    oEvento.UsuarioAdmin.UsuarioID = idUsuarioAdmin;
                    oEvento.UsuarioAdmin.TraerUsuario();
                    oEvento.EstaEnFecha();
                    listaEventos.Add(oEvento);
                }
            }
            catch (Exception ex)
            {
                {
                    //este bloque de codigo va a manejar cualquier error que pudieran 
                    //ocurrir en el servidor que pudieran causar la falla del reintento,
                    //como por ejemplo una conexion cerrada.
                    Console.WriteLine("Rollback Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
            }
            Conexiondb.Close();
            return listaEventos;
        }

        //devuelve la lista de eventos completa que esta en la db
        public List<Evento> ListarEventos()
        {
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            List<Evento> listaEventos = new List<Evento>();
            try
            {

                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DBConsulta.CommandType = CommandType.StoredProcedure;
                MySqlCommand consulta = new MySqlCommand("TraerEventosTodos", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;

                //ejecuto la consulta y obtengo un iterable con registros
                MySqlDataReader dr = consulta.ExecuteReader();
                while (dr.Read())
                {
                    Evento oEvento = new Evento();
                    oEvento.EventoID = Convert.ToInt32(dr["idEvento"]);
                    oEvento.NombreEvento = dr["NombreEvento"].ToString();
                    oEvento.Descripcion = dr["Descripcion"].ToString();
                    oEvento.Meta = Convert.ToInt32(dr["Meta"]);
                    oEvento.FechaTermina = Convert.ToDateTime(dr["FechaTermina"]);
                    oEvento.ImagenString = dr["Imagen"].ToString();
                    oEvento.EstaEnFecha();
                    listaEventos.Add(oEvento);
                }
            }
            catch (Exception ex)
            {
                {
                    //este bloque de codigo va a manejar cualquier error que pudieran 
                    //ocurrir en el servidor que pudieran causar la falla del reintento,
                    //como por ejemplo una conexion cerrada.
                    Console.WriteLine("Rollback Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
            }
            Conexiondb.Close();
            return listaEventos;
        }

        public bool ExisteEvento()
        {
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            bool retorno = false;
            try
            {
                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DBConsulta.CommandType = CommandType.StoredProcedure;
                MySqlCommand consulta = new MySqlCommand("ExisteEvento", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;

                //Agrego los parametros
                consulta.Parameters.AddWithValue("PNombreEvento", this.NombreEvento);

                //ejecuto la consulta y obtengo un iterable con registros
                MySqlDataReader dr = consulta.ExecuteReader();
                if (dr.Read())
                {
                    retorno = true;
                }
            }
            catch (Exception ex)
            {
                {
                    //este bloque de codigo va a manejar cualquier error que pudieran 
                    //ocurrir en el servidor que pudieran causar la falla del reintento,
                    //como por ejemplo una conexion cerrada.
                    Console.WriteLine("Rollback Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
            }
            Conexiondb.Close();
            return retorno;
        }

        public void TraerEvento()
        {
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            try
            {

                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DBConsulta.CommandType = CommandType.StoredProcedure;
                MySqlCommand consulta = new MySqlCommand("TraerEvento", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;

                //Agrego los parametros
                consulta.Parameters.AddWithValue("PIdEvento", this.EventoID);

                //ejecuto la consulta y obtengo un iterable con registros
                MySqlDataReader dr = consulta.ExecuteReader();
                if (dr.Read())
                {
                    this.NombreEvento = dr["NombreEvento"].ToString();
                    this.Meta = Convert.ToInt32(dr["Meta"]);
                    this.ImagenString = dr["Imagen"].ToString();
                    this.Descripcion = dr["Descripcion"].ToString();
                    this.FechaTermina = Convert.ToDateTime(dr["FechaTermina"]);
                    this.Recaudado = Convert.ToInt32(dr["Recaudado"]);
                    this.UsuarioAdmin.UsuarioID = Convert.ToInt32(dr["UsuarioAdmin"]);
                }
            }
            catch (Exception ex)
            {
                {
                    //este bloque de codigo va a manejar cualquier error que pudieran 
                    //ocurrir en el servidor que pudieran causar la falla del reintento,
                    //como por ejemplo una conexion cerrada.
                    Console.WriteLine("Rollback Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
            }
            Conexiondb.Close();
        }

        public void EstaEnFecha()
        {
            if (FechaTermina > DateTime.Now)
            {
                EnFecha = true;
            }
            else
            {
                EnFecha = false;
            }
        }

        public void ModificarEvento()
        {
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            try
            {

                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DB
                MySqlCommand consulta = new MySqlCommand("ModificarEvento", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;

                //Agrego los parametros
                consulta.Parameters.AddWithValue("PIdEvento", this.EventoID);
                consulta.Parameters.AddWithValue("PImagen", this.ImagenString);
                consulta.Parameters.AddWithValue("PDescripcion", this.Descripcion);

                //ejecuto la consulta y obtengo un iterable con registros
                consulta.ExecuteNonQuery();
                tran.Commit();
            }
            catch (Exception ex)
            {
                {
                    //este bloque de codigo va a manejar cualquier error que pudieran 
                    //ocurrir en el servidor que pudieran causar la falla del reintento,
                    //como por ejemplo una conexion cerrada.
                    Console.WriteLine("Rollback Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
            }
            Conexiondb.Close();
        }

        public void recaudadoEvento()
        {
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            try
            {

                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DBConsulta.CommandType = CommandType.StoredProcedure;
                MySqlCommand consulta = new MySqlCommand("RecaudadoEvento", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;

                //Agrego los parametros
                consulta.Parameters.AddWithValue("PIdEvento", this.EventoID);

                //ejecuto la consulta y obtengo un iterable con registros
                MySqlDataReader dr = consulta.ExecuteReader();
                if (dr.Read())
                {
                    this.Recaudado = Convert.ToInt32(dr["Recaudado"]);
                }
            }
            catch (Exception ex)
            {
                {
                    //este bloque de codigo va a manejar cualquier error que pudieran 
                    //ocurrir en el servidor que pudieran causar la falla del reintento,
                    //como por ejemplo una conexion cerrada.
                    Console.WriteLine("Rollback Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
            }
            Conexiondb.Close();
        }

        public void CantidadDeDonantesEvento()
        {
            int contadorDonantes = 0;
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            try
            {

                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DBConsulta.CommandType = CommandType.StoredProcedure;
                MySqlCommand consulta = new MySqlCommand("CantidadDonantesEvento", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;

                //Agrego los parametros
                consulta.Parameters.AddWithValue("PIdEvento", this.EventoID);

                //ejecuto la consulta y obtengo un iterable con registros
                MySqlDataReader dr = consulta.ExecuteReader();
                while (dr.Read())
                {
                    contadorDonantes++;
                }
                this.CantDonantesEvento = contadorDonantes;
            }
            catch (Exception ex)
            {
                {
                    //este bloque de codigo va a manejar cualquier error que pudieran 
                    //ocurrir en el servidor que pudieran causar la falla del reintento,
                    //como por ejemplo una conexion cerrada.
                    Console.WriteLine("Rollback Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
            }
            Conexiondb.Close();
        }

    }
}