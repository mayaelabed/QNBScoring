using QNBScoring.Core.DTOs;
using QNBScoring.Core.Interfaces;
<<<<<<< HEAD

namespace QNBScoring.Infrastructure.Services;

public class ScoringService : IScoringService
{
    public IEnumerable<ClientScoreResultDto> Score(IEnumerable<ClientRequestDto> requests)
    {
        var results = new List<ClientScoreResultDto>();

        foreach (var req in requests)
        {
            int score = 0;
            string justification = "";

            if (req.IncidentCount == 0)
            {
                score += 50;
            }
            else if (req.IncidentCount <= 2)
            {
                score += 30;
                justification += "Quelques incidents détectés. ";
            }
            else
            {
                justification += "Trop d'incidents. ";
            }

            var accountAge = (DateTime.Now - req.AccountOpened).TotalDays / 365;
            if (accountAge >= 5)
            {
                score += 30;
            }
            else if (accountAge >= 2)
            {
                score += 20;
                justification += "Ancienneté moyenne. ";
            }
            else
            {
                justification += "Compte récent. ";
            }

            string decision = score >= 60 ? "Accepté" : "Refusé";

            results.Add(new ClientScoreResultDto
            {
                ClientId = req.ClientId,
                Score = score,
                Decision = decision,
                Justification = justification
            });
        }

        return results;
    }

    IEnumerable<ClientRequestDto> IScoringService.Score(IEnumerable<ClientRequestDto> requests)
    {
        throw new NotImplementedException();
=======
using QNBScoring.Core.Models;
using System;
using System.Collections.Generic;

namespace QNBScoring.Core.Services
{
    public class ScoringService : IScoringService
    {
        public string? GetLastResults()
        {
            throw new NotImplementedException();
        }

        // Method to score a list of client requests
        public List<ClientScoreResultDto> Score(List<ClientRequestDto> clientRequests)
        {
            var scoredResults = new List<ClientScoreResultDto>();

            // Loop through each client request to score it
            foreach (var request in clientRequests)
            {
                var result = new ClientScoreResultDto
                {
                    ClientId = request.ClientId,
                    Score = (int)CalculateScore(request),  // Call the method to calculate the score
                    Decision = DetermineDecision(request),  // Call the method to determine the decision
                    Justification = "Score calculated based on client data"  // Provide a justification for the decision
                };
                scoredResults.Add(result);  // Add the scored result to the list
            }

            return scoredResults;  // Return the list of scored results
        }

        public IEnumerable<ClientScoreResultDto> Score(IEnumerable<ClientRequestDto> requests)
        {
            throw new NotImplementedException();
        }

        // Method to calculate the score based on the request
        private double CalculateScore(ClientRequestDto request)
        {
            double score = 0;

            // Example: Higher income leads to higher score
            score += request.Income > 50000 ? 20 : 10;  // Add 20 points if income > 50,000

            // Example: Incidents reduce the score
            if (request.HasIncident)  // If the client has any incidents
            {
                score -= 15;  // Deduct points for incidents
            }

            // Example: Duration of relationship increases score
            int yearsWithBank = DateTime.Now.Year - request.RelationshipStartYear;
            score += yearsWithBank * 5;  // Add 5 points for each year with the bank

            // Example: Credit limit affects the score
            score += request.CreditLimit / 1000;  // 1 point for every 1,000 units of credit limit

            // Example: Other factors (this can be extended as needed)
            if (request.HasLoan) score -= 10;  // Deduct points for existing loan

            // Ensure score is within a reasonable range
            score = Math.Max(0, Math.Min(100, score));  // Clamp the score between 0 and 100

            return score;
        }

        // Method to determine the decision (Accepted or Rejected)
        private string DetermineDecision(ClientRequestDto request)
        {
            double score = CalculateScore(request);  // Reuse the scoring logic here

            // Decision threshold (for example, if score > 70, accept; else reject)
            if (score > 70)
            {
                return "Accepted";  // Accept if score > 70
            }
            else
            {
                return "Rejected";  // Reject if score <= 70
            }
        }
>>>>>>> 42f6f51 (additionnal fuctionnality)
    }
}
