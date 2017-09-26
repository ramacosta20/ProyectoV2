using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data.OleDb;

namespace PaginaProyecto.Models
{
    public class Usuario
    {
        //declaro conexion

        private MySqlConnection Conexiondb = new MySqlConnection("server=localhost; Uid=root; Password=Proyecto; Database=mydb; Port=3306");
        

        //propiedades de la clase
        public int UsuarioID { get; set; }

        public bool EmailValido { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(60, ErrorMessage = "El nombre debe tener menos de 60 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(60, ErrorMessage = "El apellido debe tener menos de 60 caracteres")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Direccion de Email invalida")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Contraseña")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "La contraseña debe tener un minimo de 8 caracteres")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Contraseña", ErrorMessage = "La contraseña y la confirmacion no son iguales")]
        public string ConfirmarContraseña { get; set; }

        public HttpPostedFileBase Imagen { get; set; }

        public string ImagenString { get; set; }

        //metodos publicos

        //Agrega el usuario que ejecuta el metodo a la base de datos
        public void InsertarUsuario()
        {
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            try
            {
                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DB
                MySqlCommand consulta = new MySqlCommand("InsertarUsuario",Conexiondb,tran);            
                consulta.CommandType = CommandType.StoredProcedure;
                //Agrego los parametros
                consulta.Parameters.AddWithValue("PNombre", this.Nombre);
                consulta.Parameters.AddWithValue("PEmail", this.Email);
                consulta.Parameters.AddWithValue("PApellido",this.Apellido);
                consulta.Parameters.AddWithValue("PContraseña",this.Contraseña);
                consulta.Parameters.AddWithValue("PImagen", this.ImagenString);

                //ejecuto la consulta que no devuelve nada
                consulta.ExecuteNonQuery();
                tran.Commit();

                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DB
                MySqlCommand consultaId = new MySqlCommand("ObtenerIdUsuario", Conexiondb, tran);
                consultaId.CommandType = CommandType.StoredProcedure;

                //agrego los parametros
                consultaId.Parameters.AddWithValue("PEEmail", this.Email);

                //ejecuto la consulta que no devuelve nada
                MySqlDataReader dr =consultaId.ExecuteReader();
                while (dr.Read())
                {
                    this.UsuarioID = Convert.ToInt32(dr["idUsuario"].ToString());
                }
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

        //devuelve un Usuario si el mail y la contraseña(parametros) coinciden con un mail y una contraseña de un registro en la DB
        public void LoguearUsuario()
        {
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            try
            {
                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DBConsulta.CommandType = CommandType.StoredProcedure;
                MySqlCommand consulta = new MySqlCommand("LoguearUsuario", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;
                //Agrego los parametros
                consulta.Parameters.AddWithValue("PEmail", this.Email);
                consulta.Parameters.AddWithValue("PContraseña", this.Contraseña);

                //ejecuto la consulta y obtengo un iterable con registros
                MySqlDataReader dr = consulta.ExecuteReader();
                if (dr.Read())
                {
                    this.Nombre = dr["Nombre"].ToString();
                    this.Apellido = dr["Apellido"].ToString();
                    this.ImagenString = dr["Imagen"].ToString();
                    this.UsuarioID = Convert.ToInt32(dr["idUsuario"].ToString());
                }
            }
            catch (Exception ex2)
            {
                {
                    //este bloque de codigo va a manejar cualquier error que pudieran 
                    //ocurrir en el servidor que pudieran causar la falla del reintento,
                    //como por ejemplo una conexion cerrada.
                    Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("  Message: {0}", ex2.Message);
                }
            }
            Conexiondb.Close();
        }

        //devuelve true si el mail ya existe en algun registro en la DB y devuelve false de lo contrario
        public void ExisteMail()
        {
            //abro conexion y declaro una transaccion
            Conexiondb.Open();
            MySqlTransaction tran = Conexiondb.BeginTransaction();
            try
            {
                // asigno el nombre de la consulta a el nombre de consulta que tengo guardado en la DBConsulta.CommandType = CommandType.StoredProcedure;
                MySqlCommand consulta = new MySqlCommand("ExisteMail", Conexiondb, tran);
                consulta.CommandType = CommandType.StoredProcedure;
                //Agrego los parametros
                consulta.Parameters.AddWithValue("PEmail", Email);

                //ejecuto la consulta y obtengo un iterable con registros
                MySqlDataReader dr = consulta.ExecuteReader();
                if (dr.HasRows == false)
                {
                    this.EmailValido = true;
                }
            }
            catch (Exception ex2)
            {
                    //este bloque de codigo va a manejar cualquier error que pudieran 
                    //ocurrir en el servidor que pudieran causar la falla del reintento,
                    //como por ejemplo una conexion cerrada.
                    Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("  Message: {0}", ex2.Message);
            }
            Conexiondb.Close();
        }
    }
}