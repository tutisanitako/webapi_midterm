namespace Wordle_hw.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        GameId = c.Guid(nullable: false),
                        UserId = c.Int(nullable: false),
                        TargetWord = c.String(nullable: false, maxLength: 5),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        Attempts = c.Int(nullable: false),
                        IsWin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.GameId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Guesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GameId = c.Guid(nullable: false),
                        Word = c.String(nullable: false, maxLength: 5),
                        GuessNumber = c.Int(nullable: false),
                        GuessResultJson = c.String(nullable: false),
                        GuessTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .Index(t => t.GameId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 256),
                        PasswordHash = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Statistics",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        GamesPlayed = c.Int(nullable: false),
                        Wins = c.Int(nullable: false),
                        CurrentStreak = c.Int(nullable: false),
                        MaxStreak = c.Int(nullable: false),
                        TotalPoints = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Statistics", "UserId", "dbo.Users");
            DropForeignKey("dbo.Games", "UserId", "dbo.Users");
            DropForeignKey("dbo.Guesses", "GameId", "dbo.Games");
            DropIndex("dbo.Statistics", new[] { "UserId" });
            DropIndex("dbo.Guesses", new[] { "GameId" });
            DropIndex("dbo.Games", new[] { "UserId" });
            DropTable("dbo.Statistics");
            DropTable("dbo.Users");
            DropTable("dbo.Guesses");
            DropTable("dbo.Games");
        }
    }
}
