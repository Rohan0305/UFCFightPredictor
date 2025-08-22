using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace UFCFightPredictorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FighterController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _rapidApiKey;
        private readonly string _rapidApiHost = "mma-stats.p.rapidapi.com";

        public FighterController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _rapidApiKey = Environment.GetEnvironmentVariable("RAPIDAPI_KEY") ?? 
                configuration["RapidAPI:Key"] ?? 
                throw new InvalidOperationException("RapidAPI key not found");
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Fighter>>> SearchFighters([FromQuery(Name = "name")] string name)
        {
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://mma-stats.p.rapidapi.com/search?name={name}"),
                    Headers =
                    {
                        { "X-RapidAPI-Key", _rapidApiKey },
                        { "X-RapidAPI-Host", _rapidApiHost }
                    }
                };

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var fighters = JsonSerializer.Deserialize<List<Fighter>>(json);
                    
                    Console.WriteLine($"Deserialized fighters count: {fighters?.Count ?? 0}");
                    return Ok(fighters);
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"Failed to search fighters: {errorContent}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error searching fighters: {ex.Message}");
            }
        }
    }

    public class Fighter
    {
        [JsonPropertyName("Name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("Nickname")]
        public string? Nickname { get; set; }
        
        [JsonPropertyName("Division Title")]
        public string? DivisionTitle { get; set; }
        
        [JsonPropertyName("Division Body")]
        public DivisionBody? DivisionBody { get; set; }
        
        [JsonPropertyName("Fighter Bio")]
        public FighterBio? FighterBio { get; set; }
        
        [JsonPropertyName("Win Stats")]
        public WinStats? WinStats { get; set; }
        
        [JsonPropertyName("Records")]
        public Records? Records { get; set; }
        
        [JsonPropertyName("Win by Method")]
        public WinByMethod? WinByMethod { get; set; }
        
        [JsonPropertyName("Last Fight")]
        public LastFight? LastFight { get; set; }
        
        [JsonPropertyName("Fighter Facts")]
        public List<string>? FighterFacts { get; set; }
        
        [JsonPropertyName("UFC History")]
        public List<string>? UfcHistory { get; set; }
        
        [JsonPropertyName("Additional Information")]
        public Dictionary<string, object>? AdditionalInformation { get; set; }
        
        [JsonPropertyName("Image Link")]
        public string? ImageLink { get; set; }
    }

    public class DivisionBody
    {
        [JsonPropertyName("Wins")]
        public string? Wins { get; set; }
        
        [JsonPropertyName("Losses")]
        public string? Losses { get; set; }
        
        [JsonPropertyName("Draws")]
        public string? Draws { get; set; }
    }

    public class FighterBio
    {
        [JsonPropertyName("Status")]
        public string? Status { get; set; }
        
        [JsonPropertyName("Hometown")]
        public string? Hometown { get; set; }
        
        [JsonPropertyName("Trains at")]
        public string? TrainsAt { get; set; }
        
        [JsonPropertyName("Fighting style")]
        public string? FightingStyle { get; set; }
        
        [JsonPropertyName("Age")]
        public string? Age { get; set; }
        
        [JsonPropertyName("Height")]
        public string? Height { get; set; }
        
        [JsonPropertyName("Weight")]
        public string? Weight { get; set; }
        
        [JsonPropertyName("Octagon Debut")]
        public string? OctagonDebut { get; set; }
        
        [JsonPropertyName("Reach")]
        public string? Reach { get; set; }
        
        [JsonPropertyName("Leg reach")]
        public string? LegReach { get; set; }
    }

    public class WinStats
    {
        [JsonPropertyName("Wins by Knockout")]
        public string? WinsByKnockout { get; set; }
        
        [JsonPropertyName("Wins by Submission")]
        public string? WinsBySubmission { get; set; }
        
        [JsonPropertyName("Former Champion")]
        public string? FormerChampion { get; set; }
        
        [JsonPropertyName("Fight Win Streak")]
        public string? FightWinStreak { get; set; }
    }

    public class Records
    {
        [JsonPropertyName("Sig. Str. Landed")]
        public string? SigStrLanded { get; set; }
        
        [JsonPropertyName("Sig. Str. Absorbed")]
        public string? SigStrAbsorbed { get; set; }
        
        [JsonPropertyName("Takedown avg")]
        public string? TakedownAvg { get; set; }
        
        [JsonPropertyName("Submission avg")]
        public string? SubmissionAvg { get; set; }
        
        [JsonPropertyName("Sig. Str. Defense")]
        public string? SigStrDefense { get; set; }
        
        [JsonPropertyName("Takedown Defense")]
        public string? TakedownDefense { get; set; }
        
        [JsonPropertyName("Knockdown Avg")]
        public string? KnockdownAvg { get; set; }
        
        [JsonPropertyName("Average fight time")]
        public string? AverageFightTime { get; set; }
        
        [JsonPropertyName("Sig. Strikes Landed")]
        public string? SigStrikesLanded { get; set; }
        
        [JsonPropertyName("Sig. Strikes Attempted")]
        public string? SigStrikesAttempted { get; set; }
        
        [JsonPropertyName("Takedowns Landed")]
        public string? TakedownsLanded { get; set; }
        
        [JsonPropertyName("Takedowns Attempted")]
        public string? TakedownsAttempted { get; set; }
        
        [JsonPropertyName("Striking accuracy")]
        public string? StrikingAccuracy { get; set; }
        
        [JsonPropertyName("Takedown Accuracy")]
        public string? TakedownAccuracy { get; set; }
    }

    public class WinByMethod
    {
        [JsonPropertyName("Standing")]
        public string? Standing { get; set; }
        
        [JsonPropertyName("Clinch")]
        public string? Clinch { get; set; }
        
        [JsonPropertyName("Ground")]
        public string? Ground { get; set; }
    }

    public class LastFight
    {
        [JsonPropertyName("Event")]
        public string? Event { get; set; }
        
        [JsonPropertyName("Date")]
        public string? Date { get; set; }
    }

    public class PredictionRequest
    {
        public Fighter Fighter1 { get; set; } = new();
        public Fighter Fighter2 { get; set; } = new();
    }

    public class PredictionResult
    {
        public string Winner { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
} 