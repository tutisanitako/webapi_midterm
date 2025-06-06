using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    /// <summary>
    /// Defines a generic interface for basic CRUD (Create, Read, Update, Delete) operations
    /// on entities within the domain.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Asynchronously retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity (or null if not found).</returns>
        Task<TEntity> GetByIdAsync(int id); // Removed '?' from TEntity

        /// <summary>
        /// Asynchronously retrieves all entities of a specific type.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of entities.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(); // Changed back to IEnumerable<TEntity> to match your RepositoryBase

        /// <summary>
        /// Asynchronously adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// Asynchronously updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Asynchronously deletes an entity from the repository by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteAsync(int id);
    }
}
