using Microsoft.EntityFrameworkCore;
using Clipper.Application.Common.Interfaces;
using Clipper.Domain.Entities;
using Clipper.Infrastructure.Data;

namespace Clipper.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de usuários
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ClipperDbContext _context;

    public UserRepository(ClipperDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca um usuário por email
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    /// <summary>
    /// Busca um usuário por ID
    /// </summary>
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Verifica se já existe um usuário com o email informado
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    /// <summary>
    /// Verifica se existe um usuário com o email específico (método alternativo)
    /// </summary>
    public async Task<bool> ExistsWithEmailAsync(string email)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
}
