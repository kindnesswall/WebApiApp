namespace KindnessWall.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Firebase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        DeviceId = c.String(nullable: false, maxLength: 128),
                        RegisterationId = c.String(),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.DeviceId);
            
            AddColumn("dbo.AspNetUsers", "NotificationKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "NotificationKey");
            DropTable("dbo.Devices");
        }
    }
}
