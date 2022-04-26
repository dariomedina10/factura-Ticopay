namespace TicopayUniversalConnectorService.Contexto
{
    using SQLite.CodeFirst;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using TicopayUniversalConnectorService.Entities;

    public class UniversalConnectorDB : DbContext
    {
        public DbSet<Configuracion> Configuraciones { get; set; }
        public DbSet<Operacion> Operaciones { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<UniversalConnectorDB>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);

            modelBuilder.Entity<Operacion>()
                .HasRequired(e => e.Configuracion)
                .WithMany(c => c.Operaciones)
                .WillCascadeOnDelete(true);            
        }
        // El contexto se ha configurado para usar una cadena de conexión 'UniversalConnectorDB' del archivo 
        // de configuración de la aplicación (App.config o Web.config). De forma predeterminada, 
        // esta cadena de conexión tiene como destino la base de datos 'TicopayUniversalConnectorService.Contexto.UniversalConnectorDB' de la instancia LocalDb. 
        // 
        // Si desea tener como destino una base de datos y/o un proveedor de base de datos diferente, 
        // modifique la cadena de conexión 'UniversalConnectorDB'  en el archivo de configuración de la aplicación.
        public UniversalConnectorDB()
            : base("name=UniversalConnectorDB")
        {
        }

        // Agregue un DbSet para cada tipo de entidad que desee incluir en el modelo. Para obtener más información 
        // sobre cómo configurar y usar un modelo Code First, vea http://go.microsoft.com/fwlink/?LinkId=390109.     
    }
}