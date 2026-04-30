using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<InvItemMaster> InvItemMaster { get; set; }
        //public DbSet<ERPSample.Models.Inventory.UnitMaster> UnitMaster { get; set; }
        public DbSet<MaMisc> MaMisc { get; set; }
        public DbSet<InvItemUnits> InvItemUnits { get; set; }
        public DbSet<MaMiscKeys> MaMiscKeys { get; set; }
        public DbSet<FiMaVouchers> FiMaVouchers { get; set; }
        public DbSet<Parties> Parties { get; set; }
        public DbSet<Locations> Locations { get; set; }
        public DbSet<InvTransItems> InvTransItems { get; set; }
        public DbSet<FiTransactions> FiTransactions { get; set; }
        public DbSet<MaTaxType> MaTaxType { get; set; }
        public DbSet<FiTransactionEntries> FiTransactionEntries { get; set; }
        public DbSet<FiMaSubGroup> FiMaSubGroup { get; set; }
        public DbSet<FiMaAccountCategory> FiMaAccountCategory { get; set; }
        //public DbSet<ERPSample.Models.Inventory.Masters.InvItemImages> InvItemImages { get; set; }
        //public DbSet<ERPSample.Models.Ecommerce.Masters.EcomBannerMaster> EcomBannerMaster { get; set; }
        //public DbSet<ERPSample.Models.Ecommerce.Masters.EcomPanelMaster> EcomPanelMaster { get; set; }
    }
}
