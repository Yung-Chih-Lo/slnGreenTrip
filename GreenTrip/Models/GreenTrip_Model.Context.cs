//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace GreenTrip.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GreenTripEntities2 : DbContext
    {
        public GreenTripEntities2()
            : base("name=GreenTripEntities2")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<County> County { get; set; }
        public virtual DbSet<Feedback> Feedback { get; set; }
        public virtual DbSet<Journey> Journey { get; set; }
        public virtual DbSet<JourneySite> JourneySite { get; set; }
        public virtual DbSet<Owner> Owner { get; set; }
        public virtual DbSet<Site> Site { get; set; }
        public virtual DbSet<SitePerference> SitePerference { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserPerference> UserPerference { get; set; }
    }
}
