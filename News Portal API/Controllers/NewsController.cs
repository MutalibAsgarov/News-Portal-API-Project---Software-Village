using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using News_Portal_API.Data;
using News_Portal_API.DTOs;
using News_Portal_API.DTOs.News;

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
            var connection = _context.Database.GetDbConnection();

            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            command.CommandText = @"
            SELECT TOP 1
                a.id,
                a.title,
                a.short_description,
                a.image_url,
                a.created_at,
                a.read_time_minutes,
                a.is_trending,
                c.id AS category_id,
                c.name AS category_name,
                c.slug AS category_slug,
                c.badge_color,
                au.id AS author_id,
                au.name AS author_name,
                ISNULL(s.likes_count, 0) AS likes_count,
                ISNULL(s.shares_count, 0) AS shares_count,
                ISNULL(s.bookmarks_count, 0) AS bookmarks_count
            FROM articles a
            LEFT JOIN categories c ON a.category_id = c.id
            LEFT JOIN authors au ON a.author_id = au.id
            LEFT JOIN article_stats s ON a.id = s.article_id
            WHERE a.is_featured = 1
            ORDER BY a.created_at DESC";

            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return NotFound(new ApiResponse<ArticleDto>
                {
                    Success = false,
                    Message = "Featured news not found"
                });
            }

            var article = new ArticleDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                ShortDescription = reader.GetString(reader.GetOrdinal("short_description")),
                ImageUrl = reader.GetString(reader.GetOrdinal("image_url")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                ReadTimeMinutes = reader.GetInt32(reader.GetOrdinal("read_time_minutes")),
                IsTrending = reader.GetBoolean(reader.GetOrdinal("is_trending")),

                Category = new CategoryDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("category_id")),
                    Name = reader.GetString(reader.GetOrdinal("category_name")),
                    Slug = reader.GetString(reader.GetOrdinal("category_slug")),
                    BadgeColor = reader.GetString(reader.GetOrdinal("badge_color"))
                },

                Author = new AuthorDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("author_id")),
                    Name = reader.GetString(reader.GetOrdinal("author_name"))
                },

                Stats = new ArticleStatsDto
                {
                    LikesCount = reader.GetInt32(reader.GetOrdinal("likes_count")),
                    SharesCount = reader.GetInt32(reader.GetOrdinal("shares_count")),
                    BookmarksCount = reader.GetInt32(reader.GetOrdinal("bookmarks_count"))
                }
            };

            return Ok(new ApiResponse<ArticleDto>
            {
                Success = true,
                Message = "OK",
                Data = article
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ArticleDto>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
        finally
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }

    [HttpGet("breaking")]
    public async Task<IActionResult> GetBreakingNews()
    {
        try
        {
            var connection = _context.Database.GetDbConnection();

            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            command.CommandText = @"
            SELECT id, title
            FROM articles
            WHERE is_breaking = 1
            ORDER BY created_at DESC";

            using var reader = await command.ExecuteReaderAsync();

            var articles = new List<BreakingNewsDto>();

            while (await reader.ReadAsync())
            {
                articles.Add(new BreakingNewsDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Title = reader.GetString(reader.GetOrdinal("title"))
                });
            }

            return Ok(new ApiResponse<List<BreakingNewsDto>>
            {
                Success = true,
                Message = "OK",
                Data = articles
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<BreakingNewsDto>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
        finally
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }

    [HttpGet("articles")]
    public async Task<IActionResult> GetArticles(
    [FromQuery] string tab = "latest",
    [FromQuery] int page = 1,
    [FromQuery] int limit = 4)
    {
        if (!new[] { "latest", "think", "health" }.Contains(tab, StringComparer.OrdinalIgnoreCase))
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
            var connection = _context.Database.GetDbConnection();

            await connection.OpenAsync();

            var category = tab.ToLowerInvariant() switch
            {
                "think" => "Think",
                "health" => "Health",
                _ => null
            };

            using var countCommand = connection.CreateCommand();

            countCommand.CommandText = @"
            SELECT COUNT(*)
            FROM articles a
            LEFT JOIN categories c ON a.category_id = c.id
            WHERE (@category IS NULL OR c.name = @category)";

            var countCategoryParameter = countCommand.CreateParameter();
            countCategoryParameter.ParameterName = "@category";
            countCategoryParameter.Value = category is null ? DBNull.Value : category;
            countCommand.Parameters.Add(countCategoryParameter);

            var totalItems = Convert.ToInt32(
                await countCommand.ExecuteScalarAsync()
            );

            var totalPages = (int)Math.Ceiling((double)totalItems / limit);

            using var command = connection.CreateCommand();

            command.CommandText = @"
            SELECT
                a.id,
                a.title,
                a.short_description,
                a.image_url,
                a.created_at,
                a.read_time_minutes,
                c.id AS category_id,
                c.name AS category_name,
                c.slug AS category_slug,
                c.badge_color,
                au.id AS author_id,
                au.name AS author_name,
                ISNULL(s.likes_count, 0) AS likes_count,
                ISNULL(s.shares_count, 0) AS shares_count,
                ISNULL(s.bookmarks_count, 0) AS bookmarks_count
            FROM articles a
            LEFT JOIN categories c ON a.category_id = c.id
            LEFT JOIN authors au ON a.author_id = au.id
            LEFT JOIN article_stats s ON a.id = s.article_id
            WHERE (@category IS NULL OR c.name = @category)
            ORDER BY a.created_at DESC, a.id DESC
            OFFSET @offset ROWS
            FETCH NEXT @limit ROWS ONLY";

            var categoryParameter = command.CreateParameter();
            categoryParameter.ParameterName = "@category";
            categoryParameter.Value = category is null ? DBNull.Value : category;
            command.Parameters.Add(categoryParameter);

            var offsetParameter = command.CreateParameter();
            offsetParameter.ParameterName = "@offset";
            offsetParameter.Value = (page - 1) * limit;
            command.Parameters.Add(offsetParameter);

            var limitParameter = command.CreateParameter();
            limitParameter.ParameterName = "@limit";
            limitParameter.Value = limit;
            command.Parameters.Add(limitParameter);

            using var reader = await command.ExecuteReaderAsync();

            var articles = new List<ArticleDto>();

            while (await reader.ReadAsync())
            {
                articles.Add(new ArticleDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    ShortDescription = reader.GetString(reader.GetOrdinal("short_description")),
                    ImageUrl = reader.GetString(reader.GetOrdinal("image_url")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    ReadTimeMinutes = reader.GetInt32(reader.GetOrdinal("read_time_minutes")),

                    Category = new CategoryDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("category_id")),
                        Name = reader.GetString(reader.GetOrdinal("category_name")),
                        Slug = reader.GetString(reader.GetOrdinal("category_slug")),
                        BadgeColor = reader.IsDBNull(reader.GetOrdinal("badge_color"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("badge_color"))
                    },

                    Author = new AuthorDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("author_id")),
                        Name = reader.GetString(reader.GetOrdinal("author_name"))
                    },

                    Stats = new ArticleStatsDto
                    {
                        LikesCount = reader.GetInt32(reader.GetOrdinal("likes_count")),
                        SharesCount = reader.GetInt32(reader.GetOrdinal("shares_count")),
                        BookmarksCount = reader.GetInt32(reader.GetOrdinal("bookmarks_count"))
                    }
                });
            }

            return Ok(new ApiResponse<List<ArticleDto>>
            {
                Success = true,
                Message = "OK",
                Data = articles,
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
        finally
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }

    [HttpGet("live")]
    public async Task<IActionResult> GetLiveNews()
    {
        try
        {
            var connection = _context.Database.GetDbConnection();

            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            command.CommandText = @"
            SELECT TOP 1
                a.id,
                a.title,
                a.image_url,
                c.name AS category_name
            FROM articles a
            LEFT JOIN categories c ON a.category_id = c.id
            WHERE a.is_live = 1
            ORDER BY a.created_at DESC";

            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return NotFound(new ApiResponse<LiveNewsDto>
                {
                    Success = false,
                    Message = "Live news not found"
                });
            }

            var liveNews = new LiveNewsDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                ImageUrl = reader.GetString(reader.GetOrdinal("image_url")),

                Category = new LiveCategoryDto
                {
                    Name = reader.GetString(reader.GetOrdinal("category_name"))
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
        finally
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }

    [HttpGet("editors-picks")]
    public async Task<IActionResult> GetEditorsPicks()
    {
        try
        {
            var connection = _context.Database.GetDbConnection();

            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            command.CommandText = @"
            SELECT
                a.id,
                a.title,
                a.image_url,
                c.id AS category_id,
                c.name AS category_name,
                c.badge_color,
                ISNULL(s.likes_count, 0) AS likes_count,
                ISNULL(s.shares_count, 0) AS shares_count,
                ISNULL(s.bookmarks_count, 0) AS bookmarks_count
            FROM articles a
            LEFT JOIN categories c ON a.category_id = c.id
            LEFT JOIN article_stats s ON a.id = s.article_id
            WHERE a.is_editors_pick = 1
            ORDER BY a.created_at DESC";

            using var reader = await command.ExecuteReaderAsync();

            var editorsPicks = new List<EditorsPickDto>();

            while (await reader.ReadAsync())
            {
                editorsPicks.Add(new EditorsPickDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    ImageUrl = reader.GetString(reader.GetOrdinal("image_url")),

                    Category = new EditorsPickCategoryDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("category_id")),
                        Name = reader.GetString(reader.GetOrdinal("category_name")),
                        BadgeColor = reader.IsDBNull(reader.GetOrdinal("badge_color"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("badge_color"))
                    },

                    Stats = new ArticleStatsDto
                    {
                        LikesCount = reader.GetInt32(reader.GetOrdinal("likes_count")),
                        SharesCount = reader.GetInt32(reader.GetOrdinal("shares_count")),
                        BookmarksCount = reader.GetInt32(reader.GetOrdinal("bookmarks_count"))
                    }
                });
            }

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
        finally
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }


}