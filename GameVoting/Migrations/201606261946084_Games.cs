namespace GameVoting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Games : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Option", "OptionSetId", "dbo.OptionSet");
            DropIndex("dbo.Option", new[] { "OptionSetId" });
            CreateTable(
                "dbo.Game",
                c => new
                    {
                        GameId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        BggId = c.String(maxLength: 10),
                        MinimumPlayers = c.Int(),
                        MaximumPlayers = c.Int(),
                        Description = c.String(),
                        CreatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.GameId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy)
                .Index(t => t.CreatedBy);
            
            CreateTable(
                "dbo.GameSetGame",
                c => new
                    {
                        GameSetGameId = c.Int(nullable: false, identity: true),
                        GameSetId = c.Int(nullable: false),
                        GameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GameSetGameId)
                .ForeignKey("dbo.Game", t => t.GameId)
                .ForeignKey("dbo.GameSet", t => t.GameSetId)
                .Index(t => new { t.GameSetId, t.GameId }, unique: true, name: "IX_GameSetGame");
            
            CreateTable(
                "dbo.GameSet",
                c => new
                    {
                        GameSetId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.GameSetId);

            AddColumn("dbo.EventOption", "GameId", c => c.Int());

            // Migrate data
            Sql("INSERT INTO dbo.GameSet (Name) SELECT Name FROM dbo.OptionSet");
            Sql("INSERT INTO dbo.Game (Name) SELECT Name FROM [dbo].[Option]");
            Sql("INSERT INTO dbo.GameSetGame (GameSetId, GameId) SELECT gs.GameSetId, g.GameId FROM [dbo].[Option] o INNER JOIN dbo.OptionSet os ON o.OptionSetId = os.OptionSetId INNER JOIN dbo.Game g ON g.Name = o.Name INNER JOIN dbo.GameSet gs ON gs.Name = os.Name");

            Sql("INSERT INTO dbo.Game (Name) SELECT o.Name FROM dbo.EventOption o LEFT JOIN dbo.Game g ON g.Name = o.Name WHERE g.GameId IS NULL");
            Sql("UPDATE dbo.EventOption SET dbo.EventOption.GameId = dbo.Game.GameId FROM dbo.EventOption INNER JOIN dbo.Game ON dbo.Game.Name = dbo.EventOption.Name");
            
            AlterColumn("dbo.EventOption", "GameId", c => c.Int(nullable: false));            
            CreateIndex("dbo.EventOption", "GameId");
            AddForeignKey("dbo.EventOption", "GameId", "dbo.Game", "GameId");
            DropColumn("dbo.EventOption", "Name");
            DropTable("dbo.Option");
            DropTable("dbo.OptionSet");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OptionSet",
                c => new
                    {
                        OptionSetId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.OptionSetId);
            
            CreateTable(
                "dbo.Option",
                c => new
                    {
                        OptionId = c.Int(nullable: false, identity: true),
                        OptionSetId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.OptionId);

            AddColumn("dbo.EventOption", "Name", c => c.String(maxLength: 100));
            Sql("UPDATE dbo.EventOption SET dbo.EventOption.Name = dbo.Game.Name FROM dbo.EventOption INNER JOIN dbo.Game ON dbo.Game.GameId = dbo.EventOption.GameId");
            AddColumn("dbo.EventOption", "Name", c => c.String(nullable: false, maxLength: 100));

            DropForeignKey("dbo.EventOption", "GameId", "dbo.Game");
            DropForeignKey("dbo.GameSetGame", "GameSetId", "dbo.GameSet");
            DropForeignKey("dbo.GameSetGame", "GameId", "dbo.Game");
            DropForeignKey("dbo.Game", "CreatedBy", "dbo.UserProfile");
            DropIndex("dbo.GameSetGame", "IX_GameSetGame");
            DropIndex("dbo.Game", new[] { "CreatedBy" });
            DropIndex("dbo.EventOption", new[] { "GameId" });
            DropColumn("dbo.EventOption", "GameId");
            DropTable("dbo.GameSet");
            DropTable("dbo.GameSetGame");
            DropTable("dbo.Game");
            CreateIndex("dbo.Option", "OptionSetId");
            AddForeignKey("dbo.Option", "OptionSetId", "dbo.OptionSet", "OptionSetId");
        }
    }
}
