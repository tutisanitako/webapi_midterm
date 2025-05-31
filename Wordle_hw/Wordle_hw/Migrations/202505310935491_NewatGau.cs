namespace Wordle_hw.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewatGau : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TargetWord = c.String(nullable: false, maxLength: 10),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        Attempts = c.Int(nullable: false),
                        IsWin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Guesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GameId = c.Int(nullable: false),
                        Word = c.String(nullable: false, maxLength: 10),
                        GuessNumber = c.Int(nullable: false),
                        GuessResult = c.String(maxLength: 50),
                        GuessedColor = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .Index(t => t.GameId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 50),
                        PasswordHash = c.String(nullable: false),
                        Email = c.String(nullable: false, maxLength: 100),
                        Statistics_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Statistics", t => t.Statistics_Id)
                .Index(t => t.Username, unique: true)
                .Index(t => t.Email, unique: true)
                .Index(t => t.Statistics_Id);
            
            CreateTable(
                "dbo.Statistics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        GamesPlayed = c.Int(nullable: false),
                        Wins = c.Int(nullable: false),
                        CurrentStreak = c.Int(nullable: false),
                        MaxStreak = c.Int(nullable: false),
                        TotalPoints = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "Statistics_Id", "dbo.Statistics");
            DropForeignKey("dbo.Statistics", "UserId", "dbo.Users");
            DropForeignKey("dbo.Guesses", "GameId", "dbo.Games");
            DropIndex("dbo.Statistics", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "Statistics_Id" });
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.Users", new[] { "Username" });
            DropIndex("dbo.Guesses", new[] { "GameId" });
            DropIndex("dbo.Games", new[] { "UserId" });
            DropTable("dbo.Statistics");
            DropTable("dbo.Users");
            DropTable("dbo.Guesses");
            DropTable("dbo.Games");
        }
    }
}
