namespace KindnessWall.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class GiftImage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GiftImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GiftId = c.Int(nullable: false),
                        ImageUrl = c.String(),
                        UserId = c.String(maxLength: 128),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Gifts", t => t.GiftId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.GiftId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GiftImages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GiftImages", "GiftId", "dbo.Gifts");
            DropIndex("dbo.GiftImages", new[] { "UserId" });
            DropIndex("dbo.GiftImages", new[] { "GiftId" });
            DropTable("dbo.GiftImages");
        }
    }
}
