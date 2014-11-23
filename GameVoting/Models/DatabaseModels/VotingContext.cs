using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace GameVoting.Models.DatabaseModels
{
    public class VotingContext : DbContext
    {
        public VotingContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<EventOption> EventOption { get; set; }
        public DbSet<EventMember> EventMember { get; set; }
        public DbSet<EventType> EventType { get; set; }
        public DbSet<EventVote> EventVote { get; set; }
        public DbSet<OptionSet> OptionSet { get; set; }
        public DbSet<Option> Option { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Stop EF from making foreign keys cascade delete
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class VotingContextInitializer 
        : DropCreateDatabaseIfModelChanges<VotingContext>
    {
        protected override void Seed(VotingContext context)
        {
            ////Seed users and roles
            //WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName",
            //                                         autoCreateTables: true);
            //if (!Roles.RoleExists("Admin"))
            //{
            //    Roles.CreateRole("Admin");
            //}
            //if (!WebSecurity.UserExists("admin"))
            //{
            //    WebSecurity.CreateUserAndAccount("admin", "admin");
            //}
            //if (!Roles.GetRolesForUser("admin").Contains("Admin"))
            //{
            //    Roles.AddUserToRole("admin", "Admin");
            //}

            //Add Event Types
            context.EventType.Add(new EventType() {Name = "Favorite", Description = "Pick your favorite option from those available"});
            context.EventType.Add(new EventType() { Name = "Ok", Description = "Mark all options that you are ok with." });
            context.EventType.Add(new EventType() { Name = "Ok-Rank", Description = "Mark all options that you are ok with, and for each, provide a ranking." });
            context.EventType.Add(new EventType() { Name = "Rank", Description = "Provide a unique rank for all options." });

            base.Seed(context);
        }
    }
}