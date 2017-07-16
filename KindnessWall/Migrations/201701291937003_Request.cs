namespace KindnessWall.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Request : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GiftId = c.Int(nullable: false),
                        FromUserId = c.String(nullable: false, maxLength: 128),
                        ToUserId = c.String(nullable: false, maxLength: 128),
                        FromStatus = c.Byte(nullable: false),
                        ToStatus = c.Byte(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.FromUserId)
                .ForeignKey("dbo.Gifts", t => t.GiftId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ToUserId)
                .Index(t => t.GiftId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Requests", "ToUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Requests", "GiftId", "dbo.Gifts");
            DropForeignKey("dbo.Requests", "FromUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Requests", new[] { "ToUserId" });
            DropIndex("dbo.Requests", new[] { "FromUserId" });
            DropIndex("dbo.Requests", new[] { "GiftId" });
            DropTable("dbo.Requests");
        }
    }
}
