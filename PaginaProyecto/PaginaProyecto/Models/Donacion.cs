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
    public class Donacion
    {
        //declaro conexion
        private MySqlConnection Conexiondb = new MySqlConnection("server=localhost; Uid=root; Password=Proyecto; Database=mydb; Port=3306");
        //propiedades de la clase

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Numero de tarjeta")]
        public int NumeroTarjeta { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Fecha de Expiracion")]
        public DateTime Expiracion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "CV")]
        public int CV { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(0, 99999999, ErrorMessage ="Ingrese un valor de 0 a 99999999")]
        [Display(Name = "Monto")]
        public int Monto { get; set; }

        //metodos de la clase
        public void donar(int idEvento, int idUsuario)
        {
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            try
            {
                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DB
                MySqlCommand consulta = new MySqlCommand("Donar", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;

                //Agrego los parametros
                consulta.Parameters.AddWithValue("PIdEvento", idEvento);
                consulta.Parameters.AddWithValue("PIdUsuario", idUsuario);
                consulta.Parameters.AddWithValue("PMonto", this.Monto);

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
        public int recaudadoEvento(int idEvento)
        {
            int recaudado = 0;
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            try
            {
                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DBConsulta.CommandType = CommandType.StoredProcedure;
                MySqlCommand consulta = new MySqlCommand("RecaudadoEvento", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;

                //Agrego los parametros
                consulta.Parameters.AddWithValue("PIdEvento", idEvento);

                //ejecuto la consulta y obtengo un iterable con registros
                MySqlDataReader dr = consulta.ExecuteReader();
                if (dr.Read())
                {
                    recaudado = Convert.ToInt32(dr["monto"]);
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
            return recaudado;
        }
    }
}