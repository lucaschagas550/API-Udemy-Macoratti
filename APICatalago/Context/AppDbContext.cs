using APICatalago.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalago.Context
{
    public class AppDbContext : IdentityDbContext //IDentity para segurança e autenticação//DbContext // permite usar o DbSet
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Categoria> Catergorias { get; set; } // mapeia a tabela Categorias, baseada nas entidades
        public DbSet<Produto> Produtos { get; set; } // mapeia a tabela Produto, baseada nas entidades
    }
}
