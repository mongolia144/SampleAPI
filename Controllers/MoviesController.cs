using Microsoft.AspNetCore.Mvc;
using SampleApi.Data;
using SampleApi.Models;

namespace SampleApi.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    /// <summary>
    /// The rule of thumb
    ///Write operations → Task<IActionResult>: Typical responses:
    /// Task → because saving to the database is async (await _db.SaveChangesAsync()).
    /// IActionResult → because you need to return HTTP status codes, not data.
    /// CreatedAtAction(...),BadRequest(), Ok(), NoContent()
    ///Read single item → ActionResult<T>: it allows you to return either: 
    ///a Movie object (200 OK),a NotFound() (404), a BadRequest() (400)
    ///Read list → IEnumerable<T>
    /// </summary>
    private readonly AppDbContext _db;

    public MoviesController(AppDbContext db) => _db = db;

    [HttpGet]
    public IEnumerable<Movie> Get() => _db.Movies.ToList();

    

    [HttpPost]
    public async Task<IActionResult> Create(Movie movie)
    {
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
    }

    [HttpGet("{id}")]
    //With [HttpGet("{id}")] we can issue this command GET /movies/123
    //With [HttpGet()] we can issue this command GET /movies?id=123
    //public async ActionResult<Movie> GetById(string id)
    public async Task<ActionResult<Movie>>  GetById(string id) 
    {
        // Because FindAsync is async then we need to use Task<ActionResult<Movie>> and not ActionResult<Movie>
        //FindAsnc is faster than where when we are looking for the primary key and has not an Include.
        var movie = await _db.Movies.FindAsync(id);
        //var movie = _db.Movies.FirstOrDefault(m => m.Id == id);

        if (movie == null)
            return NotFound();

        return movie;
    }

    //For updates we can use PUT or PATCH. PUT updates the entire record while PATCH signals only updating some fields.
    // In practice: Most APIs use only PUT for updates. PATCH is optional and often ignored. Even Microsoft, Google, and many enterprise APIs use PUT for all updates, unless they need advanced partial update behavior.
    // Analogy: in SQL we do not have ONLY one UPDATE method. Not 2 methods: one for the entire row and another one for partial updates.
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Movie updated)
    {
        var existing = await _db.Movies.FindAsync(id);

        if (existing == null)
            return NotFound();

        existing.Title = updated.Title;
        existing.Year = updated.Year;

        await _db.SaveChangesAsync();

        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var movie = await _db.Movies.FindAsync(id);

        if (movie == null)
            return NotFound();

        _db.Movies.Remove(movie);
        await _db.SaveChangesAsync();

        return NoContent();
        //After deleting, the API should return:
        // 204 No Content → deletion succeeded
        // 404 Not Found → item doesn’t exist
        // Returning the deleted object is not recommended.
    }
}

