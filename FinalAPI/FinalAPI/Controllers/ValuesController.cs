using System.Collections.Generic;
using System.Web.Http;

namespace FinalAPI.Controllers
{
    /// <summary>
    /// Controller for managing sample values.
    /// </summary>
    public class ValuesController : ApiController
    {
        /// <summary>
        /// Retrieves all values.
        /// </summary>
        /// <returns>A list of string values.</returns>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Retrieves a value by ID.
        /// </summary>
        /// <param name="id">The ID of the value to retrieve.</param>
        /// <returns>The value associated with the specified ID.</returns>
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Creates a new value.
        /// </summary>
        /// <param name="value">The value to create.</param>
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// Updates an existing value.
        /// </summary>
        /// <param name="id">The ID of the value to update.</param>
        /// <param name="value">The updated value.</param>
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// Deletes a value by ID.
        /// </summary>
        /// <param name="id">The ID of the value to delete.</param>
        public void Delete(int id)
        {
        }
    }
}