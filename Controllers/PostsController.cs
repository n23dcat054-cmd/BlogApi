using BlogApi.Data;
using BlogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PostsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/posts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetAll()
    {
        return await _db.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    // GET: api/posts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Post>> GetById(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null) return NotFound();
        return post;
    }

    // POST: api/posts
    [HttpPost]
    public async Task<ActionResult<Post>> Create(Post post)
    {
        post.Id = 0;
        post.CreatedAt = DateTime.UtcNow;
        _db.Posts.Add(post);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    // PUT: api/posts/5
    [HttpPut("{id}")]
    public async Task<ActionResult<Post>> Update(int id, Post input)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null) return NotFound();

        post.Title = input.Title;
        post.Tag = input.Tag;
        post.Summary = input.Summary;
        post.Content = input.Content;

        await _db.SaveChangesAsync();
        return post;
    }

    // DELETE: api/posts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null) return NotFound();

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}