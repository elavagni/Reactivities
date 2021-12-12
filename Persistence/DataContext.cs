using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : DbContext
    {
        public DbSet<Activity>? Activities { set; get; }

        public DataContext(DbContextOptions options) : base(options)
        {
        }
    }
}