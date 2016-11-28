namespace Backend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class todoITemupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TodoItems", "Photo", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TodoItems", "Photo");
        }
    }
}
