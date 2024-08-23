using System.Text.RegularExpressions;
using HouseScout.Clients;
using HouseScout.Model;
using Advert = HouseScout.Model.Advert;

namespace HouseScout.Mappers;

public class SrealityMapper
{
    public List<Estate> MapResponseToModel(SrealityResponse response)
    {
        var estatesFromResponse = response.Embedded.Estates;

        return estatesFromResponse.Select(MapSingleToModel).ToList();

    }
    private Estate MapSingleToModel(EstateDTO estate)
    {
        return new Estate("sreality", estate.Id, estate.Locality, estate.Price, 0, CreateURL(estate), CreateSurface(estate), "BYT", "PRONAJEM");

    }
    /// <summary>
    /// funkcia existuje kvoli tomu ze sreality dava do url typ bytu/domu (uplne zbytocne)
    /// </summary>
    /// <param name="input"> meno v systeme sreality, z ktoreho treba sparsovat type do url</param>
    /// <returns></returns>
    private string GetCategory(string input)
    {
        string numberPattern = @"(\d+)\s*\+\s*(\d+)|(\d+)\s*\+\s*kk";
        Regex regex = new Regex(numberPattern, RegexOptions.IgnoreCase);

        Match match = regex.Match(input);

        if (match.Success)
        {   
            // case pre num+num
            if (match.Groups[1].Success && match.Groups[2].Success)
            {
                int number1 = int.Parse(match.Groups[1].Value);
                int number2 = int.Parse(match.Groups[2].Value);
                return $"{number1}+{number2}";
            }
            // case pre num+kk
            if (match.Groups[3].Success)
            {
                int number = int.Parse(match.Groups[3].Value);
                return $"{number}+kk";
            }
        }
        // potom este existuju 6 a vice, pokoj a atypicke
        if (input.Contains("6 pokojů a více"))
        {
            return "6-a-vice";
        }
        if (input.Contains("pokoje"))
        {
            return "pokoj";
        }

        if (input.Contains("atypické"))
        {
            return "atypicky";
        }

        return "unknown";
    }

    private string CreateURL(EstateDTO estate)
    {
        
        return $"https://www.sreality.cz/detail/pronajem/byt/{GetCategory(estate.Name)}/{estate.Seo.Locality}/{estate.Id}";
    }

    private int CreateSurface(EstateDTO estate)
    {
        string pattern = @"(\d+)\s?m²";
        Match match = Regex.Match(estate.Name, pattern);
        
        if (match.Success)
        {
            string capturedNumber = match.Groups[1].Value;
            int surfaceArea = int.Parse(capturedNumber);
            return surfaceArea;
        }

        return 0;
    }
}