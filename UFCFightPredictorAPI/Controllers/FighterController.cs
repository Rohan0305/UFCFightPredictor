using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
        public async Task<ActionResult<IEnumerable<Fighter>>> SearchFighters([FromQuery] string name)
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

        [HttpPost("predict")]
        public ActionResult<PredictionResult> PredictWinner([FromBody] PredictionRequest request)
        {
            try
            {
                // Simple ML prediction based on fighter stats
                var winner = PredictWinner(fighter1: request.Fighter1, fighter2: request.Fighter2);
                
                return Ok(new PredictionResult
                {
                    Winner = winner,
                    Confidence = 0.75, // Simple confidence score
                    Reason = $"Based on win rate and fight statistics"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error making prediction: {ex.Message}");
            }
        }

        private string PredictWinner(Fighter fighter1, Fighter fighter2)
        {
            // Simple prediction logic based on win rate
            var fighter1Wins = int.Parse(fighter1.DivisionBody.Wins);
            var fighter1Losses = int.Parse(fighter1.DivisionBody.Losses);
            var fighter1WinRate = fighter1Wins / (double)(fighter1Wins + fighter1Losses);

            var fighter2Wins = int.Parse(fighter2.DivisionBody.Wins);
            var fighter2Losses = int.Parse(fighter2.DivisionBody.Losses);
            var fighter2WinRate = fighter2Wins / (double)(fighter2Wins + fighter2Losses);

            // Add some randomness to make it interesting
            var random = new Random();
            var randomFactor = random.NextDouble() * 0.2 - 0.1; // ±10% randomness

            var adjustedFighter1Rate = fighter1WinRate + randomFactor;
            var adjustedFighter2Rate = fighter2WinRate + randomFactor;

            return adjustedFighter1Rate > adjustedFighter2Rate ? fighter1.Name : fighter2.Name;
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