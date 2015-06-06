using GameVoting.Models.DatabaseModels;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace GameVoting.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<GameVoting.Models.DatabaseModels.VotingContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "GameVoting.Models.DatabaseModels.VotingContext";
        }

        protected override void Seed(GameVoting.Models.DatabaseModels.VotingContext context)
        {
            //  This method will be called after migrating to the latest version.

            // Add Event Types
            context.EventType.AddOrUpdate(e => e.Name,
                new EventType() { Name = "Favorite", Description = "Pick your favorite option from those available. Results are a simple sum of scores.", MinScore = 0, MaxScore = 1 },
                new EventType() { Name = "Ok", Description = "Mark all options that you are ok with. Results combine a weighting and sum of scores. Marking '0' for an option reduces its overall weight.", MinScore = 0, MaxScore = 1 },
                new EventType() { Name = "Ok-Rank", Description = "Mark all options that you are ok with, and for each, provide a ranking. Results combine a weighting and sum of scores. Marking '0' for an option reduces its overall weight.", MinScore = 0, MaxScore = 3 },
                new EventType() { Name = "Rank", Description = "Provide a unique rank for all options. Results are a simple sum of scores.", MinScore = 1, MaxScore = null });

            // Option Sets
            var lunchGames = new OptionSet() { Name = "Lunch Games" };
            context.OptionSet.AddOrUpdate(o => o.Name,
                lunchGames);

            context.Option.AddOrUpdate(o => o.Name,
                new Option() { OptionSetId = lunchGames.OptionSetId, Name = "One Night Ultimate Werewolf" },
                new Option() { OptionSetId = lunchGames.OptionSetId, Name = "Resistance: Avalon" },
                new Option() { OptionSetId = lunchGames.OptionSetId, Name = "Coup" },
                new Option() { OptionSetId = lunchGames.OptionSetId, Name = "Saboteur" },
                new Option() { OptionSetId = lunchGames.OptionSetId, Name = "Mascarade" },
                new Option() { OptionSetId = lunchGames.OptionSetId, Name = "Lifeboat" },
                new Option() { OptionSetId = lunchGames.OptionSetId, Name = "Skulls and Roses" },
                new Option() { OptionSetId = lunchGames.OptionSetId, Name = "Ultimate Werewolf" },
                new Option() { OptionSetId = lunchGames.OptionSetId, Name = "No Game" });
            
            // Seven Wonders
            context.WondersBoards.AddOrUpdate(b => b.Name,
                new WondersBoards() { Name = "Giza (A)" },
                new WondersBoards() { Name = "Giza (B)" },
                new WondersBoards() { Name = "Babylon (A)" },
                new WondersBoards() { Name = "Babylon (B)" },
                new WondersBoards() { Name = "Ephesus (A)" },
                new WondersBoards() { Name = "Ephesus (B)" },
                new WondersBoards() { Name = "Olympia (A)" },
                new WondersBoards() { Name = "Olympia (B)" },
                new WondersBoards() { Name = "Halicarnassus (A)" },
                new WondersBoards() { Name = "Halicarnassus (B)" },
                new WondersBoards() { Name = "Rhodes (A)" },
                new WondersBoards() { Name = "Rhodes (B)" },
                new WondersBoards() { Name = "Alexandria (A)" },
                new WondersBoards() { Name = "Alexandria (B)" });
            
        }
    }
}
