using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using ForumAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ForumAPI.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
               serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string[] roleNames = { "Admin", "User" };
                IdentityResult roleResult;

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                var adminUser = new ApplicationUser
                {
                    UserName = "1@1.se",
                    Email = "1@1.se",
                    EmailConfirmed = true,
                    IsAdmin = true
                };

                string adminPassword = "Admin!1234";
                var user = await userManager.FindByEmailAsync(adminUser.Email);

                if (user == null)
                {
                    var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
                    if (createAdmin.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                // Ensure database is created
                context.Database.EnsureCreated();

                // Seed Regular User
                var regularUser = new ApplicationUser
                {
                    UserName = "user@forum.com",
                    Email = "user@forum.com",
                    EmailConfirmed = true,
                    IsAdmin = false
                };

                string userPassword = "User!1234";
                var existingUser = await userManager.FindByEmailAsync(regularUser.Email);

                if (existingUser == null)
                {
                    var createUser = await userManager.CreateAsync(regularUser, userPassword);
                    if (createUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(regularUser, "User");
                    }
                }

                // Seed Posts
                if (!context.Posts.Any())
                {
                    context.Posts.AddRange(
                        new Post
                        {
                            Title = "First Post",
                            Text = "This is the first post.",
                            Author = adminUser.UserName,
                            CreateDate = DateTime.UtcNow,
                            UserId = (await userManager.FindByEmailAsync(adminUser.Email)).Id
                        },
                        new Post
                        {
                            Title = "Second Post",
                            Text = "This is the second post.",
                            Author = regularUser.UserName,
                            CreateDate = DateTime.UtcNow,
                            UserId = (await userManager.FindByEmailAsync(regularUser.Email)).Id
                        }
                    );

                    await context.SaveChangesAsync();
                }

                // Seed Comments
                if (!context.Comments.Any())
                {
                    var firstPost = context.Posts.FirstOrDefault(p => p.Title == "First Post");
                    var secondPost = context.Posts.FirstOrDefault(p => p.Title == "Second Post");

                    if (firstPost != null && secondPost != null)
                    {
                        context.Comments.AddRange(
                            new Comment
                            {
                                Text = "This is a comment on the first post.",
                                Author = regularUser.UserName,
                                CreateDate = DateTime.UtcNow,
                                PostId = firstPost.Id,
                                UserId = (await userManager.FindByEmailAsync(regularUser.Email)).Id
                            },
                            new Comment
                            {
                                Text = "This is another comment on the first post.",
                                Author = adminUser.UserName,
                                CreateDate = DateTime.UtcNow,
                                PostId = firstPost.Id,
                                UserId = (await userManager.FindByEmailAsync(adminUser.Email)).Id
                            },
                            new Comment
                            {
                                Text = "This is a comment on the second post.",
                                Author = adminUser.UserName,
                                CreateDate = DateTime.UtcNow,
                                PostId = secondPost.Id,
                                UserId = (await userManager.FindByEmailAsync(adminUser.Email)).Id
                            }
                        );

                        await context.SaveChangesAsync();
                    }
                }

            }
        }
    }
}
