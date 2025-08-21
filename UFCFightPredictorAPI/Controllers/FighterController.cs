using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;

namespace UFCFightPredictorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FighterController : ControllerBase
    {
        static FighterController()
        {
            Console.WriteLine("FighterController class loaded!");
        }
        
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

                [HttpGet("test")]
        public ActionResult<string> Test()
        {
            return Ok("FighterController is working! Route: " + Request.Path);
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
                Console.WriteLine($"RapidAPI Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"RapidAPI Response Body: {json}");
                    var fighter = JsonSerializer.Deserialize<List<Fighter>>(json);
                    return Ok(fighter);
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"RapidAPI Error Content: {errorContent}");
                return StatusCode((int)response.StatusCode, $"Failed to search fighter from RapidAPI MMA Stats. Status: {response.StatusCode}, Error: {errorContent}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in search: {ex.Message}");
                return StatusCode(500, $"Error searching fighters: {ex.Message}");
            }
        }
    }

    public class Fighter
    {
        public string Name { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string DivisionTitle { get; set; } = string.Empty;
        public DivisionBody DivisionBody { get; set; } = new();
        public FighterBio FighterBio { get; set; } = new();
        public WinStats WinStats { get; set; } = new();
        public Records Records { get; set; } = new();
        public WinByMethod WinByMethod { get; set; } = new();
        public LastFight LastFight { get; set; } = new();
        public List<string> FighterFacts { get; set; } = new();
        public List<string> UFCHistory { get; set; } = new();
        public Dictionary<string, object> AdditionalInformation { get; set; } = new();
    }

    public class DivisionBody
    {
        public string Wins { get; set; } = string.Empty;
        public string Losses { get; set; } = string.Empty;
        public string Draws { get; set; } = string.Empty;
    }

    public class FighterBio
    {
        public string Hometown { get; set; } = string.Empty;
        public string FightingStyle { get; set; } = string.Empty;
        public string Age { get; set; } = string.Empty;
        public string Height { get; set; } = string.Empty;
        public string Weight { get; set; } = string.Empty;
        public string OctagonDebut { get; set; } = string.Empty;
        public string Reach { get; set; } = string.Empty;
        public string LegReach { get; set; } = string.Empty;
    }

    public class WinStats
    {
        public string WinsByKnockout { get; set; } = string.Empty;
        public string WinsBySubmission { get; set; } = string.Empty;
        public string FirstRoundFinishes { get; set; } = string.Empty;
    }

    public class Records
    {
        public string SigStrLanded { get; set; } = string.Empty;
        public string SigStrAbsorbed { get; set; } = string.Empty;
        public string TakedownAvg { get; set; } = string.Empty;
        public string SubmissionAvg { get; set; } = string.Empty;
        public string SigStrDefense { get; set; } = string.Empty;
        public string TakedownDefense { get; set; } = string.Empty;
        public string KnockdownAvg { get; set; } = string.Empty;
        public string AverageFightTime { get; set; } = string.Empty;
        public string SigStrikesLanded { get; set; } = string.Empty;
        public string SigStrikesAttempted { get; set; } = string.Empty;
        public string TakedownsLanded { get; set; } = string.Empty;
        public string TakedownsAttempted { get; set; } = string.Empty;
        public string StrikingAccuracy { get; set; } = string.Empty;
        public string TakedownAccuracy { get; set; } = string.Empty;
    }

    public class WinByMethod
    {
        public string Standing { get; set; } = string.Empty;
        public string Clinch { get; set; } = string.Empty;
        public string Ground { get; set; } = string.Empty;
    }

    public class LastFight
    {
        public string Event { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}
