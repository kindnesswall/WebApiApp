namespace KindnessWall.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Bookmark : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookmarks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        GiftId = c.Int(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Gifts", t => t.GiftId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.GiftId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookmarks", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Bookmarks", "GiftId", "dbo.Gifts");
            DropIndex("dbo.Bookmarks", new[] { "GiftId" });
            DropIndex("dbo.Bookmarks", new[] { "UserId" });
            DropTable("dbo.Bookmarks");
        }
    }
}
