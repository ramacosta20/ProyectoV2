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
        public int idEvento { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(60, ErrorMessage = "El nombre del evento debe tener menos de 60 caracteres")]
        [Display(Name = "Nombre del evento")]
        public string NombreEvento { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(60, ErrorMessage = "La descripcion debe tener menos de 400 caracteres")]
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Meta")]
        public int Meta { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Fecha en que termina la recaudacion (DD/MM/AAAA)")]
        public DateTime FechaTermina { get; set; }

        public Usuario AdminEvento { get; set; }

        [Display(Name = "Embed de mapa")]
        public string EmbedUbicacion { get; set; }

        [Display (Name = "Imagen")]
        public HttpPostedFileBase Imagen { get; set; }

        public int idsuarioAdmin { get; set; }

        //metodos de la clase

        //inserta un evento en la DB
        public void InsertarEvento()
        {
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
                consulta.Parameters.AddWithValue("PEmbedUbicacion", this.EmbedUbicacion);
                consulta.Parameters.AddWithValue("PImagen", this.Imagen.ToString());

                //ejecuto la consulta que no devuelve nada
                consulta.ExecuteNonQuery();
                tran.Commit();
            }
            catch (Exception ex)
            {
                //este bloque de codigo va a manejar cualquier error que pudiera 
                //ocurrir en el servidor que pudieran causar la falla del reintento,
                //como por ejemplo una conexion cerrada.
                Console.WriteLine("Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            Conexiondb.Close();
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
                MySqlCommand consulta = new MySqlCommand("TraerEvento", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;
                //Agrego los parametros
                consulta.Parameters.AddWithValue("PEmail", idEvento);

                //ejecuto la consulta y obtengo un iterable con registros
                MySqlDataReader dr = consulta.ExecuteReader();
                if (dr.Read())
                {
                    Evento oEvento = new Evento();
                    this.idEvento = Convert.ToInt32(dr["idEvento"]);
                    this.NombreEvento = dr["NombreEvento"].ToString();
                    this.Descripcion = dr["Descripcion"].ToString();
                    this.Meta = Convert.ToInt32(dr["Meta"]);
                    this.FechaTermina = Convert.ToDateTime(dr["FechaTermina"]);
                    this.EmbedUbicacion = dr["EmbedUbicacion"].ToString();
                    this.idsuarioAdmin = idUsuarioAdmin;
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
                if (dr.Read())
                {
                    Evento oEvento = new Evento();
                    oEvento.idEvento = Convert.ToInt32(dr["idEvento"]);
                    oEvento.NombreEvento = dr["NombreEvento"].ToString();
                    oEvento.Descripcion = dr["Descripcion"].ToString();
                    oEvento.Meta = Convert.ToInt32(dr["Meta"]);
                    oEvento.FechaTermina = Convert.ToDateTime(dr["FechaTermina"]);
                    oEvento.EmbedUbicacion = dr["EmbedUbicacion"].ToString();
                    //oEvento.Imagen = Path.GetFileName(dr["Imagen"].ToString());
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
    }
}