namespace KindnessWall.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewFixes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Gifts", new[] { "UserId" });
            AddColumn("dbo.Gifts", "ReceivedUserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Gifts", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Gifts", "UserId");
            CreateIndex("dbo.Gifts", "ReceivedUserId");
            AddForeignKey("dbo.Gifts", "ReceivedUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Gifts", "ReceivedUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Gifts", new[] { "ReceivedUserId" });
            DropIndex("dbo.Gifts", new[] { "UserId" });
            AlterColumn("dbo.Gifts", "UserId", c => c.String(maxLength: 128));
            DropColumn("dbo.Gifts", "ReceivedUserId");
            CreateIndex("dbo.Gifts", "UserId");
        }
    }
}
