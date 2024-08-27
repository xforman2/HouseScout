using System.Text.RegularExpressions;
using HouseScout.Clients;
using HouseScout.DTOs;
using HouseScout.Model;

namespace HouseScout.Mappers;

public class SrealityMapper
{
    public List<Estate> MapResponseToModel(SrealityResponseDTO responseDto)
    {
        var estatesFromResponse = responseDto.Embedded.Estates;

        return estatesFromResponse.Select(MapSingleToModel).ToList();

    }
    private Estate MapSingleToModel(SrealityEstate srealityEstate)
    {
        return new Estate(ApiType.SREALITY, srealityEstate.Id, srealityEstate.Locality, srealityEstate.Price, CreateURL(srealityEstate), CreateSurface(srealityEstate), EstateType.APARTMENT, OfferType.RENT);

    }
    /// <summary>
    /// method exists because sreality puts type of home/flat into url
    /// </summary>
    /// <param name="input"> name in sreality system from which the type of home/flat is extracted</param>
    /// <returns></returns>
    private string GetCategory(string input)
    {
        string numberPattern = @"(\d+)\s*\+\s*(\d+)|(\d+)\s*\+\s*kk";
        Regex regex = new Regex(numberPattern, RegexOptions.IgnoreCase);

        Match match = regex.Match(input);

        if (match.Success)
        {   
            // case for num+num
            if (match.Groups[1].Success && match.Groups[2].Success)
            {
                int number1 = int.Parse(match.Groups[1].Value);
                int number2 = int.Parse(match.Groups[2].Value);
                return $"{number1}+{number2}";
            }
            // case for num+kk
            if (match.Groups[3].Success)
            {
                int number = int.Parse(match.Groups[3].Value);
                return $"{number}+kk";
            }
        }
        // cases for other types, 6 a vice, pokoj and atypicke
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

    private string CreateURL(SrealityEstate srealityEstate)
    {
        
        return $"https://www.sreality.cz/detail/pronajem/byt/{GetCategory(srealityEstate.Name)}/{srealityEstate.Seo.Locality}/{srealityEstate.Id}";
    }

    private int CreateSurface(SrealityEstate srealityEstate)
    {
        string pattern = @"(\d+)\s?m²";
        Match match = Regex.Match(srealityEstate.Name, pattern);
        
        if (match.Success)
        {
            string capturedNumber = match.Groups[1].Value;
            int surfaceArea = int.Parse(capturedNumber);
            return surfaceArea;
        }

        return 0;
    }
}