
using FeedbackAI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
public class ApplicationDBContext : DbContext
{
    private readonly IConfiguration configuration;
    public ApplicationDBContext(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public DbSet<Feedback> Feedbacks { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
           => optionsBuilder.UseNpgsql(configuration.GetConnectionString("DatabaseConnection"));

}

