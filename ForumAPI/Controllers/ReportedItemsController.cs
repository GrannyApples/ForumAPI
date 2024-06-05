using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForumAPI.Data;
using ForumAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportedItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportedItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/ReportedItems/Posts
        [HttpGet("Posts")]
        public async Task<ActionResult<IEnumerable<Post>>> GetReportedPosts()
        {
            return await _context.Posts.Where(p => p.IsReported).ToListAsync();
        }

        // GET: api/ReportedItems/Comments
        [HttpGet("Comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetReportedComments()
        {
            return await _context.Comments.Where(c => c.IsReported).ToListAsync();
        }

        // DELETE: api/ReportedItems/Posts/5
        [HttpDelete("Posts/{id}")]
        public async Task<IActionResult> DeleteReportedPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ReportedItems/Comments/5
        [HttpDelete("Comments/{id}")]
        public async Task<IActionResult> DeleteReportedComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/ReportedItems/Posts/5/Unreport
        [HttpPut("Posts/{id}/Unreport")]
        public async Task<IActionResult> UnreportPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.IsReported = false;
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/ReportedItems/Comments/5/Unreport
        [HttpPut("Comments/{id}/Unreport")]
        public async Task<IActionResult> UnreportComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.IsReported = false;
            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}