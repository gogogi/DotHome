using DotHome.Core.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Services
{
    public class NotificationsStoreContext : DbContext
    {
        public DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }

        public NotificationsStoreContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
