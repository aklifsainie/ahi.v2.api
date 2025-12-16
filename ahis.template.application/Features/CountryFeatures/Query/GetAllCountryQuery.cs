using ahis.template.application.Shared;
using ahis.template.application.Shared.Mediator;
using ahis.template.domain.Interfaces.Repositories;
using ahis.template.domain.Models.Entities;
using ahis.template.domain.Models.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.application.Features.CountryFeatures.Query
{
    public class GetAllCountryQuery : IRequest<ApiResponse<List<CountryVM>>>
    {
    }

    public class GetAllCountryQueryHandler : IRequestHandler<GetAllCountryQuery, ApiResponse<List<CountryVM>>>
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ILogger<GetAllCountryQueryHandler> _logger;

        public GetAllCountryQueryHandler(ICountryRepository countryRepository, ILogger<GetAllCountryQueryHandler> logger)
        {
            _countryRepository = countryRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<CountryVM>>> Handle(GetAllCountryQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Handling GetAllCountryQueryHandler");

                // Get data from repository then populate into Category entity
                List<Country> categoryEntity = await _countryRepository.GetAllAsync();

                // If no data found
                if (categoryEntity == null || !categoryEntity.Any())
                {
                    _logger.LogWarning("No countries found.");
                    return ApiResponse<List<CountryVM>>.SuccessResponse(new List<CountryVM>(), "No country data found.");
                }

                // Manual map entity to view model
                List<CountryVM> categoryVM = categoryEntity
                    .Select(c => new CountryVM
                    {
                        CountryFullname = c.CountryFullname,
                        CountryShortname = c.CountryShortname,
                        CountryDescription = c.CountryDescription,
                        CountryCode2 = c.CountryCode2,
                        CountryCode3 = c.CountryCode3
                    })
                    .ToList();

                _logger.LogInformation("Successfully retrieved {Count} countries", categoryVM.Count);

                return ApiResponse<List<CountryVM>>.SuccessResponse(categoryVM, "List of all countries obtained");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetAllCountryQueryHandler");


                // Return error response with details
                return ApiResponse<List<CountryVM>>.ErrorResponse(
                    message: "Failed to get country list.",
                    errors: new
                    {
                        ExceptionMessage = ex.Message,
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    });
            }

        }
    }
}
