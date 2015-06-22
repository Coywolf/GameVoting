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
            : base("GameVoting")
        {
        }

        public DbSet<UserProfile> UserProfile { get; set; }

        // Events
        public DbSet<Event> Event { get; set; }
        public DbSet<EventOption> EventOption { get; set; }
        public DbSet<EventMember> EventMember { get; set; }
        public DbSet<EventType> EventType { get; set; }
        public DbSet<EventVote> EventVote { get; set; }
        public DbSet<OptionSet> OptionSet { get; set; }
        public DbSet<Option> Option { get; set; }

        // SevenWonders
        public DbSet<WondersGame> WondersGame { get; set; }
        public DbSet<WondersPlayer> WondersPlayer { get; set; }
        public DbSet<WondersBoards> WondersBoards { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Stop EF from making foreign keys cascade delete
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}