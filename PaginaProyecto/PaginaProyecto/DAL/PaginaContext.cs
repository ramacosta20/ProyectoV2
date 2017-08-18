using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using PaginaProyecto.Models;

namespace PaginaProyecto.DAL
{  
      public class PaginaContext : DbContext
      {
        public PaginaContext() : base("PaginaContext")
        {
            Database.SetInitializer<PaginaContext>(new DropCreateDatabaseIfModelChanges<PaginaContext>());
        }
        public DbSet<Usuario> Usuarios { get; set; }
      }    
}