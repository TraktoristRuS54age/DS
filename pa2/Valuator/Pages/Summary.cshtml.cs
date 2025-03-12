using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IDatabase _db;

    public SummaryModel(IDatabase db, ILogger<SummaryModel> logger)
    {
        _db = db;
        _logger = logger;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        string rankKey = "RANK-" + id;
        string similarityKey = "SIMILARITY-" + id;

        var rankValue = _db.StringGet(rankKey);
        var similarityValue = _db.StringGet(similarityKey);

        if (rankValue.HasValue)
        {
            Rank = (double)rankValue;
        }

        if (similarityValue.HasValue)
        {
            Similarity = (double)similarityValue;
        }
        // TODO: (pa1) проинициализировать свойства Rank и Similarity значениями из БД (Redis)
    }
}
