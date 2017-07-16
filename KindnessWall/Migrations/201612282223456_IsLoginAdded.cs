namespace KindnessWall.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class IsLoginAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsLogin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsLogin");
        }
    }
}
