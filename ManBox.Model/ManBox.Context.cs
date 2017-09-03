﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ManBox.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ManBoxEntities : DbContext
    {
        public ManBoxEntities()
            : base("name=ManBoxEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Category> Categories { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SubscriptionDelivery> SubscriptionDeliveries { get; set; }
        public DbSet<SubscriptionDeliveryModel> SubscriptionDeliveryModels { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<TranslationText> TranslationTexts { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Pack> Packs { get; set; }
        public DbSet<ProductPack> ProductPacks { get; set; }
        public DbSet<SubscriptionDeliveryMessage> SubscriptionDeliveryMessages { get; set; }
        public DbSet<Gift> Gifts { get; set; }
    }
}