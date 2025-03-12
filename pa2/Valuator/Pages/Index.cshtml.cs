using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly IDatabase _db;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IDatabase db, ILogger<IndexModel> logger)
    {
        _db = db;
        _logger = logger;
    }

    public void OnGet()
    {

    }
    public static bool IsAlphabet(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
       (c >= 'а' && c <= 'я') || (c >= 'А' && c <= 'Я');
    }
    public static double Calculate(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0.0;

        int nonAlphabeticCount = 0;
        foreach (char c in text)
        {
            if (!IsAlphabet(c))
            {
                nonAlphabeticCount++;
            }
        }

        return (double)nonAlphabeticCount / text.Length;
    }

    private bool DuplicateText(string text)
    {
        var keys = _db.Multiplexer.GetServer("localhost:6379").Keys(pattern: "TEXT-*");
        foreach (var key in keys)
        {
            var storedText = _db.StringGet(key);
            if(storedText == text)
            {
                return true;
            }
        }
        return false;
    }

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();
        string textKey = "TEXT-" + id;
        // TODO: (pa1) сохранить в БД (Redis) text по ключу textKey

        string rankKey = "RANK-" + id;
        double rank = Calculate(text);
        rank = Math.Round(rank, 2);
        _db.StringSet(rankKey, rank);
        // TODO: (pa1) посчитать rank и сохранить в БД (Redis) по ключу rankKey

        string similarityKey = "SIMILARITY-" + id;
        double similarity = DuplicateText(text) ? 1.0 : 0.0;
        _db.StringSet(similarityKey, similarity);
        if (similarity == 1)
        {
            return Redirect($"summary?id={id}");
        }
        // TODO: (pa1) посчитать similarity и сохранить в БД (Redis) по ключу similarityKey
        
        _db.StringSet(textKey, text);
        if(text != null)
        {
            return Redirect($"summary?id={id}");
        }
        return Redirect($"index");
    }
}
