using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using News_Portal_API.Data;
using News_Portal_API.DTOs;
using News_Portal_API.DTOs.News;
using News_Portal_API.Entities;

namespace News_Portal_API.Controllers;

[ApiController]
[Route("api/news")]
public class NewsController : ControllerBase
{
    private readonly AppDbContext _context;

    public NewsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedNews()
    {
        try
        {
            var article = await _context.Articles
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.Author)
                .Include(a => a.ArticleStat)
                .Where(a => a.IsFeatured == true)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (article == null)
            {
                return NotFound(new ApiResponse<ArticleDto>
                {
                    Success = false,
                    Message = "Featured news not found"
                });
            }

            return Ok(new ApiResponse<ArticleDto>
            {
                Success = true,
                Message = "OK",
                Data = MapArticle(article)
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ApiResponse<ArticleDto>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("breaking")]
    public async Task<IActionResult> GetBreakingNews()
    {
        try
        {
            var articles = await _context.Articles
                .AsNoTracking()
                .Where(a => a.IsBreaking == true)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new BreakingNewsDto
                {
                    Id = a.Id,
                    Title = a.Title
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<BreakingNewsDto>>
            {
                Success = true,
                Message = "OK",
                Data = articles
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ApiResponse<List<BreakingNewsDto>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("articles")]
    public async Task<IActionResult> GetArticles(
        [FromQuery] string tab = "latest",
        [FromQuery] int page = 1,
        [FromQuery] int limit = 4)
    {
        if (!new[] { "latest", "think", "health" }
            .Contains(tab, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new ApiResponse<List<ArticleDto>>
            {
                Success = false,
                Message = "Invalid tab. Allowed values: latest, think, health"
            });
        }

        if (page < 1 || limit < 1)
        {
            return BadRequest(new ApiResponse<List<ArticleDto>>
            {
                Success = false,
                Message = "Page and limit must be greater than zero"
            });
        }

        try
        {
            var category = tab.ToLowerInvariant() switch
            {
                "think" => "Think",
                "health" => "Health",
                _ => null
            };

            var query = _context.Articles
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.Author)
                .Include(a => a.ArticleStat)
                .AsQueryable();

            if (category != null)
            {
                query = query.Where(a =>
                    a.Category != null && a.Category.Name == category);
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalItems / limit);

            var articles = await query
                .OrderByDescending(a => a.CreatedAt)
                .ThenByDescending(a => a.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new ApiResponse<List<ArticleDto>>
            {
                Success = true,
                Message = "OK",
                Data = articles.Select(MapArticle).ToList(),
                Pagination = new Pagination
                {
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalItems,
                    Limit = limit
                }
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ApiResponse<List<ArticleDto>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("live")]
    public async Task<IActionResult> GetLiveNews()
    {
        try
        {
            var article = await _context.Articles
                .AsNoTracking()
                .Include(a => a.Category)
                .Where(a => a.IsLive == true)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (article == null)
            {
                return NotFound(new ApiResponse<LiveNewsDto>
                {
                    Success = false,
                    Message = "Live news not found"
                });
            }

            var liveNews = new LiveNewsDto
            {
                Id = article.Id,
                Title = article.Title,
                ImageUrl = article.ImageUrl,
                Category = new LiveCategoryDto
                {
                    Name = article.Category?.Name ?? string.Empty
                }
            };

            return Ok(new ApiResponse<LiveNewsDto>
            {
                Success = true,
                Message = "OK",
                Data = liveNews
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ApiResponse<LiveNewsDto>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("editors-picks")]
    public async Task<IActionResult> GetEditorsPicks()
    {
        try
        {
            var articles = await _context.Articles
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.ArticleStat)
                .Where(a => a.IsEditorsPick == true)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var editorsPicks = articles.Select(a => new EditorsPickDto
            {
                Id = a.Id,
                Title = a.Title,
                ImageUrl = a.ImageUrl,

                Category = new EditorsPickCategoryDto
                {
                    Id = a.Category?.Id ?? 0,
                    Name = a.Category?.Name ?? string.Empty,
                    BadgeColor = a.Category?.BadgeColor
                },

                Stats = new ArticleStatsDto
                {
                    LikesCount = a.ArticleStat?.LikesCount ?? 0,
                    SharesCount = a.ArticleStat?.SharesCount ?? 0,
                    BookmarksCount = a.ArticleStat?.BookmarksCount ?? 0
                }
            }).ToList();

            return Ok(new ApiResponse<List<EditorsPickDto>>
            {
                Success = true,
                Message = "OK",
                Data = editorsPicks
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ApiResponse<List<EditorsPickDto>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}/related")]
    public async Task<IActionResult> GetRelatedNews(
        string id,
        [FromQuery] int limit = 5)
    {
        if (!int.TryParse(id, out int newsId))
        {
            return BadRequest(new ApiResponse<List<RelatedNewsDto>>
            {
                Success = false,
                Message = "Invalid news ID. ID must be a number."
            });
        }

        if (limit < 1)
        {
            return BadRequest(new ApiResponse<List<RelatedNewsDto>>
            {
                Success = false,
                Message = "Limit must be greater than zero."
            });
        }

        try
        {
            var article = await _context.Articles
                .AsNoTracking()
                .Where(a => a.Id == newsId)
                .Select(a => new { a.CategoryId })
                .FirstOrDefaultAsync();

            if (article == null)
            {
                return NotFound(new ApiResponse<List<RelatedNewsDto>>
                {
                    Success = false,
                    Message = "News not found."
                });
            }

            if (article.CategoryId == null)
            {
                return Ok(new ApiResponse<List<RelatedNewsDto>>
                {
                    Success = true,
                    Message = "OK",
                    Data = new List<RelatedNewsDto>()
                });
            }

            var relatedArticles = await _context.Articles
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.ArticleStat)
                .Where(a =>
                    a.CategoryId == article.CategoryId &&
                    a.Id != newsId)
                .OrderByDescending(a => a.CreatedAt)
                .Take(limit)
                .ToListAsync();

            var relatedNews = relatedArticles.Select(a => new RelatedNewsDto
            {
                Id = a.Id,
                Title = a.Title,
                ShortDescription = a.ShortDescription,
                ImageUrl = a.ImageUrl,

                Category = new RelatedNewsCategoryDto
                {
                    Id = a.Category?.Id ?? 0,
                    Name = a.Category?.Name ?? string.Empty,
                    BadgeColor = a.Category?.BadgeColor
                },

                Stats = new RelatedNewsStatsDto
                {
                    LikesCount = a.ArticleStat?.LikesCount ?? 0,
                    SharesCount = a.ArticleStat?.SharesCount ?? 0,
                    BookmarksCount = a.ArticleStat?.BookmarksCount ?? 0
                }
            }).ToList();

            return Ok(new ApiResponse<List<RelatedNewsDto>>
            {
                Success = true,
                Message = "OK",
                Data = relatedNews
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ApiResponse<List<RelatedNewsDto>>
            {
                Success = false,
                Message = "An internal server error occurred."
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNewsDetail(string id)
    {
        if (!int.TryParse(id, out int newsId))
        {
            return BadRequest(new ApiResponse<NewsDetailDto>
            {
                Success = false,
                Message = "Invalid news ID. ID must be a number."
            });
        }

        try
        {
            var article = await _context.Articles
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.Author)
                .Include(a => a.ArticleStat)
                .FirstOrDefaultAsync(a => a.Id == newsId);

            if (article == null)
            {
                return NotFound(new ApiResponse<NewsDetailDto>
                {
                    Success = false,
                    Message = "News not found."
                });
            }

            var news = new NewsDetailDto
            {
                Id = article.Id,
                Title = article.Title,
                ShortDescription = article.ShortDescription,
                Content = article.Content,
                ImageUrl = article.ImageUrl,
                CreatedAt = article.CreatedAt,
                ReadTimeMinutes = article.ReadTimeMinutes,
                IsTrending = article.IsTrending ?? false,
                IsBreaking = article.IsBreaking ?? false,
                IsLive = article.IsLive ?? false,

                Category = article.Category == null ? null! : new NewsDetailCategoryDto
                {
                    Id = article.Category.Id,
                    Name = article.Category.Name,
                    Slug = article.Category.Slug,
                    BadgeColor = article.Category.BadgeColor
                },

                Author = article.Author == null ? null! : new NewsDetailAuthorDto
                {
                    Id = article.Author.Id,
                    Name = article.Author.Name,
                    AvatarUrl = article.Author.AvatarUrl,
                    Bio = article.Author.Bio,
                    TwitterHandle = article.Author.TwitterHandle
                },

                Stats = new NewsDetailStatsDto
                {
                    LikesCount = article.ArticleStat?.LikesCount ?? 0,
                    SharesCount = article.ArticleStat?.SharesCount ?? 0,
                    BookmarksCount = article.ArticleStat?.BookmarksCount ?? 0,
                    CommentsCount = article.ArticleStat?.CommentsCount ?? 0
                }
            };

            return Ok(new ApiResponse<NewsDetailDto>
            {
                Success = true,
                Message = "OK",
                Data = news
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ApiResponse<NewsDetailDto>
            {
                Success = false,
                Message = "An internal server error occurred."
            });
        }
    }

    private static ArticleDto MapArticle(Article article)
    {
        return new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            ShortDescription = article.ShortDescription,
            ImageUrl = article.ImageUrl,
            CreatedAt = article.CreatedAt,
            ReadTimeMinutes = article.ReadTimeMinutes,
            IsTrending = article.IsTrending ?? false,

            Category = article.Category == null ? null! : new CategoryDto
            {
                Id = article.Category.Id,
                Name = article.Category.Name,
                Slug = article.Category.Slug,
                BadgeColor = article.Category.BadgeColor
            },

            Author = article.Author == null ? null! : new AuthorDto
            {
                Id = article.Author.Id,
                Name = article.Author.Name
            },

            Stats = new ArticleStatsDto
            {
                LikesCount = article.ArticleStat?.LikesCount ?? 0,
                SharesCount = article.ArticleStat?.SharesCount ?? 0,
                BookmarksCount = article.ArticleStat?.BookmarksCount ?? 0
            }
        };
    }
}