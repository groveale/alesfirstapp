namespace Backend.Migrations
{
    using DataObjects;
    using Microsoft.Azure.Mobile.Server.Tables;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Backend.Models.MobileServiceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            SetSqlGenerator("System.Data.SqlClient", new EntityTableSqlGenerator());
        }

        protected override void Seed(Backend.Models.MobileServiceContext context)
        {
            /*
            List<TodoItem> todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "First item", Complete = false },
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false }
            };

            foreach (TodoItem todoItem in todoItems)
            {
                context.Set<TodoItem>().Add(todoItem);
            }

            List<User> users = new List<User>
            {
                new User { Id = 1, Username = "ales", Password = "password" }
            };

            foreach (User user in users)
            {
                context.Set<User>().Add(user);
            }
            */

            base.Seed(context);
        }
    }
}
