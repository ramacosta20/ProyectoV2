using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace PaginaProyecto.Models
{
    public class Administrador
    {
        [Key]
        public int AdminID { get; set; }


        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(60, ErrorMessage = "El nombre debe tener menos de 60 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "URL de Imagen")]
        [DataType(DataType.ImageUrl, ErrorMessage = "Debe ingresar la url de una imagen en el campo {0}")]
        public HttpPostedFileBase Imagen { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Direccion de Email invalida")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Contraseña")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "La contraseña debe tener como minimo 8 caracteres")]
        [DataType(DataType.Password, ErrorMessage = "Debe ingresar una contraseña")]
        public string Contraseña { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Contraseña", ErrorMessage = "La contraseña y la confirmacion no son iguales")]
        public string ConfirmarContraseña { get; set; }
    }
}