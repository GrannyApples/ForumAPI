using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForumAPI.Data;
using ForumAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ForumAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _context.Posts.Include(p => p.Comments).ToListAsync();
        }

        //  api/Posts/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // api/Posts/id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest("Post ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPost = await _context.Posts.FindAsync(id);
            if (existingPost == null)
            {
                return NotFound("Post not found");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || (user.Id != existingPost.UserId && !user.IsAdmin))
            {
                return Forbid("You are not allowed to edit this post");
            }

            existingPost.Title = post.Title;
            existingPost.Text = post.Text;
            existingPost.Image = post.Image;

            _context.Entry(existingPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound("Post not found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // api/Posts
        [HttpPost]
        public async Task<ActionResult<Post>> CreatePost(Post post)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            post.UserId = user.Id;
            post.Author = user.UserName;
            post.CreateDate = DateTime.UtcNow;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        //  api/Posts/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || (user.Id != post.UserId && !user.IsAdmin))
            {
                return Forbid("You are not allowed to delete this post");
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // api/Posts/id/Comments
        [HttpGet("{postId}/comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int postId)
        {
            var post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                return NotFound();
            }

            return post.Comments.ToList();
        }

        // api/Posts/id/Comments
        [HttpPost("{postId}/comments")]
        public async Task<ActionResult<Comment>> CreateComment(int postId, Comment comment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            comment.PostId = postId;
            comment.UserId = user.Id;
            comment.Author = user.UserName;
            comment.CreateDate = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = postId }, comment);
        }

        // api/Posts/id/Comments/id
        [HttpDelete("{postId}/comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(int postId, int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || (user.Id != comment.UserId && !user.IsAdmin))
            {
                return Forbid("You are not allowed to delete this comment");
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // api/Posts/id/Report
        [HttpPost("{postId}/report")]
        public async Task<IActionResult> ReportPost(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            post.IsReported = true;

            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // api/Posts/5/Comments/id/Report
        [HttpPost("{postId}/comments/{commentId}/report")]
        public async Task<IActionResult> ReportComment(int postId, int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            comment.IsReported = true;

            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // api/posts/reported
        [HttpGet("reported")]
        public async Task<ActionResult<IEnumerable<Post>>> GetReportedPosts()
        {
            return await _context.Posts.Where(p => p.IsReported).ToListAsync();
        }
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ForumAPI.Data;
//using ForumAPI.Models;

//namespace ForumAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PostsController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<ApplicationUser> _userManager;

//        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }

//        // GET: api/posts
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
//        {
//            return await _context.Posts.ToListAsync();
//        }

//        // GET: api/posts/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<Post>> GetPost(int id)
//        {
//            var post = await _context.Posts.FindAsync(id);

//            if (post == null)
//            {
//                return NotFound();
//            }

//            return post;
//        }

//        // POST: api/posts
//        [HttpPost]
//        public async Task<ActionResult<Post>> CreatePost(Post post)
//        {
//            var user = await _userManager.GetUserAsync(User);
//            if (user == null)
//            {
//                return Unauthorized();
//            }

//            post.UserId = user.Id;
//            post.Author = user.UserName;
//            post.CreateDate = DateTime.UtcNow;
//            _context.Posts.Add(post);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
//        }

//        // PUT: api/posts/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdatePost(int id, Post post)
//        {
//            if (id != post.Id)
//            {
//                return BadRequest();
//            }

//            _context.Entry(post).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!PostExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // DELETE: api/posts/5
//        [HttpDelete("{id}")]
//        [Authorize]
//        public async Task<IActionResult> DeletePost(int id)
//        {
//            var post = await _context.Posts.FindAsync(id);
//            if (post == null)
//            {
//                return NotFound();
//            }

//            var user = await _userManager.GetUserAsync(User);
//            if (user == null || (post.UserId != user.Id && !await _userManager.IsInRoleAsync(user, "Admin")))
//            {
//                return Forbid();
//            }

//            _context.Posts.Remove(post);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        // POST: api/posts/report
//        [HttpPost("report")]
//        [Authorize]
//        public async Task<IActionResult> ReportPost(ReportedItem reportedItem)
//        {
//            reportedItem.ReportDate = DateTime.UtcNow;
//            reportedItem.IsReviewed = false;
//            _context.ReportedItems.Add(reportedItem);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool PostExists(int id)
//        {
//            return _context.Posts.Any(e => e.Id == id);
//        }
//    }
//}

////using Microsoft.AspNetCore.Authorization;
////using Microsoft.AspNetCore.Identity;
////using Microsoft.AspNetCore.Mvc;
////using Microsoft.EntityFrameworkCore;
////using ForumAPI.Data;
////using ForumAPI.Models;
////using System.Collections.Generic;
////using System.Linq;
////using System.Threading.Tasks;

////namespace ForumAPI.Controllers
////{
////    [Authorize]
////    [Route("api/[controller]")]
////    [ApiController]
////    public class PostsController : ControllerBase
////    {
////        private readonly ApplicationDbContext _context;
////        private readonly UserManager<ApplicationUser> _userManager;

////        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
////        {
////            _context = context;
////            _userManager = userManager;
////        }

////        // GET: api/posts
////        [HttpGet]
////        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
////        {
////            return await _context.Posts.Include(p => p.Comments).ToListAsync();
////        }

////        // POST: api/posts
////        [HttpPost]
////        public async Task<ActionResult<Post>> CreatePost(Post post)
////        {
////            var user = await _userManager.GetUserAsync(User);
////            if (user == null)
////            {
////                return Unauthorized();
////            }

////            post.UserId = user.Id;
////            post.Author = user.UserName;
////            _context.Posts.Add(post);
////            await _context.SaveChangesAsync();

////            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
////        }

////        // GET: api/posts/5
////        [HttpGet("{id}")]
////        public async Task<ActionResult<Post>> GetPost(int id)
////        {
////            var post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id);

////            if (post == null)
////            {
////                return NotFound();
////            }

////            return post;
////        }

////        // DELETE: api/posts/5
////        [HttpDelete("{id}")]
////        public async Task<IActionResult> DeletePost(int id)
////        {
////            var post = await _context.Posts.FindAsync(id);
////            if (post == null)
////            {
////                return NotFound();
////            }

////            var user = await _userManager.GetUserAsync(User);
////            if (user == null || (!user.IsAdmin && post.UserId != user.Id))
////            {
////                return Forbid();
////            }

////            _context.Posts.Remove(post);
////            await _context.SaveChangesAsync();

////            return NoContent();
////        }

////        // POST: api/posts/5/comments
////        [HttpPost("{id}/comments")]
////        public async Task<ActionResult<Comment>> CreateComment(int id, Comment comment)
////        {
////            var post = await _context.Posts.FindAsync(id);
////            if (post == null)
////            {
////                return NotFound();
////            }

////            var user = await _userManager.GetUserAsync(User);
////            if (user == null)
////            {
////                return Unauthorized();
////            }

////            comment.PostId = id;
////            comment.Author = user.UserName;
////            _context.Comments.Add(comment);
////            await _context.SaveChangesAsync();

////            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
////        }

////        // GET: api/posts/5/comments/1
////        [HttpGet("{postId}/comments/{commentId}")]
////        public async Task<ActionResult<Comment>> GetComment(int postId, int commentId)
////        {
////            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.PostId == postId);

////            if (comment == null)
////            {
////                return NotFound();
////            }

////            return comment;
////        }

////        // DELETE: api/posts/5/comments/1
////        [HttpDelete("{postId}/comments/{commentId}")]
////        public async Task<IActionResult> DeleteComment(int postId, int commentId)
////        {
////            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.PostId == postId);
////            if (comment == null)
////            {
////                return NotFound();
////            }

////            var user = await _userManager.GetUserAsync(User);
////            if (user == null || (!user.IsAdmin && comment.Author != user.UserName))
////            {
////                return Forbid();
////            }

////            _context.Comments.Remove(comment);
////            await _context.SaveChangesAsync();

////            return NoContent();
////        }

////        // POST: api/posts/5/report
////        [HttpPost("{id}/report")]
////        public async Task<IActionResult> ReportPost(int id)
////        {
////            var post = await _context.Posts.FindAsync(id);
////            if (post == null)
////            {
////                return NotFound();
////            }

////            post.IsReported = true;
////            await _context.SaveChangesAsync();

////            return NoContent();
////        }

////        // POST: api/posts/5/comments/1/report
////        [HttpPost("{postId}/comments/{commentId}/report")]
////        public async Task<IActionResult> ReportComment(int postId, int commentId)
////        {
////            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.PostId == postId);
////            if (comment == null)
////            {
////                return NotFound();
////            }

////            comment.IsReported = true;
////            await _context.SaveChangesAsync();

////            return NoContent();
////        }

////        // GET: api/posts/reported
////        [Authorize(Roles = "Admin")]
////        [HttpGet("reported")]
////        public async Task<ActionResult<IEnumerable<Post>>> GetReportedPosts()
////        {
////            var reportedPosts = await _context.Posts.Where(p => p.IsReported).ToListAsync();
////            return reportedPosts;
////        }

////        // GET: api/posts/{postId}/comments/reported
////        [Authorize(Roles = "Admin")]
////        [HttpGet("{postId}/comments/reported")]
////        public async Task<ActionResult<IEnumerable<Comment>>> GetReportedComments(int postId)
////        {
////            var reportedComments = await _context.Comments.Where(c => c.PostId == postId && c.IsReported).ToListAsync();
////            return reportedComments;
////        }
////    }
////}