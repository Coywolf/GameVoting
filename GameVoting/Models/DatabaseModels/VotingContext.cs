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
        : CreateDatabaseIfNotExists<VotingContext>
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
            context.EventType.Add(new EventType() { Name = "Favorite", Description = "Pick your favorite option from those available. Results are a simple sum of scores.", MinScore = 0, MaxScore = 1 });
            context.EventType.Add(new EventType() { Name = "Ok", Description = "Mark all options that you are ok with. Results combine a weighting and sum of scores. Marking '0' for an option reduces its overall weight.", MinScore = 0, MaxScore = 1 });
            context.EventType.Add(new EventType() { Name = "Ok-Rank", Description = "Mark all options that you are ok with, and for each, provide a ranking. Results combine a weighting and sum of scores. Marking '0' for an option reduces its overall weight.", MinScore = 0, MaxScore = 3 });
            context.EventType.Add(new EventType() { Name = "Rank", Description = "Provide a unique rank for all options. Results are a simple sum of scores.", MinScore = 1, MaxScore = null });

            //Option Sets
            var lunchGames = new OptionSet() { Name = "Lunch Games" };
            context.OptionSet.Add(lunchGames);
            context.Option.Add(new Option() { OptionSet = lunchGames, Name = "One Night Ultimate Werewolf" });
            context.Option.Add(new Option() { OptionSet = lunchGames, Name = "Resistance: Avalon" });
            context.Option.Add(new Option() { OptionSet = lunchGames, Name = "Coup" });
            context.Option.Add(new Option() { OptionSet = lunchGames, Name = "Saboteur" });
            context.Option.Add(new Option() { OptionSet = lunchGames, Name = "Mascarade" });
            context.Option.Add(new Option() { OptionSet = lunchGames, Name = "Lifeboat" });
            context.Option.Add(new Option() { OptionSet = lunchGames, Name = "Skulls and Roses" });
            context.Option.Add(new Option() { OptionSet = lunchGames, Name = "Ultimate Werewolf" });
            context.Option.Add(new Option() { OptionSet = lunchGames, Name = "No Game" });

            base.Seed(context);
        }
    }
}