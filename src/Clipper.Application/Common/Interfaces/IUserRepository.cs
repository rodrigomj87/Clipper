using Clipper.Domain.Entities;

namespace Clipper.Application.Common.Interfaces;

/// <summary>
/// Interface para repositório de usuários
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Busca um usuário por email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Busca um usuário por ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<User?> GetByIdAsync(int id);

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    /// <param name="user">Usuário a criar</param>
    /// <returns>Usuário criado</returns>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    /// <param name="user">Usuário a atualizar</param>
    /// <returns>Usuário atualizado</returns>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// Verifica se já existe um usuário com o email informado
    /// </summary>
    /// <param name="email">Email a verificar</param>
    /// <returns>True se email já existe</returns>
    Task<bool> EmailExistsAsync(string email);

    /// <summary>
    /// Verifica se existe um usuário com o email específico (método alternativo)
    /// </summary>
    /// <param name="email">Email a verificar</param>
    /// <returns>True se usuário existe com esse email</returns>
    Task<bool> ExistsWithEmailAsync(string email);
}
